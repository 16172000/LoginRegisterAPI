using System;
using System.Collections.Generic;

namespace LoginRegisterAPI.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public decimal? Rating { get; set; }

    public int? Stock { get; set; }

    public string? Brand { get; set; }

    public string? Category { get; set; }

    public string? Thumbnail { get; set; }

    public string Status { get; set; } = null!;
}
