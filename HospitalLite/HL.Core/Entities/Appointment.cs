using HL.Core.Common;
using HL.Core.Enums;

namespace HL.Core.Entities
{
    internal class Appointment : BaseEntity
    {
        public int PatientId { get; set; }
        public virtual AppUser Patient { get; set; } = null;
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null;
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    }
}
