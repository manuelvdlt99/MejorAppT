namespace MejorAppTG1.Models
{
    public class Factor(string NumeroFactor, int Puntuacion, string Nivel)
    {
        public string NumeroFactor { get; set; } = NumeroFactor;
        public int Puntuacion { get; set; } = Puntuacion;
        public string Nivel { get; set; } = Nivel;
    }
}
