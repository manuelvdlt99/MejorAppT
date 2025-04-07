using Firebase.Database;
using Firebase.Database.Query;
using MejorAppTG1.Resources.Localization;
using System.Globalization;

namespace MejorAppTG1.Data
{
    public class FirebaseService
    {
        private readonly FirebaseClient _firebase;

        public FirebaseService(string firebaseUrl)
        {
            _firebase = new FirebaseClient(firebaseUrl);
        }

        public async Task<string> AddOrUpdateTest(Test test)
        {
            List<Answer> allAnswers = await App.Database.GetAnswersByTestIdAsync(test.IdTest);
            int factor01 = 0, factor02 = 0, factor03 = 0, factor04 = 0;
            string returnedValue = "";
            foreach (Answer answer in allAnswers) {
                switch (answer.Factor) {
                    case "1":
                        factor01 += answer.ValorRespuesta;
                        break;
                    case "2":
                        factor02 += answer.ValorRespuesta;
                        break;
                    case "3":
                        factor03 += answer.ValorRespuesta;
                        break;
                    case "4":
                        factor04 += answer.ValorRespuesta;
                        break;
                }
            }

            if (test.Tipo == "str_QuickTest") {
                var firebaseTest = new {
                    Genero = Strings.ResourceManager.GetString(test.GeneroUser, new CultureInfo("es")),
                    Edad = test.EdadUser,
                    Fecha = test.Fecha,
                    Factor01 = factor01,
                    Factor02 = factor02,
                    Factor03 = factor03,
                };

                var result = await _firebase
                    .Child("ResultsAnsiedadRapido")
                    .PostAsync(firebaseTest);

                returnedValue = result.Key;
            }
            else if (test.Tipo == "str_FullTest") {
                var firebaseTest = new {
                    Genero = Strings.ResourceManager.GetString(test.GeneroUser, new CultureInfo("es")),
                    Edad = test.EdadUser,
                    Fecha = test.Fecha,
                    Factor01 = factor01,
                    Factor02 = factor02,
                    Factor03 = factor03,
                    Factor04 = factor04
                };

                var result = await _firebase
                    .Child("ResultsAnsiedadCompleto")
                    .PostAsync(firebaseTest);

                returnedValue = result.Key;
            }
            else if (test.Tipo == "str_EatingTest") {
                var firebaseTest = new {
                    Genero = Strings.ResourceManager.GetString(test.GeneroUser, new CultureInfo("es")),
                    Edad = test.EdadUser,
                    Fecha = test.Fecha,
                    Puntuacion = factor01,
                };

                var result = await _firebase
                    .Child("ResultsTCA")
                    .PostAsync(firebaseTest);

                returnedValue = result.Key;
            }
            return returnedValue;
        }
    }
}
