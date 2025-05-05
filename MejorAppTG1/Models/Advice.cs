namespace MejorAppTG1.Models;

public class Advice
{
    public string Titulo { get; set; }
    public string Contenido { get; set; }
    public string Imagen { get; set; }

    // Lista de enlaces
    public IEnumerable<string> Enlaces =>
    (Contenido ?? string.Empty)
        .Split('\n')
        .Where(line => line.StartsWith("http"));

    // Lista de audios
    public IEnumerable<string> Audios =>
    (Contenido ?? string.Empty)
        .Split('\n')
        .Where(line => line.StartsWith(".mp3"));
    // Frases sin enlaces
    public IEnumerable<string> LineasTexto =>
    (Contenido ?? string.Empty)
        .Split('\n')
        .Where(line => !line.StartsWith("http") && !line.EndsWith(".mp3"));

    public bool HasLinks => Enlaces.Any();
    public bool HasAudio => Audios.Any();
    public bool HasImage => !string.IsNullOrWhiteSpace(Imagen);
}
