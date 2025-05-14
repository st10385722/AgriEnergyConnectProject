using System;
using System.Collections.Generic;

namespace Agri_EnergyConnect.Models;

//Model scaffolded from database
public partial class Product
{
    //Model scaffolded from database

    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductType { get; set; } = null!;

    public string ProductDescription { get; set; } = null!;

    public int? Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? FarmerId { get; set; }

    public int? ProductImageId { get; set; }

    public decimal? Price { get; set; }

    public virtual Farmer? Farmer { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}
