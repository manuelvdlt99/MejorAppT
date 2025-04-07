using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using MejorAppTG1.Utils;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace MejorAppTG1;

public class TranslationKeyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return null;

        // Se espera que `value` sea una clave de recurso como "str_AnsiedadRapido"
        string key = value.ToString();

        // Buscar la traducción en el archivo de recursos
        string translation = Strings.ResourceManager.GetString(key, CultureInfo.CurrentUICulture);

        // Si no se encuentra la clave, devolver la clave como fallback
        return translation ?? key;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
public partial class ResultHistoryPage : ContentPage
{
    private static int resultIndex = 0;
    private static int currentPage = 1;
    private List<Test> finishedTests;
    private List<Test> fiveTests = new List<Test>();

    public static int ResultIndex { get => resultIndex; set => resultIndex = value; }
    public static int CurrentPage { get => currentPage; set => currentPage = value; }

    public ResultHistoryPage()
    {
        InitializeComponent();
    }

    private async void OnPageAppearing(object sender, EventArgs e)
    {
        try {
            StkLoading.IsVisible = true;
            GrdData.IsVisible = false;
            await Task.Delay(800);  // Forzar a que se espere un poco antes de cargar, porque de lo contrario el indicador de carga ni siquiera aparece

            await Task.WhenAll(
                Task.Run(() => UpdateUserImage()),
                Task.Run(() => UpdateUserLabels()),
                LoadResults()
            );
        }
        finally {
            StkLoading.IsVisible = false;
            GrdData.IsVisible = true;
            string translatedAgeFormat = Strings.str_ResultHistoryPage_LblAge_Dyn;
            string translatedAge = string.Format(translatedAgeFormat, App.CurrentUser.Edad);
            string semanticDescription = string.Format(Strings.str_SemanticProperties_LoginPage_UsersPopup_SelectedUser, App.CurrentUser.Nombre, translatedAge, Strings.ResourceManager.GetString(App.CurrentUser.Genero, CultureInfo.CurrentUICulture));
            SemanticScreenReader.Announce(semanticDescription);
        }
    }

    internal void UpdateUserLabels()
    {
        MainThread.BeginInvokeOnMainThread(() => {
            LblUsername.Text = App.CurrentUser.Nombre;
            LblUserAge.Text = string.Format(Strings.str_ResultHistoryPage_LblAge_Dyn, App.CurrentUser.Edad.ToString());
            LblUserGender.Text = Strings.ResourceManager.GetString(App.CurrentUser.Genero, CultureInfo.CurrentUICulture);
            if (App.CurrentUser.Genero == "str_Genders_Man") {
                LblUserGenderIcon.Text = "♂";
            }
            else if (App.CurrentUser.Genero == "str_Genders_Woman") {
                LblUserGenderIcon.Text = "♀";
            }
            else if (App.CurrentUser.Genero == "str_Genders_NB") {
                LblUserGenderIcon.Text = "🜬";
            }
        });
    }

    internal void UpdateUserImage()
    {
        string image = App.CurrentUser.Imagen;
        if (image != null) {
            MainThread.BeginInvokeOnMainThread(() => {
                ImgProfile.Source = ImageSource.FromFile(image);
            });
        }
    }

