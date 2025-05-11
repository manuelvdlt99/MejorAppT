using MejorAppTG1.Models;

namespace MejorAppTG1.Utils
{
    /// <summary>
    /// Clase que alberga todos los métodos relativos al cálculo de puntuaciones y factores para cada tipo de test.
    /// </summary>
    public static class ScoreCalculator
    {
        /// <summary>
        /// Calcula la puntuación obtenida y el nivel asociado de un factor concreto de un test realizado.
        /// </summary>
        /// <param name="respuestas">Las respuestas del test realizado.</param>
        /// <param name="factor">El factor a calcular.</param>
        /// <param name="testRealizado">El test realizado.</param>
        /// <returns>Una instancia de Factor con la puntuación obtenida y el nivel obtenido.</returns>
        public static Factor CalculoFactores(List<Answer> respuestas, string factor, Test testRealizado)
        {
            int totalFactor = SumarPuntos(respuestas, factor, testRealizado);
            string nivel = CalcularNivel(testRealizado, factor, totalFactor);

            return new Factor
            {
                NumeroFactor = factor,
                Puntuacion = totalFactor,
                Nivel = nivel
            };
        }

        /// <summary>
        /// Suma la puntuación obtenida en un factor dado de un test realizado.
        /// </summary>
        /// <param name="respuestas">Las respuestas del test realizado.</param>
        /// <param name="factor">El factor a calcular.</param>
        /// <param name="testRealizado">El test realizado.</param>
        /// <returns>La puntuación total obtenida por el usuario en el factor.</returns>
        private static int SumarPuntos(List<Answer> respuestas, string factor, Test testRealizado)
        {
            int totalFactor = 0;
            if (testRealizado.Tipo == App.TCA_TEST_KEY) {
                for (int i = 0; i < respuestas.Count; i++) {
                    if (respuestas[i].ValorRespuesta > 0) respuestas[i].ValorRespuesta -= 1;

                    if (i == 24) {
                        switch (respuestas[i].ValorRespuesta) {
                            case 0:
                                respuestas[i].ValorRespuesta = 3;
                                break;
                            case 1:
                                respuestas[i].ValorRespuesta = 2;
                                break;
                            case 2:
                                respuestas[i].ValorRespuesta = 1;
                                break;
                            case 3:
                            case 4:
                                respuestas[i].ValorRespuesta = 0;
                                break;
                        }
                    }
                    totalFactor += respuestas[i].ValorRespuesta;
                }
            }
            else {
                for (int i = 0; i < respuestas.Count; i++) {
                    if (factor == respuestas[i].Factor) {
                        totalFactor += respuestas[i].ValorRespuesta;
                    }
                }
            }

            return totalFactor;
        }

        /// <summary>
        /// Obtiene el nivel equivalente a la puntuación obtenida en un factor concreto de un test realizado.
        /// </summary>
        /// <param name="testRealizado">El test realizado.</param>
        /// <param name="factor">El factor a calcular.</param>
        /// <param name="totalFactor">La puntuación total obtenida en el factor.</param>
        /// <returns>El nivel obtenido.</returns>
        private static string CalcularNivel(Test testRealizado, string factor, int totalFactor)
        {
            string nivel = string.Empty;
            switch (testRealizado.Tipo) {
                case App.QUICK_TEST_KEY:
                    nivel = NivelesRapido(totalFactor, factor);
                    break;
                case App.FULL_TEST_KEY:
                    nivel = NivelesCompleto(totalFactor, factor, testRealizado);
                    break;
                case App.TCA_TEST_KEY:
                    nivel = NivelesTCA(totalFactor);
                    break;
            }
            return nivel;
        }

        /// <summary>
        /// Calcula el nivel para un test de ansiedad rápido.
        /// </summary>
        /// <param name="totalFactor">La puntuación total obtenida en el factor.</param>
        /// <param name="factor">El factor a calcular.</param>
        /// <returns>El nivel obtenido.</returns>
        private static string NivelesRapido(int totalFactor, string factor)
        {
            string nivel = App.FACTORS_LEVEL_LOW;
            switch (factor) {
                case App.FACTORS_1:
                    if (totalFactor >= 15 && totalFactor <= 23) nivel = App.FACTORS_LEVEL_MEDIUM;
                    else if (totalFactor > 23) nivel = App.FACTORS_LEVEL_HIGH;
                    break;
                case App.FACTORS_2:
                    if (totalFactor == 2 || totalFactor == 3) nivel = App.FACTORS_LEVEL_MEDIUM;
                    else if (totalFactor > 3) nivel = App.FACTORS_LEVEL_HIGH;
                    break;
                case App.FACTORS_3:
                    if (totalFactor >= 15 && totalFactor <= 21) nivel = App.FACTORS_LEVEL_MEDIUM;
                    else if (totalFactor > 21) nivel = App.FACTORS_LEVEL_HIGH;
                    break;
            }
            return nivel;
        }

