using System;
using System.Collections.Generic;

namespace TicketProjectWEB.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Designation { get; set; } = null!;

    public int Age { get; set; }

    public string City { get; set; } = null!;

    public string Salary { get; set; } = null!;

    public string File { get; set; } = null!;
}
