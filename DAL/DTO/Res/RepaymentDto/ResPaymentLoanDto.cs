using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res.RepaymentDto
{
    public class ResPaymentLoanDto
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string LoanId { get; set; }
        public DateTime DeadlineDate { get; set; }
        public DateTime? RepaidAt { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
