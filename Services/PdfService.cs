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
    // Menggunakan Include dan ThenInclude adalah cara paling efisien dan direkomendasikan
    var babDataFromDb = await _context.JudulIsi
        .AsNoTracking() // Baik untuk performa karena data ini hanya untuk dibaca
        .Where(filter)
        .Include(j => j.SubBab)
            .ThenInclude(sb => sb.Poin) // <-- EF akan mengambil semua data Poin yang terkait
        .OrderBy(j => j.UrutanTampil)
        .ToListAsync();

    // Sekarang kita hanya perlu memetakan (mapping) dari model database ke model dokumen
    var babList = babDataFromDb.Select(babDb => new BabModel
    {
        JudulTeks = babDb.JudulTeks ?? string.Empty,
        UrutanTampil = babDb.UrutanTampil,
        SubBab = babDb.SubBab.OrderBy(sb => sb.UrutanTampil).Select(sbDb => new SubBabModel
        {
            Konten = sbDb.Konten ?? string.Empty,
            UrutanTampil = sbDb.UrutanTampil,
            // Cukup petakan list datar yang sudah diambil oleh ThenInclude
            Poin = sbDb.Poin.OrderBy(p => p.UrutanTampil).Select(pDb => new PoinModel
            {
                Id = pDb.Id,
                ParentId = pDb.Parent, // Pastikan nama kolom 'Parent' sudah benar
                TeksPoin = pDb.TeksPoin ?? string.Empty,
                UrutanTampil = pDb.UrutanTampil
                // Properti SubPoin tidak ada di sini, karena kita menggunakan list datar
            }).ToList()
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