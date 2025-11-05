using System;
using System.Collections.Generic;
using System.Linq;
using TimeBasedAccounting.Core.Models;

namespace TimeBasedAccounting.Core.Context
{
    public static class DbSeeder
    {
        public static void Seed(AccountingDbContext db)
        {
            // --- Пользователи ---
            if (!db.Users.Any())
            {
                db.Users.AddRange(
                    new User
                    {
                        Login = "admin",
                        PasswordHash = "admin",
                        Role = UserRole.Admin,
                        FullName = "Администратор системы",
                        CreatedAt = DateTime.Now
                    },
                    new User
                    {
                        Login = "user",
                        PasswordHash = "123456",
                        Role = UserRole.Accountant,
                        FullName = "Табельщик Иванова И.И.",
                        CreatedAt = DateTime.Now
                    },
                    new User
                    {
                        Login = "manager",
                        PasswordHash = "123456",
                        Role = UserRole.Accountant,
                        FullName = "Менеджер отдела кадров Петрова С.В.",
                        CreatedAt = DateTime.Now
                    }
                );
                db.SaveChanges();
            }

            // --- Отделы ---
            if (!db.Departments.Any())
            {
                db.Departments.AddRange(
                    new Department { Name = "Бухгалтерия" },
                    new Department { Name = "ИТ-отдел" },
                    new Department { Name = "Отдел кадров" },
                    new Department { Name = "Отдел продаж" },
                    new Department { Name = "Производственный отдел" }
                );
                db.SaveChanges();
            }

            // --- Маркеры явок ---
            if (!db.AttendanceMarkers.Any())
            {
                db.AttendanceMarkers.AddRange(
                    new AttendanceMarker { Code = "Я", Description = "Явка" },
                    new AttendanceMarker { Code = "ОТП", Description = "Отпуск" },
                    new AttendanceMarker { Code = "Б", Description = "Больничный" },
                    new AttendanceMarker { Code = "Н", Description = "Неявка" },
                    new AttendanceMarker { Code = "Я/О", Description = "Явка с опозданием" },
                    new AttendanceMarker { Code = "К", Description = "Командировка" },
                    new AttendanceMarker { Code = "В", Description = "Выходной" }
                );
                db.SaveChanges();
            }

            // --- Типы отпусков ---
            if (!db.VacationTypes.Any())
            {
                db.VacationTypes.AddRange(
                    new VacationType { Code = "ТР", Name = "Трудовой" },
                    new VacationType { Code = "УЧ", Name = "Учебный" },
                    new VacationType { Code = "БСЗ", Name = "Без сохранения з/п" },
                    new VacationType { Code = "ДЕКР", Name = "Декретный" },
                    new VacationType { Code = "БОЛ", Name = "Больничный" }
                );
                db.SaveChanges();
            }

            // --- Статусы отпусков ---
            if (!db.VacationStatuses.Any())
            {
                db.VacationStatuses.AddRange(
                    new VacationStatus { Name = "Запланирован" },
                    new VacationStatus { Name = "Утвержден" },
                    new VacationStatus { Name = "Завершен" },
                    new VacationStatus { Name = "Отклонен" }
                );
                db.SaveChanges();
            }

            // --- Сотрудники ---
            if (!db.Employees.Any())
            {
                db.Employees.AddRange(
                    new Employee
                    {
                        FullName = "Петров Петр Петрович",
                        Position = "Главный бухгалтер",
                        HireDate = new DateTime(2021, 5, 12),
                        IsActive = true,
                        DepartmentId = 1
                    },
                    new Employee
                    {
                        FullName = "Сидоров Сергей Иванович",
                        Position = "Системный администратор",
                        HireDate = new DateTime(2022, 2, 15),
                        IsActive = true,
                        DepartmentId = 2
                    },
                    new Employee
                    {
                        FullName = "Козлова Анна Михайловна",
                        Position = "Специалист по кадрам",
                        HireDate = new DateTime(2023, 1, 10),
                        IsActive = true,
                        DepartmentId = 3
                    },
                    new Employee
                    {
                        FullName = "Николаев Дмитрий Владимирович",
                        Position = "Менеджер по продажам",
                        HireDate = new DateTime(2022, 8, 22),
                        IsActive = true,
                        DepartmentId = 4
                    },
                    new Employee
                    {
                        FullName = "Федорова Екатерина Сергеевна",
                        Position = "Инженер-технолог",
                        HireDate = new DateTime(2021, 11, 5),
                        IsActive = true,
                        DepartmentId = 5
                    },
                    new Employee
                    {
                        FullName = "Волков Алексей Петрович",
                        Position = "Программист",
                        HireDate = new DateTime(2023, 3, 18),
                        IsActive = true,
                        DepartmentId = 2
                    }
                );
                db.SaveChanges();
            }

            // --- Табели учета рабочего времени ---
            if (!db.Timesheets.Any())
            {
                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month - 1, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var timesheets = new List<Timesheet>();
                var random = new Random();

                var latenessReasons = new[]
                {
        "Пробки на дорогах",
        "Проблемы с общественным транспортом",
        "Посещение врача",
        "Задержался из-за ребенка",
        "Поломка автомобиля",
        "Сильный дождь / снегопад",
        "Задержка на предыдущей встрече"
    };

                // Получаем активных сотрудников и их отпуска
                var employees = db.Employees.Where(e => e.IsActive).ToList();
                var vacations = db.Vacations
                    .Where(v => v.VacationStatusId != 4) // Исключаем отклоненные отпуска
                    .ToList();

                // Создаем табели для всех сотрудников за последний месяц
                foreach (var employee in employees)
                {
                    // Получаем отпуска сотрудника
                    var employeeVacations = vacations
                        .Where(v => v.EmployeeId == employee.EmployeeId)
                        .ToList();

                    for (int i = 0; i < 30; i++)
                    {
                        var date = startOfMonth.AddDays(i);

                        // Пропускаем выходные
                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                            continue;

                        // Проверяем, находится ли сотрудник в отпуске в эту дату
                        var isOnVacation = employeeVacations.Any(v =>
                            date >= v.StartDate && date <= v.EndDate);

                        int markerId;
                        decimal hoursWorked;
                        string? comment = null;

                        if (isOnVacation)
                        {
                            // Если в отпуске - ставим маркер отпуска
                            markerId = 2; // ОТП - Отпуск
                            hoursWorked = 0;
                            comment = "Находится в отпуске";
                        }
                        else
                        {
                            // С вероятностью 8% — опоздание, 90% — обычная явка, 2% — неявка
                            int chance = random.Next(0, 100);

                            if (chance < 8) // 8% вероятность опоздания
                            {
                                markerId = 5; // Я/О - Явка с опозданием
                                hoursWorked = random.Next(5, 8); // 5-7 часов работы при опоздании
                                comment = latenessReasons[random.Next(latenessReasons.Length)];
                            }
                            else if (chance < 98) // 90% вероятность явки
                            {
                                markerId = 1; // Я - Явка
                                hoursWorked = 8.0m;
                                comment = "d";
                            }
                            else // 2% вероятность неявки
                            {
                                markerId = 4; // Н - Неявка
                                hoursWorked = 0;
                                comment = "Неявка по неуважительной причине";
                            }
                        }

                        timesheets.Add(new Timesheet
                        {
                            Date = date,
                            HoursWorked = hoursWorked,
                            RecordedAt = date.AddHours(9 + random.Next(0, 3)), // Запись между 9 и 12 часами
                            EmployeeId = employee.EmployeeId,
                            MarkerId = markerId,
                            RecordedBy = 2, // user
                            Comment = comment
                        });
                    }
                }

                db.Timesheets.AddRange(timesheets);
                db.SaveChanges();
            }

            // --- Опоздания ---
            if (!db.Latenesses.Any())
            {
                var random = new Random();
                var latenessReasons = new[]
                {
        "Пробки на дорогах",
        "Проблемы с общественным транспортом",
        "Посещение врача",
        "Задержался из-за ребенка",
        "Поломка автомобиля",
        "Сильный дождь / снегопад",
        "Задержка на предыдущей встрече"
    };

                // Находим все табели с опозданиями (маркер Я/О)
                var lateTimesheets = db.Timesheets
                    .Where(t => t.MarkerId == 5) // Я/О - Явка с опозданием
                    .ToList();

                var latenesses = new List<Lateness>();

                foreach (var timesheet in lateTimesheets)
                {
                    // Генерируем реалистичное время опоздания: 5-45 минут
                    var durationMinutes = random.Next(1, 9) * 5; // 5, 10, 15, ..., 45 минут

                    latenesses.Add(new Lateness
                    {
                        TimesheetId = timesheet.TimesheetId,
                        DurationMinutes = durationMinutes,
                        Reason = latenessReasons[random.Next(latenessReasons.Length)]
                    });
                }

                if (latenesses.Any())
                {
                    db.Latenesses.AddRange(latenesses);
                    db.SaveChanges();
                }
            }

            // --- Отпуска ---
            if (!db.Vacations.Any())
            {
                var vacations = new[]
                {
                    new Vacation
                    {
                        StartDate = new DateTime(2024, 6, 1),
                        EndDate = new DateTime(2024, 6, 14),
                        CreatedAt = new DateTime(2024, 4, 15),
                        Comment = "Ежегодный оплачиваемый отпуск",
                        EmployeeId = 1,
                        VacationTypeId = 1, // Трудовой
                        VacationStatusId = 2, // Утвержден
                        CreatedBy = 2
                    },
                    new Vacation
                    {
                        StartDate = new DateTime(2024, 7, 1),
                        EndDate = new DateTime(2024, 7, 10),
                        CreatedAt = new DateTime(2024, 5, 20),
                        Comment = "Учебный отпуск для сдачи сессии",
                        EmployeeId = 3,
                        VacationTypeId = 2, // Учебный
                        VacationStatusId = 1, // Запланирован
                        CreatedBy = 2
                    },
                    new Vacation
                    {
                        StartDate = new DateTime(2024, 5, 1),
                        EndDate = new DateTime(2024, 5, 14),
                        CreatedAt = new DateTime(2024, 3, 10),
                        Comment = "Декретный отпуск",
                        EmployeeId = 5,
                        VacationTypeId = 4, // Декретный
                        VacationStatusId = 3, // Завершен
                        CreatedBy = 1
                    },
                    new Vacation
                    {
                        StartDate = new DateTime(2024, 8, 1),
                        EndDate = new DateTime(2024, 8, 5),
                        CreatedAt = new DateTime(2024, 6, 1),
                        Comment = "Отпуск за свой счет - семейные обстоятельства",
                        EmployeeId = 2,
                        VacationTypeId = 3, // Без сохранения з/п
                        VacationStatusId = 1, // Запланирован
                        CreatedBy = 2
                    }
                };

                db.Vacations.AddRange(vacations);
                db.SaveChanges();
            }

            // --- Кэшированные отчеты ---
            if (!db.ReportCaches.Any())
            {
                var reports = new[]
                {
                    new ReportCache
                    {
                        ReportType = "AttendanceSummary",
                        ReportData = "{\"totalEmployees\": 6, \"averageAttendance\": 95.5, \"totalLatenesses\": 12}",
                        Period = new DateTime(2024, 4, 1),
                        GeneratedAt = DateTime.Now.AddDays(-1),
                        GeneratedBy = 2
                    },
                    new ReportCache
                    {
                        ReportType = "VacationSchedule",
                        ReportData = "{\"scheduledVacations\": 3, \"activeVacations\": 1, \"upcomingVacations\": 2}",
                        Period = new DateTime(2024, 4, 1),
                        GeneratedAt = DateTime.Now.AddDays(-2),
                        GeneratedBy = 1
                    },
                    new ReportCache
                    {
                        ReportType = "DepartmentWorkHours",
                        ReportData = "{\"IT\": 160, \"Accounting\": 168, \"HR\": 152, \"Sales\": 144, \"Production\": 156}",
                        Period = new DateTime(2024, 4, 1),
                        GeneratedAt = DateTime.Now.AddDays(-3),
                        GeneratedBy = 2
                    }
                };

                db.ReportCaches.AddRange(reports);
                db.SaveChanges();
            }
        }
    }
}