using System;
using System.Collections.Generic;

#nullable disable

namespace Service
{
    public partial class Car
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public int? Manufacturer { get; set; }
        public int? Color { get; set; }
        public decimal? Price { get; set; }

        public virtual Color ColorNavigation { get; set; }
        public virtual Manufacturer ManufacturerNavigation { get; set; }
    }
}
