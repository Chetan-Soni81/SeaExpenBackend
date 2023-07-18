using System;
using System.Collections.Generic;

namespace SeaExpenBackend.DB;

public partial class Expense
{
    public int Id { get; set; }

    public int Category { get; set; }

    public double Amount { get; set; }

    public DateTime ExpenseDate { get; set; }

    public bool? IsDelete { get; set; }

    public int UserId { get; set; }

    public virtual ExpenseCategory CategoryNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
