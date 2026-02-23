using System;
using System.Collections.Generic;

namespace ECommerceWeb.Models;

public partial class Users
{
    public string Account { get; set; } = null!;

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? Role { get; set; }

    public DateTime? CreateTime { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedTime { get; set; }
}
