using System;
using System.Collections.Generic;

#nullable disable

namespace Service
{
    public partial class Manufacturer
    {
        public Manufacturer()
        {
            Cars = new HashSet<Car>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
    }
}
