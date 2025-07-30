namespace pdfquestAPI.Dtos.Perjanjian
{
    public class PerjanjianDto
    {
        public int IdPerjanjian { get; set; }
        public int IdPenyedia { get; set; }
        public string? NoPtInhealth { get; set; }
        public string? NoPtPihakKedua { get; set; }
        public DateTime TanggalTandaTangan { get; set; }
        public string? NomorBeritaAcara { get; set; }
        public DateTime TanggalBeritaAcara { get; set; }
        public string? JangkaWaktuPerjanjian { get; set; }
    }
}