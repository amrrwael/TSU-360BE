// ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TSU360.Models.Entities;

namespace TSU360.Database
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<TokenBlacklist> TokenBlacklists { get; set; }
        public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public DbSet<VolunteerApplication> VolunteerApplications { get; set; }
        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<ShopItem> ShopItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
       .Property(u => u.UserRole)
       .HasConversion<string>()
       .HasMaxLength(24);

            builder.Entity<User>()
        .Property(u => u.Faculty)
        .HasConversion<string>();  // Store as string

            builder.Entity<User>()
                .Property(u => u.Degree)
                .HasConversion<string>();

            // Configure the Event entity
            builder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Relationship with User
                entity.HasOne(e => e.CreatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Event>()
       .HasMany(e => e.VolunteerApplications)
       .WithOne(a => a.Event)
       .HasForeignKey(a => a.EventId)
       .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VolunteerApplication>()
                .HasMany(a => a.Responses)
                .WithOne(r => r.Application)
                .HasForeignKey(r => r.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Event -> SurveyQuestions with NO CASCADE
            builder.Entity<Event>()
                .HasMany(e => e.SurveyQuestions)
                .WithOne(q => q.Event)
                .HasForeignKey(q => q.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure SurveyResponse -> SurveyQuestion with ClientSetNull
            builder.Entity<SurveyResponse>()
                .HasOne(r => r.Question)
                .WithMany()
                .HasForeignKey(r => r.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);


             builder.Entity<Shop>()
            .HasMany(s => s.Items)
            .WithOne(i => i.Shop)
            .HasForeignKey(i => i.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Shop>().HasData(
        new Shop { Id = Guid.Parse("00000000-0000-0000-0000-000000000001") });

        }
    }
}