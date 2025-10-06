using Microsoft.EntityFrameworkCore;
using _250828_universityTask.Models;

namespace _250828_universityTask.Data
{
    public class TryOutDbContext : DbContext
    {
        public TryOutDbContext(DbContextOptions<TryOutDbContext> options) : base(options) { }

        // overriding EF’s (Entity Framework) model building to customize relationships + seed data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // University - Student (one-to-many, optional)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.University)
                .WithMany(u => u.Students)
                .HasForeignKey(s => s.UniversityId)
                .OnDelete(DeleteBehavior.SetNull); // maybe also restrict?

            // University - Professor (one-to-many, required)
            modelBuilder.Entity<Professor>()
                .HasOne(p => p.University)
                .WithMany(u => u.Professors)
                .HasForeignKey(p => p.UniversityId)
                .OnDelete(DeleteBehavior.Restrict); // ??

            // Professor - Student (one-to-many, optional)
            modelBuilder.Entity<Student2>()
            .HasMany(s => s.BelongingProfessors)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "ProfessorStudent2", // name of the join table
                j => j.HasOne<Professor>()
                      .WithMany()
                      .HasForeignKey("ProfessorId")
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Student2>()
                      .WithMany()
                      .HasForeignKey("Student2Id")
                      .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasData(
                        new { ProfessorId = 1, Student2Id = 1 },
                        new { ProfessorId = 2, Student2Id = 1 },
                        new { ProfessorId = 1, Student2Id = 2 },
                        new { ProfessorId = 4, Student2Id = 2 },
                        new { ProfessorId = 1, Student2Id = 3 },
                        new { ProfessorId = 4, Student2Id = 3 }
                    );
                });

            modelBuilder.Entity<University>().HasData(
                new University { Id = 1, Name = "Massachusetts Institute of Technology (MIT)", City = "Cambridge", Country = "USA" },
                new University { Id = 2, Name = "Stanford University", City = "Stanford", Country = "USA" },
                new University { Id = 3, Name = "Imperial College London", City = "London", Country = "UK" }
            );

            modelBuilder.Entity<Professor>().HasData(
                new Professor { Id = 1, Name = "Klaus Mikaelson", Email = "klaus@example.com", UniversityId = 1 },
                new Professor { Id = 2, Name = "Tim McGraw", Email = "tim@example.com", UniversityId = 3 },
                new Professor { Id = 3, Name = "Sophia Goldberg", Email = "sophia@example.com", UniversityId = 1 },
                new Professor { Id = 4, Name = "Clara Bow", Email = "clara@example.com", UniversityId = 2 }
            );

            modelBuilder.Entity<Student2>().HasData(
                new Student2 { Id = 1, Name = "Addison Montgomery", UniversityId = 2, ProfessorAddedId = 4 },
                new Student2 { Id = 2, Name = "Hanna Marin", UniversityId = 3, ProfessorAddedId = 2 },
                new Student2 { Id = 3, Name = "Mark Sloan", UniversityId = 1, ProfessorAddedId = 1 }
            );
        }

        // tables
        public DbSet<Professor> Professors { get; set; }

        public DbSet<Student2> Students { get; set; }

        public DbSet<University> Universities { get; set; }
    }
}
