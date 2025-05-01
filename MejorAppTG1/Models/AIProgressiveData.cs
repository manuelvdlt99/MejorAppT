using Microsoft.ML.Data;

namespace MejorAppTG1.Models
{
    public class AIProgressiveData
    {
        [LoadColumn(0)] public float EdadRango;
        [LoadColumn(1)] public string Genero;
        [LoadColumn(2)] public float P1;
        [LoadColumn(3)] public float P2;
        [LoadColumn(4)] public float P3;
        [LoadColumn(5), ColumnName("Label")] public float PuntuacionEvolucion;
    }
}
