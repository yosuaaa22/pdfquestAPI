namespace pdfquestAPI.Dtos
{
    public class KontenStrukturDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public required string LevelType { get; set; } // Judul, SubBab, atau Poin
        public string? Konten { get; set; }
        public int UrutanTampil { get; set; }
    }
}