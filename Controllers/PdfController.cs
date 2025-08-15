using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using pdfquestAPI.Interfaces;

namespace pdfquestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        #region Fields
        private readonly IPdfService _pdfService;
        private readonly ILogger<PdfController> _logger;
        #endregion

        #region Constructor
        public PdfController(IPdfService pdfService, ILogger<PdfController> logger)
        {
            // Pastikan dependency tidak null agar lebih mudah dideteksi saat startup
            _pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Endpoints

        /// <summary>
        /// Menghasilkan file PDF untuk perjanjian berdasarkan ID.
        /// - 200: Mengembalikan file PDF (application/pdf).
        /// - 400: perjanjianId tidak valid.
        /// - 404: Data/perjanjian tidak ditemukan.
        /// - 500: Kesalahan internal saat membuat PDF.
        /// </summary>
        /// <param name="perjanjianId">ID dari perjanjian yang akan dibuatkan PDF-nya.</param>
        [HttpGet("perjanjian/{perjanjianId}")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GeneratePerjanjianPdf(int perjanjianId)
        {
            // Validasi parameter awal agar tidak memanggil service dengan input tidak valid
            if (perjanjianId <= 0)
            {
                _logger.LogWarning("Permintaan PDF diterima dengan perjanjianId tidak valid: {PerjanjianId}", perjanjianId);
                return BadRequest(new { message = "perjanjianId harus lebih besar dari 0." });
            }

            try
            {
                // Panggil service untuk membuat PDF (mengembalikan byte[])
                var pdfBytes = await _pdfService.GeneratePerjanjianPdfAsync(perjanjianId);

                // Jika service mengembalikan null atau array kosong, anggap tidak ditemukan
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    _logger.LogWarning("PDF tidak ditemukan atau kosong untuk perjanjianId: {PerjanjianId}", perjanjianId);
                    return NotFound(new { message = "PDF tidak ditemukan untuk perjanjian tersebut." });
                }

                // Buat nama file yang informatif dan aman
                string fileName = $"PKS_Perjanjian_{perjanjianId}_{DateTime.UtcNow:yyyyMMdd}.pdf";

                // Kembalikan file PDF ke client
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (KeyNotFoundException ex)
            {
                // Tangani kasus service melaporkan bahwa data tidak ditemukan
                _logger.LogWarning(ex, "Tidak ditemukan data untuk perjanjianId: {PerjanjianId}", perjanjianId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Tangani error tak terduga dan log untuk investigasi
                _logger.LogError(ex, "Error saat membuat PDF untuk perjanjianId: {PerjanjianId}", perjanjianId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Terjadi kesalahan internal saat membuat PDF.");
            }
        }

        #endregion
    }
}
