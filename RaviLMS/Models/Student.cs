using System;
using System.Collections.Generic;

namespace RaviLMS.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
