using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuizManager.DBModels
{
    public class QuizContext : IdentityDbContext<ApplicationUser>
    {
        public QuizContext() : base("name=QuizDB") { }

        public static QuizContext Create()
        {
            return new QuizContext();
        }

        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupReference> GroupReferences { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Quiz> Quizzes { get; set; }
        public virtual DbSet<QuizAttempt> QuizAttempts { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<QuizReference> QuizReferences { get; set; }
        public virtual DbSet<ResultType> ResultTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Course>().
            //  HasMany(c => c.Students).
            //  WithMany(p => p.CoursesAttending).
            //  Map(
            //   m =>
            //   {
            //       m.MapLeftKey("CourseId");
            //       m.MapRightKey("PersonId");
            //       m.ToTable("PersonCourses");
            //   });

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Quiz>().
                HasMany(x => x.Groups).
                WithMany(y => y.Quizs).Map(m =>
                    {
                        m.ToTable("Quiz_Group");
                    });

            modelBuilder.Entity<Group>().
                HasMany(x => x.ApplicationUsers).
                WithMany(y => y.Groups).Map(m =>
                {
                    m.ToTable("Group_User");
                });
        }
    }
}
