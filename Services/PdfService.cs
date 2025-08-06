using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Documents;
using pdfquestAPI.Documents.Models;
using pdfquestAPI.Interfaces;
using pdfquestAPI.Models; // Pastikan namespace untuk model DB Anda sudah benar
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace pdfquestAPI.Services
{
    public class PdfService : IPdfService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Data.ApplicationDbContext _context;

        public PdfService(IUnitOfWork unitOfWork, Data.ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<byte[]> GeneratePerjanjianPdfAsync(int perjanjianId)
        {
            var model = await BuildPerjanjianModelAsync(perjanjianId);
            var document = new PerjanjianDocument(model);
            byte[] pdfBytes = document.GeneratePdf();
            return pdfBytes;
        }

        private async Task<PerjanjianDocumentModel> BuildPerjanjianModelAsync(int perjanjianId)
        {
            var perjanjian = await _unitOfWork.Perjanjian.GetByIdAsync(perjanjianId)
                ?? throw new KeyNotFoundException("Perjanjian tidak ditemukan.");

            var pihakKedua = await _unitOfWork.PenyediaLayanan.GetByIdAsync(perjanjian.IdPenyedia)
                ?? throw new KeyNotFoundException("Penyedia Layanan tidak ditemukan.");

            var pihakPertama = (await _unitOfWork.PihakPertama.GetAllAsync()).FirstOrDefault()
                ?? throw new KeyNotFoundException("Data Pihak Pertama tidak ditemukan.");

            // Mengambil data Ketentuan Khusus (yang BUKAN Lampiran)
            var ketentuanKhusus = await GetHierarchicalDataAsync(p => !p.JudulTeks.StartsWith("LAMPIRAN"));

            // Mengambil data Lampiran
            var lampiran = await GetHierarchicalDataAsync(p => p.JudulTeks.StartsWith("LAMPIRAN"));

            return new PerjanjianDocumentModel
            {
                Perjanjian = perjanjian,
                PihakPertama = pihakPertama,
                PihakKedua = pihakKedua,
                KetentuanKhusus = ketentuanKhusus,
                Lampiran = lampiran
            };
        }

        // PERBAIKAN: Metode ini disesuaikan untuk menghindari error kompilasi.
        private async Task<List<BabModel>> GetHierarchicalDataAsync(Expression<Func<JudulIsi, bool>> filter)
        {
            var allBab = await _context.JudulIsi.Where(filter).OrderBy(j => j.UrutanTampil).ToListAsync();
            if (!allBab.Any()) return new List<BabModel>();

            var allBabIds = allBab.Select(b => b.Id).ToList();

            var allSubBab = await _context.SubBabKetentuanKhusus
                .Where(s => allBabIds.Contains(s.IdJudul))
                .OrderBy(s => s.UrutanTampil).ToListAsync();
            
            var allSubBabIds = allSubBab.Select(s => s.Id).ToList();

            // PERBAIKAN: Query ini diubah untuk menghindari penggunaan .HasValue pada tipe 'int'.
            // Cara ini lebih aman dan tetap efisien.
            var allPoin = await _context.PoinKetentuanKhusus
                .Where(p => allSubBabIds.Contains(p.IdSubBab)) // Langsung cek containment.
                .OrderBy(p => p.UrutanTampil).ToListAsync();

            var babList = allBab.Select(bab => new BabModel
            {
                JudulTeks = bab.JudulTeks ?? string.Empty,
                UrutanTampil = bab.UrutanTampil,
                SubBab = allSubBab
                    .Where(sb => sb.IdJudul == bab.Id)
                    .Select(sb => new SubBabModel
                    {
                        Konten = sb.Konten ?? string.Empty,
                        UrutanTampil = sb.UrutanTampil,
                        // Menggunakan daftar 'allPoin' yang sudah diambil sebelumnya untuk efisiensi.
                        Poin = BuildPoinTree(allPoin.Where(p => p.IdSubBab == sb.Id).ToList(), null)
                    }).ToList()
            }).ToList();

            return babList;
        }

        private List<PoinModel> BuildPoinTree(List<PoinKetentuanKhusus> allPoinForSubBab, int? parentId)
        {
            return allPoinForSubBab
                .Where(p => p.Parent == parentId)
                .OrderBy(p => p.UrutanTampil)
                .Select(p => new PoinModel
                {
                    Id = p.Id,
                    ParentId = p.Parent,
                    TeksPoin = p.TeksPoin ?? string.Empty,
                    UrutanTampil = p.UrutanTampil,
                    SubPoin = BuildPoinTree(allPoinForSubBab, p.Id)
                }).ToList();
        }
    }
}