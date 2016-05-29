using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.Model
{
    public class EHSystem
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public string Name { get; set; }
        public string PriceFormat
        {
            get
            {
                return String.Format("{0} €", Price);
            }

        }

        public EHSystem()
        {

        }
        public EHSystem(int id, string n, double p)
        {
            Id = id;
            Name = n;
            Price = p;
            
        }
    }
}
