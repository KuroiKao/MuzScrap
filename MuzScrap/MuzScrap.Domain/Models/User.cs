using System;
using System.Collections.Generic;

namespace MuzScrap.Domain.Models
{
    public partial class User : DomainObject
    {
        public User()
        {
            Wishlists = new HashSet<Wishlist>();
        }

        public string Login { get; set; }

        public string Password { get; set; }

        public virtual IEnumerable<Wishlist> Wishlists { get; set; }
    }
}