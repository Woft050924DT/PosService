using System;
using System.Collections.Generic;

namespace PosService.Models;

public partial class Shift
{
    public int ShiftId { get; set; }

    public string ShiftNumber { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public decimal? StartingCash { get; set; }

    public decimal? EndingCash { get; set; }

    public decimal? ExpectedCash { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public virtual User User { get; set; } = null!;
}
