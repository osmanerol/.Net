using HL.Core.Common;

namespace HL.Core.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
