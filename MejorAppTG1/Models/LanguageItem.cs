namespace MejorAppTG1.Models
{
    internal class LanguageItem(string name, string code, string flag)
    {
        public string Name { get; set; } = name;
        public string Code { get; set; } = code;
        public string Flag { get; set; } = flag;
    }
}
