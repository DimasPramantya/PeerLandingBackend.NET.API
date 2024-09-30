using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("trn_repayment")]
    public class TrnRepayment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public decimal Amount { get; set; }
        [ForeignKey("Loan")]
        public string LoanId { get; set; }
        [Required]
        [Column("deadline_date")]
        public DateTime DeadlineDate { get; set; }
        [Column("repaid_at")]
        public DateTime? RepaidAt { get; set; }
        [Required]
        [Column("status")]
        public string Status { get; set; } = "pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public MstLoans Loan { get; set; }

    }
}
