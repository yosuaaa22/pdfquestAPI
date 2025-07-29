// In Models/DTOs/PoinKetentuanKhususDto.cs
namespace pdfquestAPI.Models.DTOs
{
    public class PoinKetentuanKhususDto
    {
        public int Id { get; set; }
        public string? TeksPoin { get; set; }
        public int? Parent { get; set; }
        public int UrutanTampil { get; set; }
    }
}