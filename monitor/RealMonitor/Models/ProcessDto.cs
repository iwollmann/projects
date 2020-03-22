using System;

namespace Models
{
    public class ProcessDto
    {
        public string Name { get; set; }
        public bool Responding { get; set; }
        public double CPU { get; set; }
        public double Memory { get; set; }
    }
}
