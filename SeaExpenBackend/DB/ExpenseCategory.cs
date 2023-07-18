using System;
using System.Collections.Generic;

namespace SeaExpenBackend.DB;

public partial class ExpenseCategory
{
    public int Id { get; set; }

    public string? Category { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
