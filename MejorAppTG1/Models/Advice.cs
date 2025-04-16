public class Advice
{
    public string Titulo { get; set; }
    public string Contenido { get; set; }

    // Lista de enlaces
    public List<string> Enlaces => (Contenido ?? string.Empty).Split('\n').Where(line => line.StartsWith("http")).ToList();
    // Lista de audios
    public List<string> Audios => (Contenido ?? string.Empty).Split('\n').Where(line => line.EndsWith(".mp3")).ToList();
    // Frases sin enlaces
    public List<string> LineasTexto => (Contenido ?? string.Empty).Split('\n').Where(line => !line.StartsWith("http") && !line.EndsWith(".mp3")).ToList();

    public bool HasLinks => Enlaces.Any();
    public bool HasAudio => Audios.Any();
    public string Imagen { get; set; }
}
