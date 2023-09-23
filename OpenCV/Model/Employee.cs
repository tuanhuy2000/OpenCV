using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCV
{
    public class Employee
    {
        private string name;
        private int id;

        public Employee(string name)
        {
            this.name = name;
        }
        public Employee(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public string Name { get => name; set => name = value; }
        public int Id { get => id; set => id = value; }
    }
}