    private async Task LoadNextFiveTests()
    {
        MainThread.BeginInvokeOnMainThread(() => {
            CtvResults.ItemsSource = null;
            fiveTests.Clear();

            // No puede haber una página -1, evidentemente
            if (resultIndex == 0) {
                BtnPreviousFive.IsEnabled = false;
                BtnPreviousFive.BorderColor = (Color)Application.Current.Resources["BorderColor3"];
            }
            else {
                BtnPreviousFive.IsEnabled = true;
                BtnPreviousFive.BorderColor = (Color)Application.Current.Resources["SecondaryColor1"];
            }

            for (int i = resultIndex; i < resultIndex + 5; i++) {
                if (i < finishedTests.Count) {
                    Test currentTest = finishedTests[i];
                    if (currentTest != null) {
                        fiveTests.Add(currentTest);
                    }
                    // Si i se pasa del último índice de la lista entera en el bucle, es que no hay más entradas. Deshabilita página siguiente y sal del bucle
                }
                else {
                    BtnNextFive.IsEnabled = false;
                    BtnNextFive.BorderColor = (Color)Application.Current.Resources["BorderColor3"];
                    break;
                }

                // Si por ejemplo solo hay 5 entradas, va a mostrar esas 5 siempre, pero no se pueden comprobar en el bucle las siguientes 5
                // Hay que comprobar de antemano si podría haber más entradas en una nueva página o no
                //Ej: i se ha quedado en [4] de 5 entradas totales (la última). Hay que comprobar si i es, efectivamente, el último índice de la lista entera
                if (i >= finishedTests.Count - 1) {
                    BtnNextFive.IsEnabled = false;
                    BtnNextFive.BorderColor = (Color)Application.Current.Resources["BorderColor3"];
                }
                else {
                    BtnNextFive.IsEnabled = true;
                    BtnNextFive.BorderColor = (Color)Application.Current.Resources["SecondaryColor1"];
                }
            }
            CtvResults.ItemsSource = fiveTests;
            LblCurrentPage.Text = string.Format(Strings.str_ResultHistoryPage_LblCurrentPage_Dyn, currentPage, (int)Math.Ceiling(finishedTests.Count / 5.0));
        });
    }

    private async Task LoadResults()
    {
        finishedTests = await App.Database.GetFinishedTestsByUserAsync(App.CurrentUser.IdUsuario);

        MainThread.BeginInvokeOnMainThread(async () => {
            if (finishedTests == null || finishedTests.Count == 0) {
                CtvResults.IsVisible = false;
                HslNavigationBtns.IsVisible = false;
                VslNoResults.IsVisible = true;
                await VslNoResults.ScaleTo(1.05, 300, Easing.BounceOut);
                await VslNoResults.ScaleTo(1, 300, Easing.BounceIn);
            }
            else {
                VslNoResults.IsVisible = false;
                HslNavigationBtns.IsVisible = true;
                CtvResults.IsVisible = true;
                await LoadNextFiveTests();
            }
        });
    }

    private async void BtnClearHistory_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            bool answer = await DisplayAlert(Strings.str_ResultHistoryPage_BtnClearHistory_Question, Strings.str_ResultHistoryPage_BtnClearHistory_Msg, Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);

