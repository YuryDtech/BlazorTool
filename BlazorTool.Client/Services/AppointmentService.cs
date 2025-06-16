using static BlazorTool.Client.Pages.SchedulerPage;
using BlazorTool.Client.Models;
namespace BlazorTool.Client.Services

{
    public class AppointmentService
    {
        private List<SchedulerAppointment> _appointments;
        public AppointmentService()
        {
            _appointments = GetAppointments();
        }
        public List<SchedulerAppointment> GetAllAppointments()
        {
            return _appointments;
        }
        public SchedulerAppointment GetAppointmentById(string id)
        {
            return _appointments.FirstOrDefault(x => x.Id == id);
        }

        public void AddAppointment(SchedulerAppointment appointment)
        {
            appointment.Id = Guid.NewGuid().ToString();
            _appointments.Add(appointment);
        }
        public void UpdateAppointment(SchedulerAppointment appointment)
        {
            var existingAppointment = _appointments.FirstOrDefault(x => x.Id == appointment.Id);
            if (existingAppointment != null)
            {
                existingAppointment.Title = appointment.Title;
                existingAppointment.Start = appointment.Start;
                existingAppointment.End = appointment.End;
                existingAppointment.Description = appointment.Description;
                existingAppointment.IsAllDay = appointment.IsAllDay;
            }
        }
        public void DeleteAppointment(string id)
        {
            var appointment = _appointments.FirstOrDefault(x => x.Id == id);
            if (appointment != null)
            {
                _appointments.Remove(appointment);
            }
        }

        public void DeleteAllAppointments()
        {
            _appointments.Clear();
        }


        private List<SchedulerAppointment> GetAppointments()
        {
            var appoi = new List<SchedulerAppointment>()
        {
            new SchedulerAppointment
            {
                Title = "Daily Stand-up (or Sit-down)",
                Start = new DateTime(2025, 6, 1, 9, 0, 0),
                End = new DateTime(2025, 6, 1, 9, 30, 0),
                Description = "Daily team sync-up meeting."
            },
            new SchedulerAppointment
            {
                Title = "Coffee Machine Debugging Session",
                Start = new DateTime(2025, 6, 1, 10, 15, 0),
                End = new DateTime(2025, 6, 1, 10, 45, 0),
                Description = "Discussing coffee-related issues."
            },
            new SchedulerAppointment
            {
                Title = "Rubber Duck Programming Hour",
                Start = new DateTime(2025, 6, 2, 11, 0, 0),
                End = new DateTime(2025, 6, 2, 12, 0, 0),
                Description = "Time to talk to your rubber duck."
            },
            new SchedulerAppointment
            {
                Title = "404 Error: Meeting Not Found",
                Start = new DateTime(2025, 6, 2, 14, 0, 0),
                End = new DateTime(2025, 6, 2, 15, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Ctrl+Alt+Delete Your Problems",
                Start = new DateTime(2025, 6, 3, 9, 30, 0),
                End = new DateTime(2025, 6, 3, 10, 30, 0)
            },
            new SchedulerAppointment
            {
                Title = "Code Review: Judging Other People's Code",
                Start = new DateTime(2025, 6, 3, 13, 0, 0),
                End = new DateTime(2025, 6, 3, 14, 30, 0)
            },
            new SchedulerAppointment
            {
                Title = "Stack Overflow Research Time",
                Start = new DateTime(2025, 6, 4, 10, 0, 0),
                End = new DateTime(2025, 6, 4, 11, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Agile Sprint Planning (More Like Marathon)",
                Start = new DateTime(2025, 6, 4, 15, 0, 0),
                End = new DateTime(2025, 6, 4, 17, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Debugging: It's Not a Bug, It's a Feature",
                Start = new DateTime(2025, 6, 5, 9, 0, 0),
                End = new DateTime(2025, 6, 5, 10, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Team Building: Escape Room (Debug Edition)",
                Start = new DateTime(2025, 6, 5, 16, 0, 0),
                End = new DateTime(2025, 6, 5, 18, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Weekend Hackathon: Pizza and Coding",
                IsAllDay = true,
                Start = new DateTime(2025, 6, 7),
                End = new DateTime(2025, 6, 8)
            },
            new SchedulerAppointment
            {
                Title = "Monday Morning Motivation Meeting",
                Start = new DateTime(2025, 6, 9, 8, 0, 0),
                End = new DateTime(2025, 6, 9, 9, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Git Commit Therapy Session",
                Start = new DateTime(2025, 6, 9, 11, 30, 0),
                End = new DateTime(2025, 6, 9, 12, 30, 0)
            },
            new SchedulerAppointment
            {
                Title = "Scrum Master's Stand-up Comedy",
                Start = new DateTime(2025, 6, 10, 14, 0, 0),
                End = new DateTime(2025, 6, 10, 14, 30, 0)
            },
            new SchedulerAppointment
            {
                Title = "CSS Zen Garden Meditation",
                Start = new DateTime(2025, 6, 10, 16, 0, 0),
                End = new DateTime(2025, 6, 10, 17, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Database Relationship Counseling",
                Start = new DateTime(2025, 6, 11, 10, 0, 0),
                End = new DateTime(2025, 6, 11, 11, 30, 0)
            },
            new SchedulerAppointment
            {
                Title = "API Documentation: The Novel Nobody Reads",
                Start = new DateTime(2025, 6, 11, 13, 0, 0),
                End = new DateTime(2025, 6, 11, 15, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Localhost Lunch Break",
                Start = new DateTime(2025, 6, 12, 12, 0, 0),
                End = new DateTime(2025, 6, 12, 13, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Deployment Day: Crossing Fingers and Toes",
                Start = new DateTime(2025, 6, 12, 15, 0, 0),
                End = new DateTime(2025, 6, 12, 17, 30, 0)
            },
            new SchedulerAppointment
            {
                Title = "Tech Conference: Learning New Ways to Overthink",
                IsAllDay = true,
                Start = new DateTime(2025, 6, 13),
                End = new DateTime(2025, 6, 15)
            }

        };
            appoi.ForEach(x =>
            {
                x.Id = Guid.NewGuid().ToString();
            });
            return appoi;
        }
    }
}
