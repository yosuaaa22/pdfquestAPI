namespace pdfquestAPI.Dtos.SubBabKetentuanKhusus
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateSubBabKetentuanKhususDto
    {
        [Required]
        public int IdJudul { get; set; }

        public string? Konten { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int UrutanTampil { get; set; }
    }
}
