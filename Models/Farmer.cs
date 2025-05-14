using System;
using System.Collections.Generic;

namespace Agri_EnergyConnect.Models;

//Model scaffolded from database
public partial class Farmer
{
    public int FarmerId { get; set; }

    public int? UserId { get; set; }

    public string FarmName { get; set; } = null!;

    public string FarmType { get; set; } = null!;

    public DateTime HavestingDate { get; set; }

    public string? CropType { get; set; }

    public string? LivestockType { get; set; }

    public int NumberOfEmployees { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual User? User { get; set; }
}
