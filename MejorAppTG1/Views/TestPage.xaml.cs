using System.Globalization;
using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using MejorAppTG1.Utils;

namespace MejorAppTG1
{
    public partial class TestPage : ContentPage
    {
        #region Variables
        private Button? _selectedButton;
        private readonly List<Question> _questions;
        private int _index = 0;
        private List<Answer> _answers = [];
        private readonly Test _test;
        #endregion

        #region Constructores        
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="TestPage"/>.
        /// </summary>
        /// <param name="cuestionario">El test actual de la base de datos.</param>
        /// <param name="questions">Las preguntas del test.</param>
        /// <param name="unfinished">Si es <c>true</c>, es un test a medias. En caso contrario, es un test nuevo.</param>
        public TestPage(Test cuestionario, List<Question> questions, bool unfinished)
        {
            InitializeComponent();
            Shell.SetNavBarIsVisible(this, false);
            this._test = cuestionario;
            this._questions = questions;

            if (cuestionario.Tipo == App.TCA_TEST_KEY) {
                BtnOpcion2.Text = Strings.str_TestPage_BtnOption2_2;
                BtnOpcion3.Text = Strings.str_TestPage_BtnOption3_2;
                BtnOpcion4.Text = Strings.str_TestPage_BtnOption4_2;
            }
            else if (cuestionario.Tipo == App.QUICK_TEST_KEY) {
                BtnOpcion2.Text = Strings.str_TestPage_BtnOption2_3;
                BtnOpcion3.Text = Strings.str_TestPage_BtnOption3_2;
                BtnOpcion4.Text = Strings.str_TestPage_BtnOption4_3;
            }

            if (unfinished) {
                _ = InitializeAsync();
            }
            else {
                this._answers = [];
                ContentPage_Loaded();
            }
        }
        #endregion

        #region Eventos
        /// <summary>
        /// Maneja el evento de pulsación de cualquier botón de respuesta. Almacena la respuesta en la base de datos.
        /// </summary>
        /// <param name="sender">El botón pulsado.</param>
        /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
        private async void OnBtnEncuesta_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            await button.ScaleTo(0.9, 80, Easing.CubicOut);
            await button.ScaleTo(1.1, 80, Easing.CubicIn);
            await button.ScaleTo(1.0, 80, Easing.CubicOut);
            int valor = int.Parse(button.CommandParameter.ToString());

            // Cambiar el color del botón seleccionado
            CambiarColorBotones(valor);

            // Guardar la respuesta
            _answers[_index].ValorRespuesta = valor;
            _answers[_index].Factor = _questions[_index].Factor;

            await App.Database.UpdateOrInsertAnswerAsync(new Answer {
                IdPregunta = _index + 1,
                IdTest = _test.IdTest,
                Factor = _questions[_index].Factor,
                ValorRespuesta = valor
            });

            var savedAnswers = await GetSavedAnswers();
            double progress = (double)savedAnswers.Count / _questions.Count;
            await progressBar.ProgressTo(progress, 150, Easing.SinInOut);
        }

        /// <summary>
        /// Maneja el evento de pulsación del botón de Anterior. Pasa el formulario a la pregunta anterior.
        /// </summary>
        /// <param name="sender">El botón pulsado.</param>
        /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
        private void BtnAnterior_Clicked(object sender, EventArgs e)
        {
            App.AnimateButtonInOut(sender);
            if (_index > 0) {
                _index -= 1;
                ActualizarProgreso();
            }
        }

        /// <summary>
        /// Maneja el evento de pulsación del botón de Siguiente/Finalizar. Pasa el formulario a la siguiente pregunta o, si es la última, calcula los resultados y muestra la pantalla de recomendaciones.
        /// </summary>
        /// <param name="sender">El botón pulsado.</param>
        /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
        private async void BtnSiguiente_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;

