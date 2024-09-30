using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res.CashFlow
{
    public class ResCashFlowDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
