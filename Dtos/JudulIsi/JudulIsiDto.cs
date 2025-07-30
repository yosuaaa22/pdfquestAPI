namespace pdfquestAPI.Dtos.JudulIsi // <-- PASTIKAN NAMESPACE INI BENAR
{
    // PASTIKAN NAMA CLASS INI BENAR
    public class JudulIsiDto
    {
        public int Id { get; set; }
        public int IdJenisPenyedia { get; set; }
        public string? JudulTeks { get; set; }
        public int UrutanTampil { get; set; }
    }
}