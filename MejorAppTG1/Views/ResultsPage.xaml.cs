#pragma warning disable CS8601

using CommunityToolkit.Maui.Views;
using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;

namespace MejorAppTG1;

using Microsoft.Maui.Controls;

public partial class ResultsPage : ContentPage
{
    private List<Advice> consejosDisponibles = new();
    public ObservableCollection<Advice> Consejos { get; set; } = new ObservableCollection<Advice>();

    Factor factor1;
    Factor factor2;
    Factor factor3;

    string tipoTest;

    int puntuacionTotal = 0;

    // Constructor con factores
    public ResultsPage(Factor factor1, Factor factor2, Factor factor3, string tipoTest)
    {
        this.tipoTest = tipoTest;
        //Factores del test realizado
        this.factor1 = factor1 ?? this.factor1;
        this.factor2 = factor2 ?? this.factor2;
        this.factor3 = factor3 ?? this.factor3;

        InitializeComponent();
        Shell.SetNavBarIsVisible(this, false);
    }

    private async void ContentPage_Appearing(object sender, EventArgs e)
    {
        try {
            StkLoading.IsVisible = true;
            GrdData.IsVisible = false;
            //SemanticScreenReader.Announce(LblLoading.Text);
            await Task.Delay(800);  // Forzar a que se espere un poco antes de cargar, porque de lo contrario el indicador de carga ni siquiera aparece

            BindingContext = this;

            await Task.WhenAll(
                Task.Run(() => CalcularPuntuacionTotal()),
                Task.Run(() => CargarConsejosAsync(tipoTest))
            );
        }
        finally {
            SemanticScreenReader.Announce(LblIntro.Text + Strings.str_SemanticProperties_ResultsPage_LblIntro);
            StkLoading.IsVisible = false;
            GrdData.IsVisible = true;
        }
    }

    // Para cambiar el icono de flecha
    private void Expander_ExpandedChanged(object sender, CommunityToolkit.Maui.Core.ExpandedChangedEventArgs e)
    {
        if (sender is Expander expander && expander.BindingContext is Advice consejo)
        {
            if (string.IsNullOrEmpty(consejo.Contenido))
            {
                expander.IsExpanded = false;
                return;
            }
            else if (consejo.Contenido != null)
            {
                consejo.Imagen = expander.IsExpanded ? "flechaarriba.png" : "flechaabajo.png";
            }
        }
    }

    // Para saber la puntuación total del test
    private void CalcularPuntuacionTotal()
    {
        if (tipoTest == "str_EatingTest") {
            puntuacionTotal = factor1.Puntuacion;
        } else {
            puntuacionTotal = factor1.Puntuacion + factor2.Puntuacion + factor3.Puntuacion;
        }

        
    }

    // Carga los consejos en la pantalla
    private async Task CargarConsejosAsync(string tipoTest)
    {
        if (tipoTest == "str_FullTest" || tipoTest == "str_QuickTest") {
            using var stream = await FileSystem.OpenAppPackageFileAsync("ConsejosParaAnsiedad.json");
            using var reader = new StreamReader(stream);
            var contenido = await reader.ReadToEndAsync();
            consejosDisponibles = JsonSerializer.Deserialize<List<Advice>>(contenido);
        }
        switch (tipoTest)
        {
            case "str_FullTest":
                AnsiedadCompletoConsejos(consejosDisponibles);
                break;
            case "str_QuickTest":
                AnsiedadRapidoConsejos(consejosDisponibles);
                break;
            case "str_EatingTest":
                TCAConsejos();
                break;
        }
    }