        /// <summary>
        /// Calcula el nivel para un test de ansiedad completo.
        /// </summary>
        /// <param name="totalFactor">La puntuación total obtenida en el factor.</param>
        /// <param name="factor">El factor a calcular.</param>
        /// <returns>El nivel obtenido.</returns>
        private static string NivelesCompleto(int totalFactor, string factor, Test testRealizado)
        {
            string nivel = App.FACTORS_LEVEL_LOW;
            string genero = testRealizado.GeneroUser;
            int edad = testRealizado.EdadUser;

            #region MUJERES
            if (genero == App.GENDERS_FEMALE_KEY) {
                if (edad <= 14) {
                    switch (factor) {
                        case App.FACTORS_1:
                            if (totalFactor > 16 && totalFactor <= 25) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 25) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_2:
                            if (totalFactor > 0) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_3:
                            if (totalFactor > 26 && totalFactor <= 34.1) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 34.1) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                    }
                }
                else if (edad == 15 || edad == 16) {
                    switch (factor) {
                        case App.FACTORS_1:
                            if (totalFactor > 22 && totalFactor <= 27) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 27) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_2:
                            if (totalFactor == 1) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 1) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_3:
                            if (totalFactor > 28 && totalFactor <= 36) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 36) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                    }
                }
                else if (edad >= 17) {
                    switch (factor) {
                        case App.FACTORS_1:
                            if (totalFactor > 23.5 && totalFactor <= 32) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 32) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_2:
                            if (totalFactor == 1) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 1) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_3:
                            if (totalFactor > 30 && totalFactor <= 35) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 35) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                    }
                }
            }
            #endregion

            #region HOMBRES
            else if (genero == App.GENDERS_MALE_KEY) {
                if (edad <= 14) {
                    switch (factor) {
                        case App.FACTORS_1:
                            if (totalFactor > 11 && totalFactor <= 17) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 17) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_2:
                            if (totalFactor > 0) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_3:
                            if (totalFactor > 19 && totalFactor <= 28) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 28) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                    }
                }
                else if (edad == 15 || edad == 16) {
                    switch (factor) {
                        case App.FACTORS_1:
                            if (totalFactor > 12 && totalFactor <= 18) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 18) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_2:
                            if (totalFactor == 1) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 1) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_3:
                            if (totalFactor > 19 && totalFactor <= 26) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 26) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                    }
                }
                else if (edad >= 17) {
                    switch (factor) {
                        case App.FACTORS_1:
                            if (totalFactor > 12 && totalFactor <= 18.4) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 18.4) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_2:
                            if (totalFactor <= 2 && totalFactor != 0) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 2) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_3:
                            if (totalFactor > 21 && totalFactor <= 26) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 26) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                    }
                }
            }
            #endregion

            #region NO BINARIOS
            else if (genero == App.GENDERS_NB_KEY) {
                if (edad <= 14) {
                    switch (factor) {
                        case App.FACTORS_1:
                            if (totalFactor > 13.5 && totalFactor <= 29.5) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 29.5) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_2:
                            if (totalFactor > 0) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_3:
                            if (totalFactor > 22.5 && totalFactor <= 27) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 27) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                    }
                }
                else if (edad == 15 || edad == 16) {
                    switch (factor) {
                        case App.FACTORS_1:
                            if (totalFactor > 20.4 && totalFactor <= 32.4) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 32.4) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_2:
                            if (totalFactor <= 2 && totalFactor != 0) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 2) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_3:
                            if (totalFactor > 23.5 && totalFactor <= 31) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 31) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                    }
                }
                else if (edad >= 17) {
                    switch (factor) {
                        case App.FACTORS_1:
                            if (totalFactor > 21.3 && totalFactor <= 30.2) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 30.2) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_2:
                            if (totalFactor == 1) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 1) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                        case App.FACTORS_3:
                            if (totalFactor > 25.5 && totalFactor <= 30.5) nivel = App.FACTORS_LEVEL_MEDIUM;
                            else if (totalFactor > 30.5) nivel = App.FACTORS_LEVEL_HIGH;
                            break;
                    }
                }
            }
            #endregion
            return nivel;
        }

        /// <summary>
        /// Calcula el nivel para un test de TCA.
        /// </summary>
        /// <param name="totalFactores">La puntuación total obtenida.</param>
        /// <returns>El nivel obtenido.</returns>
        private static string NivelesTCA(int totalFactores)
        {
            string nivel = App.FACTORS_LEVEL_LOW;
            if (totalFactores >= 20) nivel = App.FACTORS_LEVEL_HIGH;
            else if (totalFactores >= 15) nivel = App.FACTORS_LEVEL_MEDIUM_HIGH;
            else if (totalFactores >= 11) nivel = App.FACTORS_LEVEL_LOW_MEDIUM;
            return nivel;
        }
    }
}
