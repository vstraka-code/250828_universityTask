using _250828_universityTask.Models;
using Microsoft.EntityFrameworkCore;

namespace _250828_universityTask.Data
{
    // represents your database session
    public class AppDbContext : DbContext
    {
        // constructor
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

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
            modelBuilder.Entity<Student>()
                .HasOne(s => s.ProfessorAdded)
                .WithMany(p => p.AddedStudents)
                .HasForeignKey(s => s.ProfessorAddedId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<University>().HasData(
                new University { Id = 1, Name = "Massachusetts Institute of Technology (MIT)", City = "Cambridge", Country = "USA" },
                new University { Id = 2, Name = "Stanford University", City = "Stanford", Country = "USA" },
                new University { Id = 3, Name = "Imperial College London", City = "London", Country = "UK" }
            );

            modelBuilder.Entity<Professor>().HasData(
                new Professor { Id = 1, Name = "Klaus Mikaelson", UniversityId = 1 },
                new Professor { Id = 2, Name = "Tim McGraw", UniversityId = 3 },
                new Professor { Id = 3, Name = "Sophia Goldberg", UniversityId = 1 },
                new Professor { Id = 4, Name = "Clara Bow", UniversityId = 2 }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, Name = "Addison Montgomery", UniversityId = 2, ProfessorAddedId = 4 },
                new Student { Id = 2, Name = "Hanna Marin", UniversityId = 3, ProfessorAddedId = 2 },
                new Student { Id = 3, Name = "Mark Sloan", UniversityId = 1, ProfessorAddedId = 1 }
            );
        }

        // tables
        public DbSet<Professor> Professors { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<University> Universities { get; set; }
        
    }
}
