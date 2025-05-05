using SQLite;

namespace MejorAppTG1.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Genero { get; set; }
        public string Imagen { get; set; }
    }

}
