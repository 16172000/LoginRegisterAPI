using System.ComponentModel.DataAnnotations;

namespace TicketProjectWEB.Models
{
    public partial class ChartDataModel
    {
        [Key]
        public int Id { get; set; }
        public string Category { get; set; }
        public int TotalTicketsPending { get; set; }
    }
}
