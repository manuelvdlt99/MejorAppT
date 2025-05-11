using MejorAppTG1.Models;

namespace UnitTests
{
    public class FactorTestCase
    {
        public string Name { get; set; }
        public List<Answer> Answers { get; set; }
        public string Factor { get; set; }
        public Test TestData { get; set; }
        public Factor Expected { get; set; }

        public override string ToString() => Name;
    }

}
