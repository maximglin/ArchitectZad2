using System;
using System.Collections.Generic;

#nullable disable

namespace Service
{
    public partial class Color
    {
        public Color()
        {
            Cars = new HashSet<Car>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
    }
}
