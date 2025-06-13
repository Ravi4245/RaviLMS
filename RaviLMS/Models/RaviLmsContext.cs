using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RaviLMS.Models;

public partial class RaviLmsContext : DbContext
{
    public RaviLmsContext()
    {
    }

    public RaviLmsContext(DbContextOptions<RaviLmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-5S4O1AS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__719FE488FE297508");

            entity.ToTable("Admin");

            entity.HasIndex(e => e.Email, "UQ__Admin__A9D10534AA7AFB35").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Assignme__32499E77A520A2B2");

            entity.ToTable("Assignment");

            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Course).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Assignmen__Cours__31EC6D26");

            entity.HasOne(d => d.Student).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Assignmen__Stude__32E0915F");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__C92D71A7CE8503F2");

            entity.ToTable("Course");

            entity.Property(e => e.CourseName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(250);

            entity.HasOne(d => d.Teacher).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__Course__TeacherI__2F10007B");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grade__54F87A57773AFA4C");

            entity.ToTable("Grade");

            entity.Property(e => e.GradeValue).HasMaxLength(10);

            entity.HasOne(d => d.Assignment).WithMany(p => p.Grades)
                .HasForeignKey(d => d.AssignmentId)
                .HasConstraintName("FK__Grade__Assignmen__35BCFE0A");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52B99375D5E74");

            entity.ToTable("Student");

            entity.HasIndex(e => e.Email, "UQ__Student__A9D1053436592A7D").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasMany(d => d.Courses).WithMany(p => p.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentCourse",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__StudentCo__Cours__4AB81AF0"),
                    l => l.HasOne<Student>().WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__StudentCo__Stude__49C3F6B7"),
                    j =>
                    {
                        j.HasKey("StudentId", "CourseId").HasName("PK__StudentC__5E57FC83D01D4F99");
                        j.ToTable("StudentCourse");
                    });
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teacher__EDF25964AE37ED55");

            entity.ToTable("Teacher");

            entity.HasIndex(e => e.Email, "UQ__Teacher__A9D105349388D0DB").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
