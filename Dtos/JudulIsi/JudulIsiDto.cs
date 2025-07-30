namespace pdfquestAPI.Dtos.JudulIsi
{
    public class JudulIsiDto
    {
        public int Id { get; set; }
        public int? IdPenyedia { get; set; } // Menggunakan int? agar bisa null
        public string? JudulTeks { get; set; }
        public int UrutanTampil { get; set; }
    }
}