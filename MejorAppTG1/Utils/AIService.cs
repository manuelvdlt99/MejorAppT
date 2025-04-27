using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using Microsoft.ML;

namespace MejorAppTG1.Utils
{
    /// <summary>
    /// Clase que alberga todos los métodos relativos al uso de modelos de inteligencia artificial.
    /// </summary>
    internal class AIService
    {
        /// <summary>
        /// Utilizando un modelo entrenado, predice el resultado medio que podría obtener un usuario en un test según su género y edad.
        /// </summary>
        /// <param name="mlContext">El contexto del modelo de IA.</param>
        /// <param name="modelPath">La ruta donde está el zip del modelo entrenado.</param>
        /// <param name="user">La instancia que contiene los datos del usuario a analizar.</param>
        public static float GetAIPredictedAvgResult(MLContext mlContext, string modelPath, AIData user)
        {
            ITransformer trainedModel = mlContext.Model.Load(modelPath, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<AIData, AIPrediction>(trainedModel);
            var prediction = predEngine.Predict(user);
            return prediction.PredictedResult;
        }

        /// <summary>
        /// Interpreta la diferencia entre el resultado predicho por la IA y el resultado real y devuelve una valoración.
        /// </summary>
        /// <param name="predicted">El resultado predicho por la IA.</param>
        /// <param name="real">El resultado real.</param>
        /// <returns>La valoración. Si el resultado real es null, devuelve un string vacío.</returns>
        public static string InterpretPrediction(float predicted, float? real)
        {
            if (real.HasValue) {
                float difference = real.Value - predicted;
                float margin = predicted * 0.15f;

                if (Math.Abs(difference) <= margin)
                    return Strings.str_AIPrediction_General_1;
                else if (difference > margin && difference <= 2 * margin)
                    return Strings.str_AIPrediction_General_2;
                else if (difference > 2 * margin)
                    return Strings.str_AIPrediction_General_3;
                else if (difference < -margin && difference >= -2 * margin)
                    return Strings.str_AIPrediction_General_4;
                else
                    return Strings.str_AIPrediction_General_5;

            }
            else {
                return string.Empty;
            }
        }
    }
}
