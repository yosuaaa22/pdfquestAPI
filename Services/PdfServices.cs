using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Interfaces;
using pdfquestAPI.Documents;
using pdfquestAPI.Models.DTOs;
using QuestPDF.Fluent;

namespace pdfquestAPI.Services
{
    public class PdfService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Service ini "memegang" kontrak Manajer Arsip (IUnitOfWork), bukan DbContext.
        public PdfService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<byte[]?> GeneratePerjanjianPdfAsync(int perjanjianId)
        {
            // --- 1. Meminta semua dokumen mentah dari Manajer Arsip ---
            var perjanjian = await _unitOfWork.Perjanjian.GetByIdAsync(perjanjianId);
            if (perjanjian == null)
            {
                // Jika data utama tidak ada, proses tidak bisa dilanjutkan.
                return null;
            }

            // Ambil semua data pendukung yang terkait.
            var allJudul = await _unitOfWork.JudulIsi.GetAllAsync();
            var allSubBab = await _unitOfWork.SubBabKetentuanKhusus.GetAllAsync();
            var allPoin = await _unitOfWork.PoinKetentuanKhusus.GetAllAsync();
            // Anda juga bisa mengambil data PenyediaLayanan dan PihakPertama di sini jika diperlukan.

            // --- 2. Merakit dokumen mentah menjadi satu paket DTO yang rapi ---
            // Ini adalah bagian penting dari logika bisnis.
            var perjanjianDto = new PerjanjianDto
            {
                IdPerjanjian = perjanjian.IdPerjanjian,
                NoPtInhealth = perjanjian.NoPtInhealth,
                NoPtPihakKedua = perjanjian.NoPtPihakKedua,
                TanggalTandaTangan = perjanjian.TanggalTandaTangan,
                NomorBeritaAcara = perjanjian.NomorBeritaAcara,

                // Di sini kita membangun struktur data bersarang (nested) yang dinamis.
                DaftarIsi = allJudul
                    .OrderBy(j => j.UrutanTampil)
                    .Select(judul => new JudulIsiDto
                    {
                        Id = judul.Id,
                        JudulTeks = judul.JudulTeks,
                        UrutanTampil = judul.UrutanTampil,
                        SubBab = allSubBab
                            .Where(sb => sb.IdJudul == judul.Id)
                            .OrderBy(sb => sb.UrutanTampil)
                            .Select(subBab => new SubBabKetentuanKhususDto
                            {
                                Id = subBab.Id,
                                Konten = subBab.Konten,
                                UrutanTampil = subBab.UrutanTampil,
                                Poin = allPoin
                                    .Where(p => p.IdSubBab == subBab.Id)
                                    .OrderBy(p => p.UrutanTampil)
                                    .Select(poin => new PoinKetentuanKhususDto
                                    {
                                        Id = poin.Id,
                                        TeksPoin = poin.TeksPoin,
                                        Parent = poin.Parent,
                                        UrutanTampil = poin.UrutanTampil
                                    }).ToList()
                            }).ToList()
                    }).ToList()
            };

            // --- 3. Memberikan paket DTO ke mesin cetak (QuestPDF) ---
            var document = new PerjanjianDocument(perjanjianDto);
            byte[] pdfBytes = document.GeneratePdf();

            return pdfBytes;
        }
    }
}