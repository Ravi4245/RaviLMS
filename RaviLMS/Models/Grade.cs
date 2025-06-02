using System;
using System.Collections.Generic;

namespace RaviLMS.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public int? AssignmentId { get; set; }

    public string? GradeValue { get; set; }

    public virtual Assignment? Assignment { get; set; }
}
