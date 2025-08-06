using pdfquestAPI.Models; // Pastikan namespace untuk model DB Anda sudah benar
using System.Collections.Generic;

namespace pdfquestAPI.Documents.Models
{
    /// <summary>
    /// Model ini adalah "paket data" lengkap untuk satu dokumen perjanjian.
    /// </summary>
    public class PerjanjianDocumentModel
    {
        // Data dari tabel Perjanjian
        public required Perjanjian Perjanjian { get; set; }
    
        // Data dari tabel PihakPertama
        public required PihakPertama PihakPertama { get; set; }
        
        // Data dari tabel PenyediaLayanan (Pihak Kedua)
        public required PenyediaLayanan PihakKedua { get; set; }
    
        // Data dinamis untuk Ketentuan Khusus (Bab, Sub-Bab, dan Poin)
        public List<BabModel> KetentuanKhusus { get; set; } = new();

        // Data dinamis untuk Lampiran
        public List<BabModel> Lampiran { get; set; } = new();
    }
    
    /// <summary>
    /// Model untuk merepresentasikan satu Bab (dari tabel JudulIsi)
    /// </summary>
    public class BabModel
    {
        public required string JudulTeks { get; set; }
        public int UrutanTampil { get; set; }
        public List<SubBabModel> SubBab { get; set; } = new();
    }
    
    /// <summary>
    /// Model untuk merepresentasikan satu Sub-Bab
    /// </summary>
    public class SubBabModel
    {
        public required string Konten { get; set; }
        public int UrutanTampil { get; set; }
        public List<PoinModel> Poin { get; set; } = new();
    }
    
    /// <summary>
    /// Model untuk merepresentasikan satu Poin (bisa berjenjang/nested)
    /// </summary>
    public class PoinModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public required string TeksPoin { get; set; }
        public int UrutanTampil { get; set; }
        public List<PoinModel> SubPoin { get; set; } = new();
    }
}
