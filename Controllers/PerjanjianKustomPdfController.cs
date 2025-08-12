using Microsoft.AspNetCore.Mvc;
using pdfquestAPI.Repositories; // Pastikan namespace repository Anda sudah benar
using System;
using System.Threading.Tasks;
using pdfquestAPI.Documents;
using QuestPDF.Fluent;
using pdfquestAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace pdfquestAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class PerjanjianKustomPdfController : ControllerBase
    {
        // Kembalikan dependency ke PerjanjianKontenRepository
        private readonly PerjanjianKontenRepository _repository;
        private readonly ApplicationDbContext _context;
        // Kembalikan constructor ke versi aslinya
        public PerjanjianKustomPdfController(PerjanjianKontenRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context; // Sekarang baris ini valid karena 'context' sudah ada.
        }

        // Endpoint untuk generate PDF kustom sudah DIHAPUS dari sini

        // Endpoint-endpoint di bawah ini sekarang akan berfungsi kembali
        // karena _repository sudah didefinisikan dengan benar.

        [HttpGet("perjanjian/{id}/pdf/kustom")]
        public async Task<IActionResult> GenerateApprovedKustomPdf(int id)
        {
            try
            {
                var latestApproved = await _context.PermintaanPersetujuan
                    .Where(p => p.PerjanjianId == id && p.Status == "Approved")
                    .OrderByDescending(p => p.TanggalDireview)
                    .FirstOrDefaultAsync();

                if (latestApproved == null)
                {
                    return NotFound($"Tidak ada versi PDF yang disetujui untuk Perjanjian ID {id}. Silakan ajukan persetujuan terlebih dahulu.");
                }

                // --- INI ADALAH KUNCI PERBAIKANNYA ---
                // Jika TanggalDireview null, kode ini akan memakai TanggalDiajukan
                // sehingga tidak akan terjadi error.
                var tanggalUntukNamaFile = latestApproved.TanggalDireview ?? latestApproved.TanggalDiajukan;

                string fileName = $"PKS_KUSTOM_APPROVED_{id}_{tanggalUntukNamaFile:yyyyMMdd}.pdf";

                if (latestApproved.PdfSnapshot == null || latestApproved.PdfSnapshot.Length == 0)
                {
                    return StatusCode(500, "Data PDF snapshot pada persetujuan yang ditemukan ternyata kosong (null).");
                }

                return File(latestApproved.PdfSnapshot, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Terjadi kesalahan internal saat mengambil PDF yang disetujui: " + ex.Message);
            }
        }


        /// <summary>
        /// BARU: Endpoint untuk melihat preview PDF dari editan yang sedang berjalan.
        /// </summary>
        [HttpGet("perjanjian/{id}/pdf/preview")]
    public IActionResult GeneratePreviewPdf(int id)
    {
        // Logika ini sama persis dengan endpoint /kustom Anda yang lama
        try
        {
            var perjanjianModel = _repository.GetPerjanjianModelKustom(id);
            if (perjanjianModel == null)
            {
                return NotFound($"Data perjanjian kustom dengan ID {id} tidak ditemukan.");
            }

            var document = new PerjanjianDocument(perjanjianModel);
            byte[] pdfBytes = document.GeneratePdf();
            string fileName = $"PREVIEW_Perjanjian_{id}_{DateTime.Now:yyyyMMdd}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Terjadi kesalahan internal saat membuat preview PDF: " + ex.Message);
        }
    }
        [HttpGet("perjanjian/{id}/konten/struktur")]
        public async Task<IActionResult> GetKontenByKeyword(int id, [FromQuery] string kataKunci)
        {
            if (string.IsNullOrWhiteSpace(kataKunci))
            {
                return BadRequest("Parameter 'kataKunci' tidak boleh kosong.");
            }

            try
            {
                var hasil = await _repository.CariKontenByKeywordAsync(id, kataKunci);

                if (hasil == null)
                {
                    return NotFound(new { message = $"Konten dengan kata kunci '{kataKunci}' pada perjanjian ID {id} tidak ditemukan." });
                }

                return Ok(hasil);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan internal: {ex.Message}");
            }
        }

        [HttpPost("perjanjian/konten")]
        public async Task<IActionResult> TambahKonten([FromBody] CreateKontenDto createDto)
        {
            if (createDto == null || string.IsNullOrEmpty(createDto.Konten))
            {
                return BadRequest("Data konten tidak boleh kosong.");
            }
            try
            {
                await _repository.TambahDanUrutkanUlangKontenAsync(createDto);
                return Ok(new { message = "Konten berhasil ditambahkan dan diurutkan ulang." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gagal menambah konten: {ex.Message}");
            }
        }

        [HttpPut("perjanjian/konten/{kontenId}")]
        public async Task<IActionResult> UpdateKonten(int kontenId, [FromBody] UpdateKontenDto updateDto)
        {
            if (updateDto == null || string.IsNullOrEmpty(updateDto.Konten))
            {
                return BadRequest("Konten tidak boleh kosong.");
            }
            try
            {
                await _repository.UpdateKontenAsync(kontenId, updateDto.Konten);
                return Ok(new { message = "Konten berhasil diperbarui." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gagal memperbarui konten: {ex.Message}");
            }
        }

        [HttpDelete("perjanjian/{perjanjianId}/konten/{kontenId}")]
        public async Task<IActionResult> HapusKonten(int perjanjianId, int kontenId)
        {
            try
            {
                await _repository.HapusDanUrutkanUlangKontenAsync(kontenId, perjanjianId);
                return Ok(new { message = "Konten berhasil dihapus dan diurutkan ulang." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gagal menghapus konten: {ex.Message}");
            }
        }
    }
}