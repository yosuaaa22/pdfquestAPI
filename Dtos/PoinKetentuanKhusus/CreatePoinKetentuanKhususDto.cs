namespace pdfquestAPI.Dtos.PoinKetentuanKhusus
{
    using System.ComponentModel.DataAnnotations;

    public class CreatePoinKetentuanKhususDto
    {
        [Required]
        public int IdSubBab { get; set; }

        public string? TeksPoin { get; set; }

        // Parent bisa null, jadi tidak perlu [Required]
        public int? Parent { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int UrutanTampil { get; set; }
    }
}