using System;
using System.Collections.Generic;

namespace Agri_EnergyConnect.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string Email { get; set; } = null!;

    public int RoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public virtual ICollection<Farmer> Farmers { get; set; } = new List<Farmer>();

    public virtual UserRole Role { get; set; } = null!;
}
