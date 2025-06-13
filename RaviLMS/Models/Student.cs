using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RaviLMS.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; } 

    public string? Password { get; set; } 

    public string? Status { get; set; }

    public DateTime? DateOfBirth { get; set; }   
    public string PhoneNumber { get; set; }

    public string? city { get; set; }

    [JsonIgnore]

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    [JsonIgnore]

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    

}
