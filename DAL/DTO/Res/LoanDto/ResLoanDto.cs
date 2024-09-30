using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Res.UserDto;

namespace DAL.DTO.Res.LoadDto
{
    public class ResLoanDto
    {
        public string Id { get; set; }
        public string BorrowerId { get; set; }
        public string? LenderId { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal TotalRepaid { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }
        public ResSingleUserDto Borrower { get; set; }
        public ResSingleUserDto? Lender { get; set; }
        public DateTime? FundedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
