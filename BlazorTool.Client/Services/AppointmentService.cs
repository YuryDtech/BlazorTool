using static BlazorTool.Client.Pages.SchedulerPage;
using BlazorTool.Client.Models;
namespace BlazorTool.Client.Services

{
    public class AppointmentService
    {
        private List<SchedulerAppointment> _appointments = new();
        private readonly ApiServiceClient _apiServiceClient;
        public AppointmentService(ApiServiceClient apiServiceClient)
        {
            _apiServiceClient = apiServiceClient;
        }
        
        public async Task InitializeAsync()
        {
            var orders = await _apiServiceClient.GetWorkOrdersCachedAsync();
            _appointments = ConvertAppointmentsFromOrders(orders);
        }
        public List<SchedulerAppointment> GetAllAppointments()
        {
            return _appointments;
        }
        public SchedulerAppointment? GetAppointmentById(string id)
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

        public List<SchedulerAppointment> GetAppointmentsFromOrders(IEnumerable<WorkOrder> orders)
        {
            if (orders == null || !orders.Any())
                return new List<SchedulerAppointment>();
            return ConvertAppointmentsFromOrders(orders.ToList());
        }

        private List<SchedulerAppointment> ConvertAppointmentsFromOrders(List<WorkOrder> orders)
        {
            var appointments = new List<SchedulerAppointment>();
            foreach (var order in orders)
                 appointments.Add(new SchedulerAppointment(order));

            return appointments;
        }
    }
}
