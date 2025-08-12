using Microsoft.AspNetCore.Mvc;
using pdfquestAPI.Interfaces;

namespace pdfquestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly IPdfService _pdfService;
        private readonly ILogger<PdfController> _logger;

        public PdfController(IPdfService pdfService, ILogger<PdfController> logger)
        {
            _pdfService = pdfService;
            _logger = logger;
        }

        /// <summary>
        /// Menghasilkan dan mengunduh dokumen Perjanjian Kerja Sama dalam format PDF.
        /// </summary>
        /// <param name="perjanjianId">ID dari perjanjian yang akan dibuatkan PDF-nya.</param>
        /// <returns>File PDF.</returns>
        [HttpGet("perjanjian/{perjanjianId}")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GeneratePerjanjianPdf(int perjanjianId)
        {
            try
            {
                var pdfBytes = await _pdfService.GeneratePerjanjianPdfAsync(perjanjianId);
                string fileName = $"PKS_Perjanjian_{perjanjianId}_{DateTime.Now:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Terjadi error saat membuat PDF untuk perjanjian ID: {perjanjianId}");
                return StatusCode(500, "Terjadi kesalahan internal saat membuat PDF.");
            }
        }

        
    }
}
