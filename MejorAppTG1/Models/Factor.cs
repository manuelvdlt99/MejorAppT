namespace MejorAppTG1.Models
{
    public class Factor
    {
        public string NumeroFactor { get; set; }
        public int Puntuacion { get; set; }
        public string Nivel { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not Factor other) return false;
            return NumeroFactor.Equals(other.NumeroFactor) &&
                   Puntuacion == other.Puntuacion &&
                   Nivel.Equals(other.Nivel);
        }

        public override int GetHashCode()
            => HashCode.Combine(NumeroFactor, Puntuacion, Nivel);
    }
}
