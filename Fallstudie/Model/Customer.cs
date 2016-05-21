using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.ViewModel
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int NumbProjects { get; set; }

        public int NumbConfHouses { get; set; }


        public Customer(){ }

        public Customer(int id, string name, int nP, int nCH)
        {
            this.Id = id;
            this.Name = name;
            this.NumbProjects = nP;
            this.NumbConfHouses = nCH;
        }
    }
}
