using Firebase.Database;
using Firebase.Database.Query;
using MejorAppTG1.Resources.Localization;
using System.Globalization;

namespace MejorAppTG1.Data
{
    public class FirebaseService
    {
        private readonly FirebaseClient _firebase;

        /// <summary>
        /// Inicializa una instancia de la clase <see cref="FirebaseService"/>.
        /// </summary>
        /// <param name="firebaseUrl">La URL de la base de datos de Firebase.</param>
        public FirebaseService(string firebaseUrl)
        {
            _firebase = new FirebaseClient(firebaseUrl);
        }

        /// <summary>
        /// Añade un test a la base de datos o, si ya existe, lo actualiza.
        /// </summary>
        /// <param name="test">El test a añadir o modificar.</param>
        /// <returns>La clave en Firebase del registro recién añadido o modificado.</returns>
        public async Task<string> AddOrUpdateTest(Test test)
        {
            List<Answer> allAnswers = await App.Database.GetAnswersByTestIdAsync(test.IdTest);
            int factor01 = 0, factor02 = 0, factor03 = 0, factor04 = 0;
            string returnedValue = string.Empty;
            foreach (Answer answer in allAnswers) {
                switch (answer.Factor) {
                    case App.FACTORS_1:
                        factor01 += answer.ValorRespuesta;
                        break;
                    case App.FACTORS_2:
                        factor02 += answer.ValorRespuesta;
                        break;
                    case App.FACTORS_3:
                        factor03 += answer.ValorRespuesta;
                        break;
                    case App.FACTORS_4:
                        factor04 += answer.ValorRespuesta;
                        break;
                }
            }

            if (test.Tipo == App.QUICK_TEST_KEY) {
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
            else if (test.Tipo == App.FULL_TEST_KEY) {
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
            else if (test.Tipo == App.TCA_TEST_KEY) {
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
