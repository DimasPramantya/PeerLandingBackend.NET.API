using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("mst_loans")]
    public class MstLoans
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("Borrower")]
        [Column("borrower_id")]
        public string? BorrowerId { get; set; }

        [ForeignKey("Lender")]
        [Column("lender_id")]
        public string? LenderId { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }
        [Required]
        [Column("interest_rate")]
        public decimal InterestRate { get; set; }
        [Required]
        [Column("duration_month")]
        public int DurationMonth { get; set; }
        [Required]
        [Column("status")]
        public string Status { get; set; } = "requested";

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("total_repaid")]
        public decimal TotalRepaid { get; set; } = 0;

        [Column("funded_at")]
        public DateTime? FundedAt { get; set; }
        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public MstUser? Borrower { get; set; }
        public MstUser? Lender { get; set; }
        public List<TrnRepayment> Repayments { get; set; } = new List<TrnRepayment>();

        [NotMapped] 
        public decimal? UnpaidAmount => TotalAmount - TotalRepaid;
    }
}
