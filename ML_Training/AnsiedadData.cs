using Microsoft.ML.Data;

namespace ML_Training
{
    public class AnsiedadData
    {
        [LoadColumn(0)] public float Edad;
        [LoadColumn(1)] public float Factor01;
        [LoadColumn(2)] public float Factor02;
        [LoadColumn(3)] public float Factor03;
        [LoadColumn(4)] public string Genero;
        [LoadColumn(5), ColumnName("Label")] public float NivelAnsiedad;
    }
}
