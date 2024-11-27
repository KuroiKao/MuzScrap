using MuzScrap.Domain.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MuzScrap.BaseContext
{
    public partial class ProductCard
    {
        public string? Brand { get; set; }

        public string? Title { get; set; }

        public string? Price { get; set; }

        public string? Price2 { get; set; }

        public string? Store { get; set; }

        public string? Store2 { get; set; }

        public string? Source { get; set; }

        public string? Source2 { get; set; }

        public string? ProductType { get; set; }

        public string? Image { get; set; }

        public string? Image2 { get; set; }

        public virtual IEnumerable<Product> Products { get; set;}
    }
}
