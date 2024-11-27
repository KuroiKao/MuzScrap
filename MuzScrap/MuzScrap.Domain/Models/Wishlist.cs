using System;
using System.Collections.Generic;

namespace MuzScrap.Domain.Models
{
    public partial class Wishlist : DomainObject
    {
        public int UserId { get; set; }

        public int ProductId { get; set; }

        public virtual User User { get; set; }

        public virtual Product Product { get; set; }
    }
}