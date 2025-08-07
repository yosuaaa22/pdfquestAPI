using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Data; // Pastikan namespace DbContext Anda sudah benar
using pdfquestAPI.Documents;
using pdfquestAPI.Documents.Models;
using pdfquestAPI.Interfaces;
using pdfquestAPI.Models;
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
        private readonly ApplicationDbContext _context;

        public PdfService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<byte[]> GeneratePerjanjianPdfAsync(int id_penyedia)
        {
            // Menggunakan id_penyedia untuk membangun model
            var model = await BuildPerjanjianModelAsync(id_penyedia);
            var document = new PerjanjianDocument(model);
            byte[] pdfBytes = document.GeneratePdf();
            return pdfBytes;
        }

        private async Task<PerjanjianDocumentModel> BuildPerjanjianModelAsync(int id_penyedia)
{
    // Mengambil data spesifik berdasarkan id_penyedia
    var pihakKedua = await _unitOfWork.PenyediaLayanan.GetByIdAsync(id_penyedia)
        ?? throw new KeyNotFoundException($"Penyedia Layanan dengan ID {id_penyedia} tidak ditemukan.");

    var perjanjian = await _context.Perjanjian.FirstOrDefaultAsync(p => p.IdPenyedia == id_penyedia)
        ?? throw new KeyNotFoundException($"Perjanjian untuk Penyedia Layanan ID {id_penyedia} tidak ditemukan.");

    var pihakPertama = (await _unitOfWork.PihakPertama.GetAllAsync()).FirstOrDefault()
        ?? throw new KeyNotFoundException("Data Pihak Pertama tidak ditemukan.");

    // --- PERUBAHAN LOGIKA PENGAMBILAN DATA ---

    // 1. Mengambil data Ketentuan Khusus dari MASTER TEMPLATE (id_penyedia IS NULL)
    var ketentuanKhusus = await GetHierarchicalDataAsync(p => p.IdPenyedia == null && !(p.JudulTeks.StartsWith("LAMPIRAN")));

    // 2. Mengambil data struktur Lampiran dari MASTER TEMPLATE (id_penyedia IS NULL)
    var lampiran = await GetHierarchicalDataAsync(p => p.IdPenyedia == null && p.JudulTeks.StartsWith("LAMPIRAN"));

    // 3. Mengambil data ISI TABEL yang SPESIFIK untuk id_penyedia yang diminta
    var lampiranPicData = await _context.Lampiran_PIC
        .Where(p => p.IdPenyedia == id_penyedia) // Tetap menggunakan id_penyedia
        .Select(p => new PicModel
        {
            JenisPIC = p.JenisPIC,
            NamaPIC = p.NamaPIC,
            NomorTelepon = p.NomorTelepon,
            AlamatEmail = p.AlamatEmail
        })
        .ToListAsync();

    var lampiranTindakanMedisData = await _context.Lampiran_TindakanMedis
        .Where(t => t.IdPenyedia == id_penyedia) // Tetap menggunakan id_penyedia
        .Select(t => new TindakanMedisModel
        {
            JenisTindakanMedis = t.JenisTindakanMedis,
            Tarif = t.Tarif,
            Keterangan = t.Keterangan
        })
        .ToListAsync();

    // --- AKHIR PERUBAHAN ---

    return new PerjanjianDocumentModel
    {
        Perjanjian = perjanjian,
        PihakPertama = pihakPertama,
        PihakKedua = pihakKedua,
        KetentuanKhusus = ketentuanKhusus,
        Lampiran = lampiran,
        LampiranPic = lampiranPicData,
        LampiranTindakanMedis = lampiranTindakanMedisData
    };
}

        private async Task<List<BabModel>> GetHierarchicalDataAsync(Expression<Func<JudulIsi, bool>> filter)
        {
            var allBab = await _context.JudulIsi.Where(filter).OrderBy(j => j.UrutanTampil).ToListAsync();
            if (!allBab.Any()) return new List<BabModel>();

            var allBabIds = allBab.Select(b => b.Id).ToList();

            var allSubBab = await _context.SubBabKetentuanKhusus
                .Where(s => allBabIds.Contains(s.IdJudul))
                .OrderBy(s => s.UrutanTampil).ToListAsync();
            
            var allSubBabIds = allSubBab.Select(s => s.Id).ToList();

            var allPoin = await _context.PoinKetentuanKhusus
                .Where(p => allSubBabIds.Contains(p.IdSubBab))
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