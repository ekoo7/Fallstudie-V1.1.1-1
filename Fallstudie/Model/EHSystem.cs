using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.Model
{
    public class EHSystem
    {
        public double Price { get; set; }
        public string Name { get; set; }

        public EHSystem(string n, double p)
        {
            Price = p;
            Name = n;
        }
    }
}