    // Método para mostrar los consejos de Ansiedad Rápido
    private void AnsiedadRapidoConsejos(List<Advice> consejosDisponibles)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            if (consejosDisponibles.Count > 0) {
                // Punto A
                if (puntuacionTotal >= 32 && puntuacionTotal <= 49) {
                    LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                }
                // Punto B
                else if (puntuacionTotal >= 50 && puntuacionTotal <= 80) {
                    LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                }
                // Opción 3
                else // esta sería para todo bien
                {
                    LblIntro.Text = Strings.str_ResultsPage_LblIntro_Low;
                }
                // CASO 1: Para TODO BIEN
                if (factor1.Puntuacion <= 14 && factor3.Puntuacion <= 14 && factor2.Puntuacion <= 1) {
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                }
                // CASO 2: PARA SOLO EL FACTOR COGNITIVO (FACTOR 1)
                else if (factor1.Puntuacion >= 15 && factor3.Puntuacion <= 14 && factor2.Puntuacion <= 1) {
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[0].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[0].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[3].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[3].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_SeeMore, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[17].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[17].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
                }
                // CASO 3: PARA SOLO EL FACTOR FISIOLÓGICO (FACTOR 3)
                else if (factor1.Puntuacion >= 15 && factor3.Puntuacion <= 14 && factor2.Puntuacion <= 1) {
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
                }
                // CASO 4: PARA EL FACTOR DE EVITACIÓN (FACTOR 2)
                else if (factor1.Puntuacion <= 14 && factor3.Puntuacion <= 14 && factor2.Puntuacion >= 2) {
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[18].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[18].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[19].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[19].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
                }
                // CASO 5: PARA LOS FACTORES COGNITIVO, FISIOLÓGICO Y EVITACIÓN (FACTOR 1, 2 Y 3)
                else if (factor1.Puntuacion >= 15 && factor3.Puntuacion >= 15 && factor2.Puntuacion >= 2) {
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[11].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[11].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
                }
                // CASO 6: PARA FACTOR COGNITIVO Y FISIOLÓGICO (FACTORES 1 y 2)
                else if (factor1.Puntuacion >= 15 && factor3.Puntuacion >= 15 && factor2.Puntuacion <= 1) {
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
                }
                // CASO 7: PARA FACTORES COGNITIVO Y EVITACION (FACTORES 1 y 3)
                else if (factor1.Puntuacion >= 15 && factor3.Puntuacion <= 14 && factor2.Puntuacion >= 2) {
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
                }
                // CASO 8: PARA FACTORES FISIOLÓGICO Y EVITACIÓN (FACTORES 2 y 3)
                else if (factor1.Puntuacion <= 14 && factor3.Puntuacion >= 15 && factor2.Puntuacion >= 2) {
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                    Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
                }

                // Estos se muestran siempre excepto para TODO BIEN
                /*if (factor1.Puntuacion > 14 && factor3.Puntuacion > 14 && factor2.Puntuacion > 1)
                {
                    Consejos.Add(new Consejo { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                    Consejos.Add(new Consejo { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                    Consejos.Add(new Consejo { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
                }*/
            }
        });
    }

    // Método para mostratr los consejos de Ansiedad Completo
    private void AnsiedadCompletoConsejos(List<Advice> consejosDisponibles)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            if (factor1.Nivel.Equals("Bajo") && factor2.Nivel.Equals("Bajo") && factor3.Nivel.Equals("Bajo")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Low_2;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Bajo") && factor2.Nivel.Equals("Bajo") && factor3.Nivel.Equals("Medio")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[0].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[0].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[3].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[3].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_SeeMore, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[17].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[17].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Bajo") && factor2.Nivel.Equals("Bajo") && factor3.Nivel.Equals("Alto")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[0].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[0].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[3].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[3].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_SeeMore, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[17].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[17].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Bajo") && factor2.Nivel.Equals("Medio") && factor3.Nivel.Equals("Bajo")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[18].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[18].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[19].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[19].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Bajo") && factor2.Nivel.Equals("Medio") && factor3.Nivel.Equals("Medio")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Bajo") && factor2.Nivel.Equals("Medio") && factor3.Nivel.Equals("Alto")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Bajo") && factor2.Nivel.Equals("Alto") && factor3.Nivel.Equals("Bajo")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[18].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[18].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[19].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[19].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Bajo") && factor2.Nivel.Equals("Alto") && factor3.Nivel.Equals("Medio")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Bajo") && factor2.Nivel.Equals("Alto") && factor3.Nivel.Equals("Alto")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Medio") && factor2.Nivel.Equals("Bajo") && factor3.Nivel.Equals("Bajo")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Medio") && factor2.Nivel.Equals("Bajo") && factor3.Nivel.Equals("Medio")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Medio") && factor2.Nivel.Equals("Bajo") && factor3.Nivel.Equals("Alto")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Medio") && factor2.Nivel.Equals("Medio") && factor3.Nivel.Equals("Bajo")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Medio") && factor2.Nivel.Equals("Medio") && factor3.Nivel.Equals("Medio")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Medio") && factor2.Nivel.Equals("Medio") && factor3.Nivel.Equals("Alto")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Medio") && factor2.Nivel.Equals("Alto") && factor3.Nivel.Equals("Bajo")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Medio") && factor2.Nivel.Equals("Alto") && factor3.Nivel.Equals("Medio")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Medio") && factor2.Nivel.Equals("Alto") && factor3.Nivel.Equals("Alto")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Alto") && factor2.Nivel.Equals("Bajo") && factor3.Nivel.Equals("Bajo")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Alto") && factor2.Nivel.Equals("Bajo") && factor3.Nivel.Equals("Medio")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Alto") && factor2.Nivel.Equals("Bajo") && factor3.Nivel.Equals("Alto")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Alto") && factor2.Nivel.Equals("Medio") && factor3.Nivel.Equals("Bajo")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Alto") && factor2.Nivel.Equals("Medio") && factor3.Nivel.Equals("Medio")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Alto") && factor2.Nivel.Equals("Medio") && factor3.Nivel.Equals("Alto")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Alto") && factor2.Nivel.Equals("Alto") && factor3.Nivel.Equals("Bajo")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Alto") && factor2.Nivel.Equals("Alto") && factor3.Nivel.Equals("Medio")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });

            }
            else if (factor1.Nivel.Equals("Alto") && factor2.Nivel.Equals("Alto") && factor3.Nivel.Equals("Alto")) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_Prevent, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture) });

                Consejos.Add(new Advice { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
            }
        });
    }

    // Método para mostrar los consejos de TCA
    private void TCAConsejos ()
    {
        MainThread.BeginInvokeOnMainThread(() => {
            LblIntro.Text = Strings.str_ResultsPage_LblIntro_EatingTest;
            if (factor1.Nivel.Equals("Bajo")) {
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString("str_EatingAdvice1_Title", CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString("str_EatingAdvice1_Content", CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Alto")) {
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString("str_EatingAdvice2_Title", CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString("str_EatingAdvice2_Content", CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Moderado-Alto")) {
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString("str_EatingAdvice3_Title", CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString("str_EatingAdvice3_Content", CultureInfo.CurrentUICulture) });
            }
            else if (factor1.Nivel.Equals("Leve-Moderado")) {
                Consejos.Add(new Advice { Titulo = Strings.ResourceManager.GetString("str_EatingAdvice4_Title", CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString("str_EatingAdvice4_Content", CultureInfo.CurrentUICulture) });
            }
        });
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var label = (Label)sender;
        var formattedString = label.FormattedText;
        string url = formattedString.Spans[1].Text;

        if (!string.IsNullOrWhiteSpace(url)) {
            try {
                await Launcher.OpenAsync(new Uri(url));
            } catch (Exception ex) {
                Console.WriteLine(string.Format(Strings.str_ResultsPage_LinkError, ex.Message));
                await DisplayAlert(Strings.str_ResultHistoryPage_ImgProfile_Error, Strings.str_ResultsPage_LinkError_Msg, Strings.str_ResultHistoryPage_BtnCheck_OK);
            }
        }
    }

    private async void BtnFinish_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            await Navigation.PopAsync(true);
        } finally {
            App.ButtonPressed = false;
        }
    }
}