using System;
using System.Collections.Generic;

namespace RaviLMS.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string? CourseName { get; set; }

    public string? Description { get; set; }

    public int? TeacherId { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual Teacher? Teacher { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();



}
