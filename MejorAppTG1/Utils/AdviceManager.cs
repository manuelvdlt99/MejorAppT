using MejorAppTG1.Models;
using System.Text.Json;
using MejorAppTG1.Resources.Localization;
using System.Globalization;
using System.Collections.ObjectModel;

namespace MejorAppTG1.Utils
{
    /// <summary>
    /// Clase que alberga todos los métodos relativos a la recuperación de consejos y su organización para pasárselos al formulario correspondiente.
    /// </summary>
    public static class AdviceManager
    {
        /// <summary>
        /// Calcula la puntuación total de un test realizado.
        /// </summary>
        /// <param name="factor1">El factor 1 del test realizado.</param>
        /// <param name="factor2">El factor 2 del test realizado.</param>
        /// <param name="factor3">El factor 3 del test realizado.</param>
        /// <param name="tipoTest">El tipo del test realizado.</param>
        /// <returns>La puntuación total del test realizado.</returns>
        public static int CalcularPuntuacionTotal(Factor factor1, Factor factor2, Factor factor3, string tipoTest)
        {
            int puntuacionTotal;
            if (tipoTest == App.TCA_TEST_KEY) {
                puntuacionTotal = factor1.Puntuacion;
            }
            else {
                puntuacionTotal = factor1.Puntuacion + factor2.Puntuacion + factor3.Puntuacion;
            }
            return puntuacionTotal;
        }

        /// <summary>
        /// Recupera los consejos del JSON, los procesa dependiendo del tipo de test realizado y los devuelve.
        /// </summary>
        /// <param name="tipoTest">El tipo del test realizado.</param>
        /// <param name="puntuacionTotal">La puntuación obtenida en todo el test.</param>
        /// <param name="factor1">El factor 1 del test realizado.</param>
        /// <param name="factor2">El factor 2 del test realizado.</param>
        /// <param name="factor3">El factor 3 del test realizado.</param>
        /// <returns>La lista de categorías de consejos con todos los datos pertinentes.</returns>
        public static async Task<List<AdviceCategory>> CargarCategoriasAsync(string tipoTest, int puntuacionTotal, Factor factor1, Factor factor2, Factor factor3)
        {
            List<Advice> consejosDisponibles = new();
            if (tipoTest == App.FULL_TEST_KEY || tipoTest == App.QUICK_TEST_KEY) {
                using var stream = await FileSystem.OpenAppPackageFileAsync(App.JSON_ADVICES_FULL);
                using var reader = new StreamReader(stream);
                var contenido = await reader.ReadToEndAsync();
                consejosDisponibles = JsonSerializer.Deserialize<List<Advice>>(contenido);
            }
            List<AdviceCategory> categories = new();

            switch (tipoTest) {
                case App.FULL_TEST_KEY:
                    categories = AnsiedadCompletoConsejos(consejosDisponibles, puntuacionTotal, factor1, factor2, factor3);
                    break;
                case App.QUICK_TEST_KEY:
                    categories = AnsiedadRapidoConsejos(consejosDisponibles, puntuacionTotal, factor1, factor2, factor3);
                    break;
                case App.TCA_TEST_KEY:
                    categories = TCAConsejos(factor1);
                    break;
            }
            return categories;
        }

