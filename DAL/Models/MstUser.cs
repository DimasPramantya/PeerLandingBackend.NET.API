using DAL.Models;
using System;
using System.Collections.Generic;


public class MstUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public decimal Balance { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<MstLoans> Loans { get; set; } = new List<MstLoans>();
    public List<MstLoans> Fundings { get; set; } = new List<MstLoans>();
    public List<CashFlow> CashFlows { get; set; } = new List<CashFlow>();
}
