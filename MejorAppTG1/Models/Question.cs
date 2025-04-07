using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MejorAppTG1.Models
{
    public class Question
    {
        private string content;
        private string factor;
        public string Content { get => content; set => content = value; }
        public string Factor { get => factor; set => factor = value; }

        public override string ToString()
        {
            return Content + "  " + Factor;
        }
    }
}
