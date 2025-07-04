﻿using System;
using System.Collections.Generic;

namespace RaviLMS.Models;

public partial class Teacher
{
    public int TeacherId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Status { get; set; }

    public DateTime? DateOfBirth { get; set; }    

    public string PhoneNumber { get; set; }

    public string City { get; set; }


    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

}
