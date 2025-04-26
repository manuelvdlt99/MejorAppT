using Microsoft.ML.Data;

namespace MejorAppTG1.AI_Models
{
    /// <summary>
    /// Clase que utiliza el modelo de IA para devolver el dato que ha predicho.
    /// </summary>
    public class AIPrediction
    {
        [ColumnName("Score")]
        public float PredictedResult;
    }
}
