using MejorAppTG1.Models;

namespace UnitTests
{
    public class AdviceTestCase
    {
        public string Name { get; set; }
        public int PuntuacionTotal { get; set; }
        public Factor Factor1 { get; set; }
        public Factor Factor2 { get; set; }
        public Factor Factor3 { get; set; }
        public string TipoTest { get; set; }
        public List<AdviceCategory> Expected { get; set; }

        public override string ToString() => Name;
    }

}
