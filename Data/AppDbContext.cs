using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Models.Entities;

namespace TourManagement.Web.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<TourItinerary> TourItineraries => Set<TourItinerary>();
    public DbSet<TourSchedule> TourSchedules => Set<TourSchedule>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingParticipant> BookingParticipants => Set<BookingParticipant>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Guide> Guides => Set<Guide>();
    public DbSet<GuideAssignment> GuideAssignments => Set<GuideAssignment>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(b =>
        {
            b.ToTable("Users");
            b.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            b.HasIndex(u => u.Email).IsUnique();
            b.HasIndex(u => u.PhoneNumber).IsUnique().HasFilter(null);
        });

        builder.Entity<Destination>(b =>
        {
            b.ToTable("Destinations");
            b.HasIndex(x => x.Name);
        });

        builder.Entity<Tour>(b =>
        {
            b.ToTable("Tours");
            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.Name);
            b.HasOne(x => x.Destination).WithMany(d => d.Tours)
                .HasForeignKey(x => x.DestinationId).OnDelete(DeleteBehavior.Restrict);
            b.Property(x => x.RowVersion).IsRowVersion();
            b.Property(x => x.BasePrice).HasColumnType("decimal(18,2)");
        });

        builder.Entity<TourItinerary>(b =>
        {
            b.ToTable("TourItineraries");
            b.HasOne(x => x.Tour).WithMany(t => t.Itineraries)
                .HasForeignKey(x => x.TourId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => new { x.TourId, x.DayNumber });
        });

        builder.Entity<TourSchedule>(b =>
        {
            b.ToTable("TourSchedules");
            b.HasIndex(x => x.Code).IsUnique();
            b.HasOne(x => x.Tour).WithMany(t => t.Schedules)
                .HasForeignKey(x => x.TourId).OnDelete(DeleteBehavior.Cascade);
            b.Property(x => x.RowVersion).IsRowVersion();
            b.Property(x => x.Price).HasColumnType("decimal(18,2)");
            b.HasIndex(x => x.StartDate);
        });

        builder.Entity<Booking>(b =>
        {
            b.ToTable("Bookings");
            b.HasIndex(x => x.BookingCode).IsUnique();
            b.HasOne(x => x.User).WithMany(u => u.Bookings)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Schedule).WithMany(s => s.Bookings)
                .HasForeignKey(x => x.ScheduleId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Promotion).WithMany(p => p.Bookings)
                .HasForeignKey(x => x.PromotionId).OnDelete(DeleteBehavior.SetNull);
            b.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
            b.Property(x => x.Discount).HasColumnType("decimal(18,2)");
            b.Property(x => x.Surcharge).HasColumnType("decimal(18,2)");
            b.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            b.Property(x => x.PaidAmount).HasColumnType("decimal(18,2)");
            b.HasIndex(x => x.BookedAt);
        });

        builder.Entity<BookingParticipant>(b =>
        {
            b.ToTable("BookingParticipants");
            b.HasOne(x => x.Booking).WithMany(b => b.Participants)
                .HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Payment>(b =>
        {
            b.ToTable("Payments");
            b.HasIndex(x => x.TransactionCode).IsUnique();
            b.HasOne(x => x.Booking).WithMany(b => b.Payments)
                .HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
            b.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        });

        builder.Entity<Guide>(b =>
        {
            b.ToTable("Guides");
        });

        builder.Entity<GuideAssignment>(b =>
        {
            b.ToTable("GuideAssignments");
            b.HasOne(x => x.Guide).WithMany(g => g.Assignments)
                .HasForeignKey(x => x.GuideId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.Schedule).WithMany(s => s.GuideAssignments)
                .HasForeignKey(x => x.ScheduleId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => new { x.GuideId, x.ScheduleId }).IsUnique();
        });

        builder.Entity<Review>(b =>
        {
            b.ToTable("Reviews");
            b.HasOne(x => x.Booking).WithOne(b => b.Review)
                .HasForeignKey<Review>(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.User).WithMany(u => u.Reviews)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Tour).WithMany(t => t.Reviews)
                .HasForeignKey(x => x.TourId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => x.TourId);
        });

        builder.Entity<Promotion>(b =>
        {
            b.ToTable("Promotions");
            b.HasIndex(x => x.Code).IsUnique();
            b.Property(x => x.DiscountValue).HasColumnType("decimal(18,2)");
            b.Property(x => x.MaxDiscount).HasColumnType("decimal(18,2)");
            b.Property(x => x.MinOrderValue).HasColumnType("decimal(18,2)");
        });

        builder.Entity<Notification>(b =>
        {
            b.ToTable("Notifications");
            b.HasOne(x => x.User).WithMany(u => u.Notifications)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AuditLog>(b =>
        {
            b.ToTable("AuditLogs");
            b.HasOne(x => x.User).WithMany(u => u.AuditLogs)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
            b.HasIndex(x => x.CreatedAt);
        });
    }
}