using System;
using System.Collections.Generic;

namespace Agri_EnergyConnect.Models;

public partial class UserRole
{
    //Model scaffolded from database
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string RoleDescription { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
