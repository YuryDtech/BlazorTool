namespace BlazorTool.Client.Models
{
    public class SchedulerAppointment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAllDay { get; set; }
        public string Description { get; set; } = string.Empty;

        public SchedulerAppointment()
        {
            //new SchedulerAppointment from now
            Id = Guid.NewGuid().ToString();
            Title = string.Empty;
            Start = DateTime.Now;
            End = DateTime.Now.AddHours(5);
            IsAllDay = false;
            Description = string.Empty;
        }
        public SchedulerAppointment(WorkOrder order)
        {
            Id = order.WorkOrderID.ToString();
            Title = order.AssetNo ?? $"WO {order.WorkOrderID}";
            Description = order.WODesc ?? string.Empty;

            Start = order.StartDate
                    ?? order.TakeDate
                    ?? order.AddDate
                    ?? DateTime.Now;

            End = order.EndDate
                  ?? order.CloseDate
                  ?? Start.AddHours(1);

            IsAllDay = false;
        }
    }
}
