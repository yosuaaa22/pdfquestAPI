// Mengimpor namespace yang dibutuhkan dari .NET dan proyek Anda
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using pdfquestAPI.Documents;
using pdfquestAPI.Documents.Models;
using pdfquestAPI.Repositories;
using QuestPDF.Fluent;


namespace pdfquestAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class PerjanjianKustomPdfController : ControllerBase
    {
        private readonly PerjanjianKontenRepository _repository;

        public PerjanjianKustomPdfController(PerjanjianKontenRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("perjanjian/{id}/pdf/kustom")]
        public IActionResult GenerateKustomPdf(int id)
        {
            try
            {
                var perjanjianModel = _repository.GetPerjanjianModelKustom(id);
                if (perjanjianModel == null)
                {
                    return NotFound($"Data perjanjian dengan ID {id} tidak ditemukan.");
                }
                var document = new PerjanjianDocument(perjanjianModel);
                byte[] pdfBytes = document.GeneratePdf();
                string fileName = $"Perjanjian_Kustom_{id}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Terjadi kesalahan internal saat membuat PDF: " + ex.Message);
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