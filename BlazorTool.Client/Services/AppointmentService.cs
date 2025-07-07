using BlazorTool.Client.Models;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using static BlazorTool.Client.Pages.SchedulerPage;
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
        public async Task<List<SchedulerAppointment>> GetAllAppointments()
        {
            if (_appointments == null || !_appointments.Any())
            {
                var orders = await _apiServiceClient.GetWorkOrdersCachedAsync();
                _appointments = GetAppointmentsFromOrders(orders);
            }
            return _appointments;
        }

        public async Task<List<SchedulerAppointment>> GetTakenAppointments(List<WorkOrder>? orders = null)
        {
                if (_appointments == null || !_appointments.Any())
                {
                    if (orders == null)
                    {
                        Console.WriteLine("===> _appointments = null, run _apiServiceClient.GetWorkOrdersCachedAsync()");
                        orders = await _apiServiceClient.GetWorkOrdersCachedAsync();
                    }
                    _appointments = GetAppointmentsFromOrders(orders);
                }
                return _appointments.Where(x => !string.IsNullOrWhiteSpace(x.DepName) && x.Start != null).ToList();
        }

        public async Task<List<SchedulerAppointment>> GetUnTakenAppointments()
        {
            if (_appointments == null || !_appointments.Any())
            {
                var orders = await _apiServiceClient.GetWorkOrdersCachedAsync();
                _appointments = GetAppointmentsFromOrders(orders);
            }
            return _appointments.Where(x => string.IsNullOrWhiteSpace(x.DepName) || x.Start == null).ToList();
        }

        public SchedulerAppointment? GetAppointmentById(int id)
        {
            return _appointments.FirstOrDefault(x => x.AppointmentId == id);
        }

        public async Task AddAppointment(SchedulerAppointment appointment)
        {
            //na razie nie dodajemy nowych zadań, tylko aktualizujemy istniejące

            //appointment.AppointmentId = 0;
            //_appointments.Add(appointment);
            //await _apiServiceClient.SaveWorkOrderAsync((WorkOrder)appointment);
        }
        public async Task UpdateAppointment(SchedulerAppointment appointment)
        {
            var existingAppointment = _appointments.FirstOrDefault(x => x.AppointmentId == appointment.AppointmentId);
            if (existingAppointment != null)
            {
                existingAppointment = appointment.ShallowCopy();
                await _apiServiceClient.SaveWorkOrderAsync((WorkOrder)existingAppointment);
            }
            else // New appointment, add it to the list
            {
                var newId = await _apiServiceClient.SaveWorkOrderAsync((WorkOrder)appointment);
                if (newId < 0)
                {
                    Console.WriteLine("Error: Failed to save the new appointment.");
                    return;
                }
                appointment.AppointmentId = newId;
                _appointments.Add(appointment);
            }
        }

        public async Task DeleteAppointment(SchedulerAppointment ap)
        {
            if (ap != null)
            {
                _appointments.Remove(ap);
                await _apiServiceClient.DeleteWorkOrderAsync(ap.WorkOrderID, ap.MachineID);
            }
        }

        public void DeleteAllAppointments()
        {
            //TODO delete start and end dates for all appointments
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
