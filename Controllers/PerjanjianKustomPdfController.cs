using Microsoft.AspNetCore.Mvc;
using pdfquestAPI.Repositories; // Pastikan namespace repository Anda sudah benar
using System;
using System.Threading.Tasks;
using pdfquestAPI.Documents;
using QuestPDF.Fluent;
using pdfquestAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using pdfquestAPI.Dtos;

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
        /// <summary>
        /// Menghasilkan PDF dari data perjanjian terkini yang telah disetujui dan diubah.
        /// </summary>
        [HttpGet("perjanjian/{id}/pdf/final")] // Ganti nama endpoint menjadi 'final' atau 'approved'
        public IActionResult GenerateFinalPerjanjianPdf(int id)
        {
            try
            {
                // Logika ini sekarang sudah BENAR, karena mengambil data terkini dari Perjanjian_Konten
                // yang telah diubah oleh ChangeRequestController.
                var perjanjianModel = _repository.GetPerjanjianModelKustom(id);
                if (perjanjianModel == null)
                {
                    return NotFound($"Data untuk membangun PDF perjanjian ID {id} tidak ditemukan.");
                }

                var document = new PerjanjianDocument(perjanjianModel);
                byte[] pdfBytes = document.GeneratePdf();
                string fileName = $"FINAL_PKS_{id}_{DateTime.Now:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Terjadi kesalahan internal saat membuat PDF final: " + ex.Message);
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
                // Panggil metode baru yang sudah kita buat
                var hasil = await _repository.CariSemuaKontenByKeywordsAsync(id, kataKunci);

                if (!hasil.Any())
                {
                    return NotFound(new { message = $"Tidak ada konten yang cocok dengan kata kunci '{kataKunci}' pada perjanjian ID {id}." });
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
                await _repository.UpdateKontenAsync(kontenId, updateDto);
                return Ok(new { message = "Konten berhasil diperbarui dan diurutkan ulang." });
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