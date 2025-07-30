namespace pdfquestAPI.Dtos.JudulIsi
{
    using System.ComponentModel.DataAnnotations;

    public class CreateJudulIsiDto
    {
        [Required]
        public int IdPenyedia { get; set; }

        [Required]
        [MaxLength(255)]
        public required string JudulTeks { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int UrutanTampil { get; set; }
    }
}