            try {
                App.AnimateButtonInOut(sender);
                if (_selectedButton == null) {
                    await DisplayAlert(Strings.str_TestPage_EmptyAnswer, Strings.str_TestPage_EmptyAnswer_Msg, Strings.str_ResultHistoryPage_BtnCheck_OK);
                }
                else {
                    _selectedButton = null;
                    if (_index < _questions.Count - 1) {
                        _index += 1;
                        ActualizarProgreso();
                    }
                    else {
                        Factor? factor1 = null, factor2 = null, factor3 = null;
                        if (_test.Tipo != App.TCA_TEST_KEY) {
                            var resultados = await Task.WhenAll(
                                Task.Run(() => ScoreCalculator.CalculoFactores(_answers, App.FACTORS_1, _test)),
                                Task.Run(() => ScoreCalculator.CalculoFactores(_answers, App.FACTORS_2, _test)),
                                Task.Run(() => ScoreCalculator.CalculoFactores(_answers, App.FACTORS_3, _test))
                            );

                            factor1 = resultados[0];
                            factor2 = resultados[1];
                            factor3 = resultados[2];
                        }
                        else {
                            factor1 = ScoreCalculator.CalculoFactores(_answers, App.FACTORS_1, _test);
                        }

                        // Llamar a la ventana de resultados
                        await Navigation.PushAsync(new ResultsPage(factor1, factor2, factor3, _test.Tipo), true);
                        if (Navigation.NavigationStack.Count > 0) {
                            Navigation.RemovePage(Navigation.NavigationStack[^1]);
                        }

                        _test.Terminado = true;
                        _test.Fecha = DateTime.Now;
                        await App.Database.UpdateTestAsync(_test);
                    }
                }
            }
            finally {
                App.ButtonPressed = false;
            }
        }

        /// <summary>
        /// Maneja el evento de aparición de la pantalla.
        /// </summary>
        private void ContentPage_Loaded()
        {
            LblTitulo.Text = Strings.ResourceManager.GetString(_test.Tipo, CultureInfo.CurrentUICulture);
            while (_answers.Count < _questions.Count) {
                _answers.Add(new Answer {
                    ValorRespuesta = 5 // Valor predeterminado para cargar la lista y modificarla mas tarde
                });
            }
            ActualizarProgreso();
        }

        /// <summary>
        /// Determina el comportamiento de la aplicación al pulsar el botón de Atrás. Si el usuario ha respondido alguna pregunta, se le pregunta si quiere guardar su test para continuarlo más tarde. Si no cancela la petición, volverá en cualquier caso al menú principal.
        /// </summary>
        /// <returns>
        ///   <see langword="true" /> si se ha realizado la navegación hacia atrás, si no <see langword="false" />.
        /// </returns>
        /// <remarks>
        /// </remarks>
        protected override bool OnBackButtonPressed()
        {
            if ((_selectedButton != null && _index == 0) || _index > 0) { // Si ya ha respondido a la primera pregunta, que muestre la advertencia
                // Hay que hacer otro hilo que muestre la alerta de forma asíncrona, este método tiene que devolver un boolean básico sí o sí
                Dispatcher.Dispatch(async () => {
                    string action = await DisplayActionSheet(Strings.str_TestPage_LeaveTest_Warning, Strings.str_ResultHistoryPage_BtnCancel, null, Strings.str_TestPage_LeaveTest_BtnSave, Strings.str_TestPage_LeaveTest_BtnDelete);

                    if (action == Strings.str_TestPage_LeaveTest_BtnSave) {
                        await Navigation.PopAsync(true);
                    }
                    else if (action == Strings.str_TestPage_LeaveTest_BtnDelete) {
                        await App.Database.DeleteTestAndAnswersAsync(_test);
                        await Navigation.PopAsync(true);
                    }
                });

                return true;
            }
            else {    // Si no ha respondido ni a la primera pregunta, que no pregunte y borre directamente este test
                App.Database.DeleteTestAndAnswersAsync(_test);
            }

            // Si la condición no se cumple, permite la acción de retroceso normal
            return base.OnBackButtonPressed();
        }
        #endregion

        #region Métodos        
        /// <summary>
        /// Inicializa el test.
        /// </summary>
        private async Task InitializeAsync()
        {
            var savedAnswers = await GetSavedAnswers();
            this._answers = [.. savedAnswers];

            _index = savedAnswers.Count;
            progressBar.Progress = (double)savedAnswers.Count / _questions.Count;

            // Si el usuario guarda precisamente en la última pregunta, el código lo posicionaría en un índice que no existe
            // Por tanto, quiero que lo mueva un índice hacia atrás, a la última pregunta
            if (_index >= _questions.Count) {
                _index -= 1;
            }

            ContentPage_Loaded();
        }

        /// <summary>
        /// Devuelve las respuestas almacenadas del test actual.
        /// </summary>
        /// <returns>Una lista con las respuestas.</returns>
        private async Task<List<Answer>> GetSavedAnswers()
        {
            return await App.Database.GetAnswersByTestIdAsync(_test.IdTest);
        }

        /// <summary>
        /// Cambia la pregunta y actualiza los botones.
        /// </summary>
        private void ActualizarProgreso()
        {
            LblProgreso.Text = string.Format(Strings.str_TestPage_QuestionCount, (_index + 1), _questions.Count);
            // Saco el ID del string de cada pregunta del JSON y lo busco en Resources para mostrarlo en la interfaz
            string locId = _questions[_index].Content;
            LblPregunta.Text = Strings.ResourceManager.GetString(locId, CultureInfo.CurrentUICulture);

            // Actualizar los colores de los botones
            CambiarColorBotones(_answers[_index].ValorRespuesta);

            ConfigurarBotones();

            SemanticScreenReader.Announce(string.Format(Strings.str_SemanticProperties_TestPage_QuestionCount, (_index + 1), _questions.Count));
            SemanticScreenReader.Announce(Strings.ResourceManager.GetString(locId, CultureInfo.CurrentUICulture));
        }

        /// <summary>
        /// Configura los botones según el índice de la pregunta.
        /// </summary>
        private void ConfigurarBotones()
        {
            BtnAnterior.IsEnabled = _index > 0;
            BtnAnterior.BorderColor = _index > 0
                ? (Color)Application.Current.Resources["PrimaryColor"]
                : (Color)Application.Current.Resources["BorderColor3"];

            BtnSiguiente.Text = (_index == _questions.Count - 1)
                ? Strings.str_TestPage_BtnFinish
                : Strings.str_TestPage_BtnNext;
        }

        /// <summary>
        /// Cambia el color de los botones según el valor que se haya seleccionado.
        /// </summary>
        /// <param name="valorSeleccionado">El valor seleccionado.</param>
        private void CambiarColorBotones(int valorSeleccionado)
        {
            foreach (var child in VslBotones.Children) {
                if (child is Button btn) {
                    int btnValor = int.TryParse(btn.CommandParameter.ToString(), out int cmdParam) ? cmdParam : -1;
                    btn.BackgroundColor = (Color)Application.Current.Resources["BorderColor3"];
                    if (btnValor == valorSeleccionado) {
                        _selectedButton = btn;
                        btn.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
                    }
                }
            }
        }
        #endregion
    }
}
