using System.Diagnostics;
using System.Globalization;
using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using MejorAppTG1.Utils;
using Microsoft.Maui;

namespace MejorAppTG1
{
    public partial class TestPage : ContentPage
    {
        private Button selectedButton;

        List<Question> preguntas;
        int indice = 0;
        List<Answer> valorPreguntas;
        Test cuestionario;
        int cantidadPreguntas;

        public TestPage(Test cuestionario, List<Question> questions, bool unfinished)
        {
            InitializeComponent();
            Shell.SetNavBarIsVisible(this, false);
            this.cuestionario = cuestionario;
            this.preguntas = questions;
            this.cantidadPreguntas = preguntas.Count;

            if (cuestionario.Tipo == "str_EatingTest") {
                BtnOpcion2.Text = Strings.str_TestPage_BtnOption2_2;
                BtnOpcion3.Text = Strings.str_TestPage_BtnOption3_2;
                BtnOpcion4.Text = Strings.str_TestPage_BtnOption4_2;
            } else if (cuestionario.Tipo == "str_QuickTest") {
                BtnOpcion2.Text = Strings.str_TestPage_BtnOption2_3;
                BtnOpcion3.Text = Strings.str_TestPage_BtnOption3_2;
                BtnOpcion4.Text = Strings.str_TestPage_BtnOption4_3;
            }

            if (unfinished) {
                _ = InitializeAsync();
            } else {
                this.valorPreguntas = new List<Answer>();
                ContentPage_Loaded();
            }
        }

        private async Task InitializeAsync()
        {
            var savedAnswers = await GetSavedAnswers();
            this.valorPreguntas = savedAnswers.ToList();

            indice = savedAnswers.Count;
            progressBar.Progress = (double)savedAnswers.Count / cantidadPreguntas;

            // Si el usuario guarda precisamente en la última pregunta, el código lo posicionaría en un índice que no existe
            // Por tanto, quiero que lo mueva un índice hacia atrás, a la última pregunta
            if (indice >= cantidadPreguntas) {
                indice -= 1;
            }

            ContentPage_Loaded();
        }

        private async Task<List<Answer>> GetSavedAnswers()
        {
            return await App.Database.GetAnswersByTestIdAsync(cuestionario.IdTest);
        }

        private async void OnBtnEncuesta_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;
            try {
                var button = (Button)sender;
                await button.ScaleTo(0.9, 80, Easing.CubicOut);
                await button.ScaleTo(1.1, 80, Easing.CubicIn);
                await button.ScaleTo(1.0, 80, Easing.CubicOut);
                int valor = int.Parse(button.CommandParameter.ToString());

                // Cambiar el color del botón seleccionado
                CambiarColorBotones(valor);

                // Guardar la respuesta
                valorPreguntas[indice].ValorRespuesta = valor;
                valorPreguntas[indice].Factor = preguntas[indice].Factor;

                await App.Database.UpdateOrInsertAnswerAsync(new Answer {
                    IdPregunta = indice + 1,
                    IdTest = cuestionario.IdTest,
                    Factor = preguntas[indice].Factor,
                    ValorRespuesta = valor
                });

                var savedAnswers = await GetSavedAnswers();
                double progress = (double)savedAnswers.Count / cantidadPreguntas;
                await progressBar.ProgressTo(progress, 150, Easing.SinInOut);
            } finally {
                App.ButtonPressed = false;
            }
        }

        private void BtnAnterior_Clicked(object sender, EventArgs e)
        {
            if (indice > 0)
            {
                indice -= 1;
                ActualizarProgreso();
            }
        }

        private async void BtnSiguiente_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;

