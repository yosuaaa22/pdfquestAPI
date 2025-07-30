namespace pdfquestAPI.Dtos.PoinKetentuanKhusus
{
    public class PoinKetentuanKhususDto
    {
        public int Id { get; set; }
        public int IdSubBab { get; set; }
        public string? TeksPoin { get; set; }
        public int? Parent { get; set; }
        public int UrutanTampil { get; set; }
    }
}