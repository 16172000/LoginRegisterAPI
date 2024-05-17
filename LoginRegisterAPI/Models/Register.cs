using System;
using System.Collections.Generic;

namespace LoginRegisterAPI.Models;

public partial class Register
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string ConfirmPassword { get; set; } = null!;

    public int Age { get; set; }

    public DateTime Dob { get; set; }

    public string State { get; set; } = null!;

    public long PhoneNumber { get; set; }

    public string? Salt { get; set; }

    public string? PasswordResetToken { get; set; }

    public DateTime? TokenExpiration { get; set; }

    public virtual ICollection<Tickett> Ticketts { get; set; } = new List<Tickett>();
}
