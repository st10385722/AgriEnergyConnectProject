using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agri_EnergyConnect.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int RoleId { get; set; }

    public DateOnly CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<User> InverseCreatedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual UserRole Role { get; set; } = null!;

    // [NotMapped]
    // public string? Password {get; set;}

    // [NotMapped]
    // public string? ConfirmPassword {get; set;}
}
