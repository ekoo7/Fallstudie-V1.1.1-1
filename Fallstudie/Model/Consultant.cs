﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.Model
{
    public class Consultant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Consultant(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public Consultant()
        {

        }
    }
}
