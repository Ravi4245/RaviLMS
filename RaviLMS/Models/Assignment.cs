using System;
using System.Collections.Generic;

namespace RaviLMS.Models;

public partial class Assignment
{
    public int AssignmentId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? CourseId { get; set; }

    public int? StudentId { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual Student? Student { get; set; }
}
