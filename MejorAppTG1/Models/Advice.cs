using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Windows.Input;

public class Advice : INotifyPropertyChanged
{
    [JsonIgnore]
    private string _imagen = "flechaabajo.png"; // Imagen inicial

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

    public string Imagen
    {
        get => _imagen;
        set
        {
            if (_imagen != value)
            {
                _imagen = value;
                OnPropertyChanged(nameof(Imagen));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
