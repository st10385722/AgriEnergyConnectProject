using System;
using System.Collections.Generic;

namespace Agri_EnergyConnect.Models;

public partial class ProductImage
{
    public int ImageId { get; set; }

    public int? ProductId { get; set; }

    public byte[] ImageData { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Product? Product { get; set; }
}
