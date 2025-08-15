using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Data;
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

        /// <summary>
        /// Generate PDF agreement bytes for a given provider (id_penyedia).
        /// </summary>
        public async Task<byte[]> GeneratePerjanjianPdfAsync(int id_penyedia)
        {
            var model = await BuildPerjanjianModelAsync(id_penyedia);
            var document = new PerjanjianDocument(model);
            return document.GeneratePdf();
        }

        /// <summary>
        /// Build the document model for the agreement based on id_penyedia.
        /// - Retrieves specific provider data
        /// - Loads master template sections (ketentuan khusus and lampiran structure)
        /// - Loads provider-specific table data (PIC and Tindakan Medis)
        /// </summary>
        private async Task<PerjanjianDocumentModel> BuildPerjanjianModelAsync(int id_penyedia)
        {
            // Provider (pihak kedua)
            var pihakKedua = await _unitOfWork.PenyediaLayanan.GetByIdAsync(id_penyedia)
                ?? throw new KeyNotFoundException($"Penyedia Layanan dengan ID {id_penyedia} tidak ditemukan.");

            // Perjanjian record that references the provider
            var perjanjian = await _context.Perjanjian.FirstOrDefaultAsync(p => p.IdPenyedia == id_penyedia)
                ?? throw new KeyNotFoundException($"Perjanjian untuk Penyedia Layanan ID {id_penyedia} tidak ditemukan.");

            // Pihak pertama (diasumsikan hanya satu entri)
            var pihakPertama = (await _unitOfWork.PihakPertama.GetAllAsync()).FirstOrDefault()
                ?? throw new KeyNotFoundException("Data Pihak Pertama tidak ditemukan.");

            // Ambil struktur teks dari master template (id_penyedia == null)
            // - Ketentuan khusus: semua judul yang bukan Lampiran
            var ketentuanKhusus = await GetHierarchicalDataAsync(p => p.IdPenyedia == null && (p.JudulTeks == null || !p.JudulTeks.StartsWith("LAMPIRAN")));

            // - Lampiran: struktur lampiran (judul yang berawalan "LAMPIRAN")
            var lampiran = await GetHierarchicalDataAsync(p => p.IdPenyedia == null && p.JudulTeks != null && p.JudulTeks.StartsWith("LAMPIRAN"));

            // Ambil data tabel yang spesifik untuk penyedia ini
            var lampiranPicData = await _context.Lampiran_PIC
                .Where(p => p.IdPenyedia == id_penyedia)
                .Select(p => new PicModel
                {
                    JenisPIC = p.JenisPIC,
                    NamaPIC = p.NamaPIC,
                    NomorTelepon = p.NomorTelepon,
                    AlamatEmail = p.AlamatEmail
                })
                .ToListAsync();

            var lampiranTindakanMedisData = await _context.Lampiran_TindakanMedis
                .Where(t => t.IdPenyedia == id_penyedia)
                .Select(t => new TindakanMedisModel
                {
                    JenisTindakanMedis = t.JenisTindakanMedis,
                    Tarif = t.Tarif,
                    Keterangan = t.Keterangan
                })
                .ToListAsync();

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

        /// <summary>
        /// Retrieve hierarchical Bab -> SubBab -> Poin data using eager loading.
        /// Returns mapped domain models suitable for document generation.
        /// </summary>
        private async Task<List<BabModel>> GetHierarchicalDataAsync(Expression<Func<JudulIsi, bool>> filter)
        {
            // Gunakan AsNoTracking karena data hanya dibaca untuk pembuatan dokumen
            var babDataFromDb = await _context.JudulIsi
                .AsNoTracking()
                .Where(filter)
                .Include(j => j.SubBab)
                    .ThenInclude(sb => sb.Poin)
                .OrderBy(j => j.UrutanTampil)
                .ToListAsync();

            // Map entity -> document model, menjaga urutan tampil
            var babList = babDataFromDb.Select(babDb => new BabModel
            {
                JudulTeks = babDb.JudulTeks ?? string.Empty,
                UrutanTampil = babDb.UrutanTampil,
                SubBab = babDb.SubBab
                    .OrderBy(sb => sb.UrutanTampil)
                    .Select(sbDb => new SubBabModel
                    {
                        Konten = sbDb.Konten ?? string.Empty,
                        UrutanTampil = sbDb.UrutanTampil,
                        Poin = sbDb.Poin
                            .OrderBy(p => p.UrutanTampil)
                            .Select(pDb => new PoinModel
                            {
                                Id = pDb.Id,
                                ParentId = pDb.Parent,
                                TeksPoin = pDb.TeksPoin ?? string.Empty,
                                UrutanTampil = pDb.UrutanTampil
                                // SubPoin dibangun terpisah jika diperlukan
                            }).ToList()
                    }).ToList()
            }).ToList();

            return babList;
        }

        /// <summary>
        /// Build nested point tree (Poin -> SubPoin) from a flat list.
        /// Digunakan jika struktur poin memiliki parent-child di dalam satu subBab.
        /// </summary>
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