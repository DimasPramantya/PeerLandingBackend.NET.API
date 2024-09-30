using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("mst_cash_flow")]
    public class CashFlow
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [ForeignKey("User")]
        [Column("user_id")]
        public string UserId { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("type")]
        public string Type { get; set; }


        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public MstUser User { get; set; }
    }
}
