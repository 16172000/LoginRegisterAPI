using System;
using System.Collections.Generic;

namespace LoginRegisterAPI.Models;

public partial class DepartmentTbl
{
    public int Id { get; set; }

    public string? UserName { get; set; }

    public string? Department { get; set; }

    public virtual ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}
