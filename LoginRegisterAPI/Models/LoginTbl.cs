using System;
using System.Collections.Generic;

namespace LoginRegisterAPI.Models;

public partial class LoginTbl
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Salt { get; set; }
}
