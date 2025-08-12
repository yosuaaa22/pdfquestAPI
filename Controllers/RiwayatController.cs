// In Controllers/RiwayatController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Data;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/history")]
public class RiwayatController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RiwayatController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Menampilkan semua riwayat perubahan untuk sebuah perjanjian.
    /// </summary>
    [HttpGet("{perjanjianId}")]
    public async Task<IActionResult> GetHistory(int perjanjianId)
    {
        var history = await _context.RiwayatPerubahan
            .Where(r => r.PerjanjianId == perjanjianId)
            .OrderByDescending(r => r.Timestamp)
            .Select(r => new { r.Timestamp, r.Pengguna, r.Aksi, r.DeskripsiPerubahan })
            .ToListAsync();
            
        if (!history.Any())
        {
            return NotFound($"Tidak ada riwayat ditemukan untuk Perjanjian ID {perjanjianId}.");
        }

        return Ok(history);
    }
}