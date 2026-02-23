using System;
using System.Collections.Generic;

namespace ECommerceWeb.Models;

public partial class Products
{
    public string ProductId { get; set; } = null!;

    public string? ProductName { get; set; }

    public string? ProductCategory { get; set; }

    public int? Stock { get; set; }

    public string? Status { get; set; }

    public string? ImagePath { get; set; }

    public DateTime? CreateTime { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedTime { get; set; }
}
