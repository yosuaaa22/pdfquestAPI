// In Models/DTOs/JudulIsiDto.cs
namespace pdfquestAPI.Models.DTOs
{
    public class JudulIsiDto
    {
        public int Id { get; set; }
        public string? JudulTeks { get; set; }
        public int UrutanTampil { get; set; }
        public List<SubBabKetentuanKhususDto> SubBab { get; set; } = new();
    }
}