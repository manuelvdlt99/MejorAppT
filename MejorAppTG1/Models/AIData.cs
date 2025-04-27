using Microsoft.ML.Data;

namespace MejorAppTG1.Models
{
    /// <summary>
    /// Clase que utiliza el modelo de IA como entrada de datos
    /// </summary>
    public class AIData
    {
        [LoadColumn(0)] public float EdadRango;
        [LoadColumn(1)] public string Genero;
        [LoadColumn(2), ColumnName("Label")] public float Puntuacion;
    }
}
