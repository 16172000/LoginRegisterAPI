using System;
using System.Collections.Generic;

namespace LoginRegisterAPI.Models;

public partial class Tickett
{
    public int TicketId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? SubCategory { get; set; }

    public string? Attachment { get; set; }

    public string? Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreationDate { get; set; }

    public string? AssignedTo { get; set; }

    public virtual Register? CreatedByNavigation { get; set; }
}
