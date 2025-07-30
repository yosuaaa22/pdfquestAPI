using System.ComponentModel.DataAnnotations;

namespace pdfquestAPI.Dtos.JudulIsi // <-- PASTIKAN NAMESPACE INI BENAR
{
    // PASTIKAN NAMA CLASS INI BENAR
    public class CreateJudulIsiDto 
    {
        [Required]
        public int IdJenisPenyedia { get; set; }

        [Required]
        [MaxLength(255)]
        public required string JudulTeks { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int UrutanTampil { get; set; }
    }
}