        /// <summary>
        /// Devuelve un texto introductorio para la página de recomendaciones dependiendo del tipo de test, los factores y/o la puntuación total.
        /// </summary>
        /// <param name="tipoTest">El tipo del test realizado.</param>
        /// <param name="puntuacionTotal">La puntuación obtenida en todo el test.</param>
        /// <param name="factor1">El factor 1 del test realizado.</param>
        /// <param name="factor2">El factor 2 del test realizado.</param>
        /// <param name="factor3">El factor 3 del test realizado.</param>
        /// <returns>El texto introductorio personalizado.</returns>
        public static string CargarLblIntro(string tipoTest, int puntuacionTotal, Factor factor1, Factor factor2, Factor factor3)
        {
            string text = string.Empty;
            switch (tipoTest) {
                case App.FULL_TEST_KEY:
                    if (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                        text = Strings.str_ResultsPage_LblIntro_Low_2;
                    }
                    else if ((factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM))
                        || (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW))
                        || (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM))
                        || (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW))
                        || (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM))
                        || (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW))
                        || (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM))) {
                        text = Strings.str_ResultsPage_LblIntro_Medium;
                    }
                    else {
                        text = Strings.str_ResultsPage_LblIntro_High;
                    }
                    break;
                case App.QUICK_TEST_KEY:
                    if (puntuacionTotal >= 32 && puntuacionTotal <= 49) {
                        text = Strings.str_ResultsPage_LblIntro_Medium;
                    }
                    else if (puntuacionTotal >= 50 && puntuacionTotal <= 80) {
                        text = Strings.str_ResultsPage_LblIntro_High;
                    }
                    else {
                        text = Strings.str_ResultsPage_LblIntro_Low;
                    }
                    break;
                case App.TCA_TEST_KEY:
                    text = Strings.str_ResultsPage_LblIntro_EatingTest;
                    break;
            }
            return text;
        }

        /// <summary>
        /// Calcula y crea las categorías con los consejos asociados según los resultados del test de ansiedad rápido realizado.
        /// </summary>
        /// <param name="consejosDisponibles">La lista con todos los consejos disponibles.</param>
        /// <param name="factor1">El factor 1 del test realizado.</param>
        /// <param name="factor2">El factor 2 del test realizado.</param>
        /// <param name="factor3">El factor 3 del test realizado.</param>
        /// <param name="puntuacionTotal">La puntuación obtenida en todo el test.</param>
        /// <returns>La lista de categorías de consejos con todos los datos pertinentes.</returns>
        public static List<AdviceCategory> AnsiedadRapidoConsejos(List<Advice> consejosDisponibles, int puntuacionTotal, Factor factor1, Factor factor2, Factor factor3)
        {
            List<AdviceCategory> consejos = new();
            if (consejosDisponibles.Count > 0) {
                #region CASO 1: PARA TODO BIEN
                if (factor1.Puntuacion <= 14 && factor3.Puntuacion <= 14 && factor2.Puntuacion <= 1) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "thumbsup_icon.png", Name = Strings.str_ResultsPage_Category_KeepItUp, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    consejos.Add(category);
                }
                #endregion
                #region CASO 2: PARA SOLO EL FACTOR COGNITIVO (FACTOR 3)
                else if (factor3.Puntuacion >= 15 && factor1.Puntuacion <= 14 && factor2.Puntuacion <= 1) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 1, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[0].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[0].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[0].Imagen });
                    category.Advices.Add(new Advice { Id = 2, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                    category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                    category.Advices.Add(new Advice { Id = 4, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[3].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[3].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[3].Imagen });
                    category.Advices.Add(new Advice { Id = 5, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "seemore_icon.png", Name = Strings.str_ResultsPage_Category_SeeMore, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 18, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[17].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[17].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[17].Imagen });
                    category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                    consejos.Add(category);
                }
                #endregion
                #region CASO 3: PARA SOLO EL FACTOR FISIOLÓGICO (FACTOR 1)
                else if (factor1.Puntuacion >= 15 && factor3.Puntuacion <= 14 && factor2.Puntuacion <= 1) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                    category.Advices.Add(new Advice { Id = 7, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                    category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                    category.Advices.Add(new Advice { Id = 9, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                    category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                    consejos.Add(category);
                }
                #endregion
                #region CASO 4: PARA EL FACTOR DE EVITACIÓN (FACTOR 2)
                else if (factor1.Puntuacion <= 14 && factor3.Puntuacion <= 14 && factor2.Puntuacion >= 2) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                    category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                    category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 19, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[18].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[18].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[18].Imagen });
                    category.Advices.Add(new Advice { Id = 20, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[19].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[19].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[19].Imagen });
                    category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                    consejos.Add(category);
                }
                #endregion
                #region CASO 5: PARA LOS FACTORES COGNITIVO, FISIOLÓGICO Y EVITACIÓN (FACTOR 1, 2 Y 3)
                else if (factor1.Puntuacion >= 15 && factor3.Puntuacion >= 15 && factor2.Puntuacion >= 2) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                    category.Advices.Add(new Advice { Id = 12, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[11].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[11].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[11].Imagen });
                    category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                    category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                    category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                    consejos.Add(category);
                }
                #endregion
                #region CASO 6: PARA FACTOR COGNITIVO Y FISIOLÓGICO (FACTORES 1 y 3)
                else if (factor1.Puntuacion >= 15 && factor3.Puntuacion >= 15 && factor2.Puntuacion <= 1) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                    category.Advices.Add(new Advice { Id = 2, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                    category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                    category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                    category.Advices.Add(new Advice { Id = 9, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                    consejos.Add(category);
                }
                #endregion
                #region CASO 7: PARA FACTORES COGNITIVO Y EVITACION (FACTORES 3 y 2)
                else if (factor3.Puntuacion >= 15 && factor1.Puntuacion <= 14 && factor2.Puntuacion >= 2) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                    category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                    category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                    category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    category.Advices.Add(new Advice { Id = 5, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                    consejos.Add(category);
                }
                #endregion
                #region CASO 8: PARA FACTORES FISIOLÓGICO Y EVITACIÓN (FACTORES 1 y 2)
                else if (factor3.Puntuacion <= 14 && factor1.Puntuacion >= 15 && factor2.Puntuacion >= 2) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                    category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                    category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                    category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    consejos.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                    consejos.Add(category);
                }
                #endregion
            }
            return consejos;
        }

        /// <summary>
        /// Calcula y crea las categorías con los consejos asociados según los resultados del test de ansiedad completo realizado.
        /// </summary>
        /// <param name="consejosDisponibles">La lista con todos los consejos disponibles.</param>
        /// <param name="factor1">El factor 1 del test realizado.</param>
        /// <param name="factor2">El factor 2 del test realizado.</param>
        /// <param name="factor3">El factor 3 del test realizado.</param>
        /// <param name="puntuacionTotal">La puntuación obtenida en todo el test.</param>
        /// <returns>La lista de categorías de consejos con todos los datos pertinentes.</returns>
        public static List<AdviceCategory> AnsiedadCompletoConsejos(List<Advice> consejosDisponibles, int puntuacionTotal, Factor factor1, Factor factor2, Factor factor3)
        {
            List<AdviceCategory> consejos = new();
            #region BAJO-BAJO-BAJO
            if (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                ////LblIntro.Text = Strings.str_ResultsPage_LblIntro_Low_2;
                AdviceCategory category = new AdviceCategory { ImagePath = "thumbsup_icon.png", Name = Strings.str_ResultsPage_Category_KeepItUp, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region BAJO-BAJO-MEDIO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 1, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[0].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[0].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[0].Imagen });
                category.Advices.Add(new Advice { Id = 2, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Id = 4, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[3].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[3].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[3].Imagen });
                category.Advices.Add(new Advice { Id = 5, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "seemore_icon.png", Name = Strings.str_ResultsPage_Category_SeeMore, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 18, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[17].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[17].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[17].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region BAJO-BAJO-ALTO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 1, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[0].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[0].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[0].Imagen });
                category.Advices.Add(new Advice { Id = 2, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Id = 4, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[3].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[3].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[3].Imagen });
                category.Advices.Add(new Advice { Id = 5, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "seemore_icon.png", Name = Strings.str_ResultsPage_Category_SeeMore, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 18, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[17].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[17].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[17].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region BAJO-MEDIO-BAJO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 19, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[18].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[18].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[18].Imagen });
                category.Advices.Add(new Advice { Id = 20, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[19].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[19].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[19].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region BAJO-MEDIO-MEDIO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Id = 5, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region BAJO-MEDIO-ALTO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Id = 5, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region BAJO-ALTO-BAJO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 19, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[18].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[18].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[18].Imagen });
                category.Advices.Add(new Advice { Id = 20, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[19].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[19].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[19].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region BAJO-ALTO-MEDIO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Id = 5, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region BAJO-ALTO-ALTO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Id = 5, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region MEDIO-BAJO-BAJO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 7, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 9, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region MEDIO-BAJO-MEDIO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 2, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 9, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region MEDIO-BAJO-ALTO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 2, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 9, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region MEDIO-MEDIO-BAJO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region MEDIO-MEDIO-MEDIO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 12, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[11].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region MEDIO-MEDIO-ALTO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 12, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[11].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region MEDIO-ALTO-BAJO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region MEDIO-ALTO-MEDIO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 12, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[11].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region MEDIO-ALTO-ALTO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 12, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[11].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region ALTO-BAJO-BAJO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 7, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 9, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region ALTO-BAJO-MEDIO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 2, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 9, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region ALTO-BAJO-ALTO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 2, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                category.Advices.Add(new Advice { Id = 3, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 9, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region ALTO-MEDIO-BAJO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region ALTO-MEDIO-MEDIO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 12, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[11].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region ALTO-MEDIO-ALTO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 12, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[11].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region ALTO-ALTO-BAJO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region ALTO-ALTO-MEDIO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 12, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[11].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            #region ALTO-ALTO-ALTO
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                //LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 6, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Id = 12, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[11].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Id = 8, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Id = 11, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Id = 10, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 13, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Id = 14, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Id = 15, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Id = 21, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Id = 22, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Id = 23, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                consejos.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Id = 16, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Id = 17, Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                consejos.Add(category);
            }
            #endregion
            return consejos;
        }

        /// <summary>
        /// Calcula y crea las categorías con los consejos asociados según los resultados del test de TCA realizado.
        /// </summary>
        /// <param name="factor1">El factor 1 del test realizado.</param>
        /// <returns>La lista de categorías de consejos con todos los datos pertinentes.</returns>
        public static List<AdviceCategory> TCAConsejos(Factor factor1)
        {
            List<AdviceCategory> consejos = new();
            AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
            if (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                category.Advices.Add(new Advice { Id = 1, Titulo = Strings.ResourceManager.GetString("str_EatingAdvice1_Title", CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString("str_EatingAdvice1_Content", CultureInfo.CurrentUICulture), Imagen = "eatingadvice.png" });
            }
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                category.Advices.Add(new Advice { Id = 1, Titulo = Strings.ResourceManager.GetString("str_EatingAdvice2_Title", CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString("str_EatingAdvice2_Content", CultureInfo.CurrentUICulture), Imagen = "eatingadvice.png" });
            }
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM_HIGH)) {
                category.Advices.Add(new Advice { Id = 1, Titulo = Strings.ResourceManager.GetString("str_EatingAdvice3_Title", CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString("str_EatingAdvice3_Content", CultureInfo.CurrentUICulture), Imagen = "eatingadvice.png" });
            }
            else if (factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW_MEDIUM)) {
                category.Advices.Add(new Advice { Id = 1, Titulo = Strings.ResourceManager.GetString("str_EatingAdvice4_Title", CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString("str_EatingAdvice4_Content", CultureInfo.CurrentUICulture), Imagen = "eatingadvice.png" });

            }
            consejos.Add(category);
            return consejos;
        }
    }
}
