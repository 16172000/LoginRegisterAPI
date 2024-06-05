using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TicketProjectWEB.Models;

public partial class ResetPassword
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }
    [DisplayName("Confirm Password")]
    public string? ConfirmPassword { get; set; }

    public string? Salt { get; set; }

    [DisplayName("Password Reset Token")]
    public string? PasswordResetToken { get; set; }
}
