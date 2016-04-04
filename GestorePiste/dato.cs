using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorePiste
{
    
    public class Dato
    {
        public int valoreprimaauto { get; set; }
        public int valoresecondaauto { get; set; }

        public Dato (int val1, int val2)
        {
            valoreprimaauto = val1;
            valoresecondaauto = val2;
        }

    }
}
