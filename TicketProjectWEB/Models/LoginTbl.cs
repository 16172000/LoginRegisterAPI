using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TicketProjectWEB.Models;

public partial class LoginTbl
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Email is Required")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Passowrd is Required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public string? Salt { get; set; }
}
