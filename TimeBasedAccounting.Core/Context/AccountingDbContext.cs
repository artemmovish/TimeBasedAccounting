using Microsoft.EntityFrameworkCore;
using TimeBasedAccounting.Core.Models;
using TimeBasedAccounting.Core.Services;

namespace TimeBasedAccounting.Core.Context
{
    public class AccountingDbContext : DbContext
    {
        public AccountingDbContext(DbContextOptions<AccountingDbContext> options) : base(options)
        {
        }

        // 👇 Эти сущности не имеют таблиц, но нужны для FromSqlRaw
        public DbSet<AddUserResult> AddUserResults { get; set; }
        public DbSet<LoginResult> LoginResults { get; set; }

        /// <summary>
        /// Маркеры посещаемости (например, "Явка", "Отпуск", "Больничный" и т.д.).
        /// </summary>
        public DbSet<AttendanceMarker> AttendanceMarkers { get; set; }

        /// <summary>
        /// Отделы организации.
        /// </summary>
        public DbSet<Department> Departments { get; set; }

        /// <summary>
        /// Сотрудники, работающие в организации.
        /// </summary>
        public DbSet<Employee> Employees { get; set; }

        /// <summary>
        /// Опоздания сотрудников (привязаны к табелю).
        /// </summary>
        public DbSet<Lateness> Latenesses { get; set; }

        /// <summary>
        /// Кэшированные отчёты (для ускорения повторных запросов).
        /// </summary>
        public DbSet<ReportCache> ReportCaches { get; set; }

        /// <summary>
        /// Табели учёта рабочего времени (Timesheet).
        /// </summary>
        public DbSet<Timesheet> Timesheets { get; set; }

        /// <summary>
        /// Пользователи системы (учётные записи, логины и роли).
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Отпуска сотрудников.
        /// </summary>
        public DbSet<Vacation> Vacations { get; set; }

        /// <summary>
        /// Статусы отпусков (например, "Одобрен", "Отклонён", "На рассмотрении").
        /// </summary>
        public DbSet<VacationStatus> VacationStatuses { get; set; }

        /// <summary>
        /// Типы отпусков (например, "Ежегодный", "Без сохранения з/п", "Больничный").
        /// </summary>
        public DbSet<VacationType> VacationTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Отмечаем их как "keyless" (без ключей)
            modelBuilder.Entity<AddUserResult>().HasNoKey();
            modelBuilder.Entity<LoginResult>().HasNoKey();

            // Конфигурация AttendanceMarker
            modelBuilder.Entity<AttendanceMarker>(entity =>
            {
                entity.HasKey(e => e.MarkerId);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
            });

            // Конфигурация Department
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DepartmentId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Конфигурация Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Position).IsRequired().HasMaxLength(100);
                entity.Property(e => e.HireDate).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();

                // Связь с Department
                entity.HasOne(e => e.Department)
                      .WithMany(d => d.Employees)
                      .HasForeignKey(e => e.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Конфигурация Lateness
            modelBuilder.Entity<Lateness>(entity =>
            {
                entity.HasKey(e => e.LatenessId);
                entity.Property(e => e.DurationMinutes).IsRequired();
                entity.Property(e => e.Reason).HasMaxLength(500);

                // Связь с Timesheet
                entity.HasOne(e => e.Timesheet)
                      .WithOne(t => t.Lateness)
                      .HasForeignKey<Lateness>(e => e.TimesheetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Конфигурация ReportCache
            modelBuilder.Entity<ReportCache>(entity =>
            {
                entity.HasKey(e => e.ReportId);
                entity.Property(e => e.ReportType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ReportData).IsRequired();
                entity.Property(e => e.Period).IsRequired();
                entity.Property(e => e.GeneratedAt).IsRequired();

                // Связь с User
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.GeneratedBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Конфигурация Timesheet
            modelBuilder.Entity<Timesheet>(entity =>
            {
                entity.HasKey(e => e.TimesheetId);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.HoursWorked).IsRequired().HasPrecision(5, 2);
                entity.Property(e => e.RecordedAt).IsRequired();

                // Связь с Employee
                entity.HasOne(e => e.Employee)
                      .WithMany(emp => emp.Timesheets)
                      .HasForeignKey(e => e.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Связь с AttendanceMarker
                entity.HasOne(e => e.AttendanceMarker)
                      .WithMany(am => am.Timesheets)
                      .HasForeignKey(e => e.MarkerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Связь с User
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.RecordedBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Конфигурация User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Login).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired().HasConversion<string>();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.CreatedAt).IsRequired();

                // Индекс для логина
                entity.HasIndex(e => e.Login).IsUnique();
            });

            // Конфигурация Vacation
            modelBuilder.Entity<Vacation>(entity =>
            {
                entity.HasKey(e => e.VacationId);
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Comment).HasMaxLength(500);

                // Связь с Employee
                entity.HasOne(e => e.Employee)
                      .WithMany(emp => emp.Vacations)
                      .HasForeignKey(e => e.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Связь с VacationType
                entity.HasOne(e => e.VacationType)
                      .WithMany(vt => vt.Vacations)
                      .HasForeignKey(e => e.VacationTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Связь с VacationStatus
                entity.HasOne(e => e.VacationStatus)
                      .WithMany(vs => vs.Vacations)
                      .HasForeignKey(e => e.VacationStatusId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Связь с User
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Конфигурация VacationStatus
            modelBuilder.Entity<VacationStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });

            // Конфигурация VacationType
            modelBuilder.Entity<VacationType>(entity =>
            {
                entity.HasKey(e => e.TypeId);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });
        }
    }
}
