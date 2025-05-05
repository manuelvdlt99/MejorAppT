using SQLite;

namespace MejorAppTG1.Models;

public class Test
{
    [PrimaryKey, AutoIncrement]
    public int IdTest { get; set; }

    public int IdUser { get; set; }
    public int EdadUser { get; set; }
    public string GeneroUser { get; set; }
    public string Tipo { get; set; }
    public DateTime Fecha { get; set; }
    public bool Terminado { get; set; }
    public string IdFirebase { get; set; }
}