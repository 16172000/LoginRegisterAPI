using System;
using System.Collections.Generic;

namespace TicketProjectWEB.Models;

public partial class ResetPassword
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? ConfirmPassword { get; set; }

    public string? Salt { get; set; }

    public string? PasswordResetToken { get; set; }
}
