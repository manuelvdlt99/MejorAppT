namespace MejorAppTG1.Models
{
    public class Question
    {
        public string Content { get; set; }
        public string Factor { get; set; }

        public override string ToString()
        {
            return Content + "  " + Factor;
        }
    }
}
