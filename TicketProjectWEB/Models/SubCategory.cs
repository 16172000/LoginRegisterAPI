using System;
using System.Collections.Generic;

namespace TicketProjectWEB.Models;

public partial class SubCategory
{
    public int SubCategoryId { get; set; }

    public string? Name { get; set; }

    public int? DepartmentId { get; set; }

    public virtual DepartmentTbl? Department { get; set; }
}
