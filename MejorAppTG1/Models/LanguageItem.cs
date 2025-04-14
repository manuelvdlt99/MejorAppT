using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MejorAppTG1.Models
{
    internal class LanguageItem
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Flag { get; set; }

        public LanguageItem(string name, string code, string flag)
        {
            Name = name;
            Code = code;
            Flag = flag;
        }
    }
}
