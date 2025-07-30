namespace pdfquestAPI.Interfaces
{
    public interface IPdfService
    {
        // Menghasilkan PDF sebagai byte array berdasarkan ID Perjanjian
        Task<byte[]> GeneratePerjanjianPdfAsync(int perjanjianId);
    }
}