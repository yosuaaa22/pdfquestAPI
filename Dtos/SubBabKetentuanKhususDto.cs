// In Models/DTOs/SubBabKetentuanKhususDto.cs
namespace pdfquestAPI.Models.DTOs
{
    public class SubBabKetentuanKhususDto
    {
        public int Id { get; set; }
        public string? Konten { get; set; }
        public int UrutanTampil { get; set; }
        public List<PoinKetentuanKhususDto> Poin { get; set; } = new();
    }
}