            if (answer) {
                await App.Database.DeleteTestsByUserAsync(App.CurrentUser.IdUsuario);
                await LoadResults();
            }
        } finally {
            App.ButtonPressed = false;
        }
    }

    private async void OnFrameTapped(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            App.AnimateFrameInOut(sender);

            var action = await DisplayActionSheet(Strings.str_ResultHistoryPage_ChooseOption_Question, Strings.str_ResultHistoryPage_BtnCancel, null, Strings.str_ResultHistoryPage_BtnCheck, Strings.str_ResultHistoryPage_BtnDelete);

            if (action == Strings.str_ResultHistoryPage_BtnCheck) {
                ConsultarRegistro(sender);
            }
            else if (action == Strings.str_ResultHistoryPage_BtnDelete) {
                EliminarRegistro(sender);
            }
        } finally {
            App.ButtonPressed = false;
        }
    }

    private async void ConsultarRegistro(object sender)
    {
        Test selectedTest = (Test)((Frame)sender).BindingContext;
        CalculateFactorsAndShowResults(selectedTest);
    }

    private async Task CalculateFactorsAndShowResults(Test test)
    {
        List<Answer> answers = await App.Database.GetAnswersByTestIdAsync(test.IdTest);
        var resultados = await Task.WhenAll(
            Task.Run(() => ScoreCalculator.CalculoFactores(answers, "1", test)),
            Task.Run(() => ScoreCalculator.CalculoFactores(answers, "2", test)),
            Task.Run(() => ScoreCalculator.CalculoFactores(answers, "3", test))
        );

        Factor factor1 = resultados[0];
        Factor factor2 = resultados[1];
        Factor factor3 = resultados[2];

        await Navigation.PushAsync(new ResultsPage(factor1, factor2, factor3, test.Tipo), true);
    }

    private async void EliminarRegistro(object sender)
    {
        bool answer = await DisplayAlert(Strings.str_ResultHistoryPage_BtnDelete_Question, Strings.str_ResultHistoryPage_BtnDelete_Msg, Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);
        
        if (answer) {
            Test selectedTest = (Test)((Frame)sender).BindingContext;

            await App.Database.DeleteTestAndAnswersAsync(selectedTest);
            await LoadResults();

            // Si al eliminar este registro se queda la página vacía, vuelve una página para atrás
            if (resultIndex >= finishedTests.Count) {
                BtnPreviousFive_Clicked(null, null);
            }
        }
    }

    private async void BtnLastResult_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            if (finishedTests == null || finishedTests.Count == 0) {
                await DisplayAlert(Strings.str_ResultHistoryPage_BtnCheck_NoResults, Strings.str_ResultHistoryPage_BtnCheck_Msg, Strings.str_ResultHistoryPage_BtnCheck_OK);
            }
            else {
                await CalculateFactorsAndShowResults(finishedTests[0]);
            }
        } finally {
            App.ButtonPressed = false;
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            await ImgEditProfile.ScaleTo(1.2, 50);
            await ImgEditProfile.ScaleTo(1, 50);
            var status = await Permissions.RequestAsync<Permissions.StorageRead>();

            if (status == PermissionStatus.Granted) {
                if (MediaPicker.Default.IsCaptureSupported) {
                    FileResult photo = await MediaPicker.Default.PickPhotoAsync();

                    if (photo != null) {
                        string localFilePath = GetUniqueFilePath(FileSystem.CacheDirectory, photo.FileName);
                        App.CurrentUser.Imagen = localFilePath;

                        using Stream sourceStream = await photo.OpenReadAsync();
                        using FileStream localFileStream = File.OpenWrite(localFilePath);
                        await sourceStream.CopyToAsync(localFileStream);

                        await App.Database.UpdateUsuarioAsync(App.CurrentUser);
                        UpdateUserImage();
                    }
                }
            }
            else {
                var toast = Toast.Make(Strings.str_ResultHistoryPage_ImgProfile_NoPermission, ToastDuration.Short, 14);
                await toast.Show(new CancellationTokenSource().Token);
            }
        }
        catch (Exception ex) {
            Console.WriteLine(string.Format(Strings.str_ResultHistoryPage_ImgProfile_ErrorMsg, ex.Message));
            await DisplayAlert(Strings.str_ResultHistoryPage_ImgProfile_Error, Strings.str_ResultHistoryPage_ImgProfile_ErrorMsg2, Strings.str_ResultHistoryPage_BtnCheck_OK);
        } finally {
            App.ButtonPressed = false;
        }
    }

    // Método para evitar que se sobrescriban las imágenes si se llaman igual (al igual que Windows añade (1), (2)...)
    private string GetUniqueFilePath(string directory, string fileName)
    {
        string filePath = Path.Combine(directory, fileName);
        string fileExtension = Path.GetExtension(fileName);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        int counter = 1;

        // Incrementa el sufijo hasta encontrar un nombre que no exista
        while (File.Exists(filePath)) {
            string newFileName = $"{fileNameWithoutExtension} ({counter}){fileExtension}";
            filePath = Path.Combine(directory, newFileName);
            counter++;
        }

        return filePath;
    }

    private async void BtnPreviousFive_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            resultIndex -= 5;
            currentPage--;
            await LoadNextFiveTests();
            await SlideCollectionView("Left");
        } finally {
            App.ButtonPressed = false;
        }
    }

    private async void BtnNextFive_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            resultIndex += 5;
            currentPage++;
            await LoadNextFiveTests();
            await SlideCollectionView("Right");
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    private async Task SlideCollectionView(string direction)
    {
        var offset = direction == "Left" ? -100 : 100;
        CtvResults.TranslationX = offset;
        await CtvResults.TranslateTo(0, 0, 300, Easing.CubicOut);
    }
}