using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAsterix
{
    public class Dictionary_Info
    {
        public string OriginalName { get; set; }
        public string NameWithArrow { get; set; }

        public Dictionary_Info(string originalName)
        {
            OriginalName = originalName;
            NameWithArrow = $"{originalName} ⬇";  // Nombre con flecha
        }
    }
}
