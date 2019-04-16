using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myApp.Models
{
    public class MyStudent : IEquatable<MyStudent>
    {
        public int id { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return "ID: " + id + "   Name: " + name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
                MyStudent objAsPart = obj as MyStudent;
            if (objAsPart == null) return false;
                else return Equals(objAsPart);
        }

        public override int GetHashCode()
        {
            return id;
        }

        public bool Equals(MyStudent other)
        {
            if (other == null) return false;
            return (this.id.Equals(other.id));
        }

    }
}
