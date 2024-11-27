using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace MuzScrap.Domain.Models
{
    public partial class Product : DomainObject
    {
        public Product()
        {
            Wishlists = new HashSet<Wishlist>();
        }

        public string? Brand { get; set; }

        public string? Title { get; set; }

        public string? Price { get; set; }

        public string? Store { get; set; }

        public string? Source { get; set; }

        public string? ProductType { get; set; }
        
        public string? Image { get; set; }

        public virtual IEnumerable<Wishlist> Wishlists { get; set; }

    }
}