using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req.LoanDto
{
    public class ReqLoanDto
    {
        [Required(ErrorMessage = "Amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "InterestRate is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Interest Rate must be a positive number")]
        public decimal InterestRate { get; set; }
        [Required(ErrorMessage = "Duration is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Duration must be a positive number")]
        public int Duration { get; set; }
    }
}
