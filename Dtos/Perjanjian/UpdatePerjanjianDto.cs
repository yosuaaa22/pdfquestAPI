namespace pdfquestAPI.Dtos.Perjanjian
{
    using System.ComponentModel.DataAnnotations;

    public class UpdatePerjanjianDto
    {
        [Required]
        public int IdPenyedia { get; set; }

        [MaxLength(100)]
        public string? NoPtInhealth { get; set; }

        [MaxLength(100)]
        public string? NoPtPihakKedua { get; set; }

        [Required]
        public DateTime TanggalTandaTangan { get; set; }

        [MaxLength(100)]
        public string? NomorBeritaAcara { get; set; }

        [Required]
        public DateTime TanggalBeritaAcara { get; set; }

        [MaxLength(100)]
        public string? JangkaWaktuPerjanjian { get; set; }
    }
}