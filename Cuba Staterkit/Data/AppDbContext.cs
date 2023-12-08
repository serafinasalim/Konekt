using Cuba_Staterkit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cuba_Staterkit.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet properties for your model classes
        public DbSet<t_account> Account { get; set; }
        public DbSet<t_employee> Employee { get; set; }
        public DbSet<t_position> Position { get; set; }
        public DbSet<t_attendance> Attendance { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            new t_accountConfiguration(builder.Entity<t_account>());
            new t_employeeConfiguration(builder.Entity<t_employee>());
            new t_positionConfiguration(builder.Entity<t_position>());
            new t_attendanceConfiguration(builder.Entity<t_attendance>());
        }

    }
    // Configuration class for t_account
    public class t_accountConfiguration
    {
        public t_accountConfiguration(EntityTypeBuilder<t_account> entity)
        {
            entity
                .HasKey(a => a.user_id);
            entity
                .HasOne<t_employee>(e => e.Employee)
                .WithOne(a => a.Account)
                .HasForeignKey<t_account>(a => a.username)
                .HasPrincipalKey<t_employee>(e => e.username);
        }
    }
    // Configuration class for t_employee
    public class t_employeeConfiguration
    {
        public t_employeeConfiguration(EntityTypeBuilder<t_employee> entity)
        {
            entity
                .HasKey(e => e.employee_id);
            entity
                .HasOne<t_account>(a => a.Account)
                .WithOne(e => e.Employee)
                .HasForeignKey<t_employee>(e => e.username)
                .HasPrincipalKey<t_account>(a => a.username);
            entity
                .HasOne<t_position>(e => e.Position)
                .WithMany(a => a.Employee)
                .HasForeignKey(a => a.position_id)
                .HasPrincipalKey(e => e.position_id);
            // Add more configurations for t_employee if needed
        }
    }
    //Configuration class for t_position
    public class t_positionConfiguration
    {
        public t_positionConfiguration(EntityTypeBuilder<t_position> entity)
        {
            entity
                .HasKey(p => p.position_id);
        }
    }
    //Configuration class for t_attendance
    public class t_attendanceConfiguration
    {
        public t_attendanceConfiguration(EntityTypeBuilder<t_attendance> entity)
        {
            entity
                .HasKey(a => a.attendance_id);
            entity
                .HasOne<t_employee>(e => e.Employee)
                .WithMany(a => a.Attendance)
                .HasForeignKey(a => a.employee_id)
                .HasPrincipalKey(e => e.employee_id);
        }
    }

}
