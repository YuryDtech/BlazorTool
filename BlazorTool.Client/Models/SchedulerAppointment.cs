namespace BlazorTool.Client.Models
{
    public class SchedulerAppointment : WorkOrder
    {
        public string AppointmentId { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAllDay { get; set; }
        public string Description { get; set; } = string.Empty;

        public SchedulerAppointment()
        {
            AppointmentId = Guid.NewGuid().ToString();
            Title = string.Empty;
            Start = DateTime.Now;
            End = DateTime.Now.AddHours(5);
            IsAllDay = false;
            Description = string.Empty;
        }

        public SchedulerAppointment(WorkOrder order)
        {
            // Копируем все поля WorkOrder через рефлексию или руками
            CopyFromWorkOrder(order);

            AppointmentId = order.WorkOrderID.ToString();
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

        private void CopyFromWorkOrder(WorkOrder order)
        {
            // Копируем все поля вручную — только если явно нужно,
            // иначе все public/protected поля наследуются автоматически!
            this.WorkOrderID = order.WorkOrderID;
            this.MachineID = order.MachineID;
            this.AssetNo = order.AssetNo;
            this.DeviceCategory = order.DeviceCategory;
            this.WODesc = order.WODesc;
            this.WOCategory = order.WOCategory;
            this.WOState = order.WOState;
            this.WOReason = order.WOReason;
            this.AddDate = order.AddDate;
            this.TakeDate = order.TakeDate;
            this.CloseDate = order.CloseDate;
            this.Cost = order.Cost;
            this.TakePersons = order.TakePersons;
            this.PlanID = order.PlanID;
            this.StateColor = order.StateColor;
            this.ModDate = order.ModDate;
            this.ModPerson = order.ModPerson;
            this.WOLevel = order.WOLevel;
            this.LevelID = order.LevelID;
            this.ActCount = order.ActCount;
            this.DepName = order.DepName;
            this.AssignedPerson = order.AssignedPerson;
            this.FileCount = order.FileCount;
            this.PartCount = order.PartCount;
            this.PlanActCount = order.PlanActCount;
            this.StartDate = order.StartDate;
            this.EndDate = order.EndDate;
            this.IsScheduledPlanned = order.IsScheduledPlanned;
            this.CategoryID = order.CategoryID;
            this.ReasonID = order.ReasonID;
            this.IneffectiveCount = order.IneffectiveCount;
            this.PersonTakeDate = order.PersonTakeDate;
            this.DeviceCategoryID = order.DeviceCategoryID;
        }
    }
}
