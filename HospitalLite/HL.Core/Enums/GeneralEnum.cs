namespace HL.Core.Enums
{
    public enum AppointmentStatus
    {
        Scheduled = 1, // Randevu oluşturuldu
        Checked = 2, // Hasta hastaneye geldi
        InExamination = 3, // Muayene sürüyor
        LabPending = 4, // Tahlil bekleniyor
        Completed = 5, // Sürec bitti
        Cancelled = 6 // İptal
    }
}
