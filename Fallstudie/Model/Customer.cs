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
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public int NumbProjects { get; set; }

        public int NumbConfHouses { get; set; }

        public Customer(int id, string fn, string ln, int nP, int nCH)
        {
            this.Id = id;
            this.Firstname = fn;
            this.Lastname = ln;
            this.NumbProjects = nP;
            this.NumbConfHouses = nCH;
        }
    }
}