            try
            {
                if (selectedButton == null)
                {
                    await DisplayAlert(Strings.str_TestPage_EmptyAnswer, Strings.str_TestPage_EmptyAnswer_Msg, Strings.str_ResultHistoryPage_BtnCheck_OK);
                }
                else
                {
                    selectedButton = null;
                    if (indice < cantidadPreguntas - 1)
                    {
                        indice += 1;
                        ActualizarProgreso();
                    }
                    else
                    {
                        Factor factor1 = null, factor2 = null, factor3 = null;
                        if (cuestionario.Tipo != "str_EatingTest")
                        {
                            var resultados = await Task.WhenAll(
                                Task.Run(() => ScoreCalculator.CalculoFactores(valorPreguntas, "1", cuestionario)),
                                Task.Run(() => ScoreCalculator.CalculoFactores(valorPreguntas, "2", cuestionario)),
                                Task.Run(() => ScoreCalculator.CalculoFactores(valorPreguntas, "3", cuestionario))
                            );

                            factor1 = resultados[0];
                            factor2 = resultados[1];
                            factor3 = resultados[2];
                        }
                        else
                        {
                            factor1 = ScoreCalculator.CalculoFactores(valorPreguntas, "1", cuestionario);
                        }

                        // Guardar la página actual para borrarla después
                        var paginaActual = Navigation.NavigationStack.LastOrDefault();

                        // Llamar a la ventana de resultados
                        await Navigation.PushAsync(new ResultsPage(factor1, factor2, factor3, cuestionario.Tipo), true);

                        // Borrar la página de cuestionarios
                        if (paginaActual != null)
                        {
                            Navigation.RemovePage(paginaActual);
                        }

                        cuestionario.Terminado = true;
                        cuestionario.Fecha = DateTime.Now;
                        await App.Database.UpdateTestAsync(cuestionario);
                    }
                }
            }
            finally
            {
                App.ButtonPressed = false;
            }
        }


        private void ContentPage_Loaded()
        {
            LblTitulo.Text = Strings.ResourceManager.GetString(cuestionario.Tipo, CultureInfo.CurrentUICulture); ;
            while (valorPreguntas.Count < cantidadPreguntas) {
                valorPreguntas.Add(new Answer {
                    ValorRespuesta = 5 // Valor predeterminado para cargar la lista y modificarla mas tarde
                });
            }
            ActualizarProgreso();
        }

        private void ActualizarProgreso()
        {
            LblProgreso.Text = string.Format(Strings.str_TestPage_QuestionCount, (indice + 1), cantidadPreguntas);
            // Saco el ID del string de cada pregunta del JSON y lo busco en Resources para mostrarlo en la interfaz
            string locId = preguntas[indice].Content;
            LblPregunta.Text = Strings.ResourceManager.GetString(locId, CultureInfo.CurrentUICulture);

            // Actualizar los colores de los botones
            CambiarColorBotones(valorPreguntas[indice].ValorRespuesta);

            ConfigurarBotones();

            SemanticScreenReader.Announce(string.Format(Strings.str_SemanticProperties_TestPage_QuestionCount, (indice + 1), cantidadPreguntas));
            SemanticScreenReader.Announce(Strings.ResourceManager.GetString(locId, CultureInfo.CurrentUICulture));
        }
        private void ConfigurarBotones() {
            BtnAnterior.IsEnabled = indice > 0;
            BtnAnterior.BorderColor = indice > 0
                ? (Color)Application.Current.Resources["PrimaryColor"]
                : (Color)Application.Current.Resources["BorderColor3"];

            BtnSiguiente.Text = (indice == cantidadPreguntas - 1)
                ? Strings.str_TestPage_BtnFinish
                : Strings.str_TestPage_BtnNext;
        }

        private void CambiarColorBotones(int valorSeleccionado)
        {
            foreach (var child in VslBotones.Children)
            {
                if (child is Button btn)
                {
                    int btnValor = int.TryParse(btn.CommandParameter.ToString(), out int cmdParam) ? cmdParam : -1;
                    btn.BackgroundColor = (Color)Application.Current.Resources["BorderColor3"];
                    if (btnValor == valorSeleccionado)
                    {
                        selectedButton = btn;
                        btn.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
                    }
                }
            }
        }
        protected override bool OnBackButtonPressed()
        {
            if ((selectedButton != null && indice == 0) || indice > 0) { // Si ya ha respondido a la primera pregunta, que muestre la advertencia
                // Hay que hacer otro hilo que muestre la alerta de forma asíncrona, este método tiene que devolver un boolean básico sí o sí
                Dispatcher.Dispatch(async () =>
                {
                    string action = await DisplayActionSheet(Strings.str_TestPage_LeaveTest_Warning, Strings.str_ResultHistoryPage_BtnCancel, null, Strings.str_TestPage_LeaveTest_BtnSave, Strings.str_TestPage_LeaveTest_BtnDelete);

                    if (action == Strings.str_TestPage_LeaveTest_BtnSave) {
                        await Navigation.PopAsync(true);
                    }
                    else if (action == Strings.str_TestPage_LeaveTest_BtnDelete) {
                        await App.Database.DeleteTestAndAnswersAsync(cuestionario);
                        await Navigation.PopAsync(true);
                    }
                });

                return true;
            } else {    // Si no ha respondido ni a la primera pregunta, que no pregunte y borre directamente este test
                App.Database.DeleteTestAndAnswersAsync(cuestionario);
            }

            // Si la condición no se cumple, permite la acción de retroceso normal
            return base.OnBackButtonPressed();
        }
    }
}
