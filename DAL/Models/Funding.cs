using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("trn_funding")]
    public class Funding
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [ForeignKey("Loan")]
        public string LoanId { get; set; }
        [Required]
        [ForeignKey("Lender")]
        public string LenderId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime FundedAt { get; set; }

        public MstLoans Loan { get; set; }
        public MstUser Lender { get; set; }
    }
}
