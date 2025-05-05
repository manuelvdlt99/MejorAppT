using SQLite;

namespace MejorAppTG1.Models;

public class Answer
{
    // Este paquete de SQLite no admite claves primarias compuestas (en este caso sería IdPregunta + IdTest)
    // Tendremos que confiar en que no se repetirán los IdTest autogenerados
    [PrimaryKey, AutoIncrement]
    public int IdAnswer { get; set; }
    public int IdPregunta { get; set; }
    public int IdTest { get; set; }
    public string Factor { get; set; }
    public int ValorRespuesta { get; set; }
}