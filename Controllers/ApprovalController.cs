// In Controllers/ApprovalController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // <-- Tambahkan ini
using pdfquestAPI.Data; // <-- Tambahkan ini
using pdfquestAPI.Documents; // <-- Tambahkan ini
using pdfquestAPI.Interfaces; // <-- Tambahkan ini
using pdfquestAPI.Models; // <-- Tambahkan ini
using pdfquestAPI.Repositories; // <-- Tambahkan ini
using QuestPDF.Fluent; // <-- Tambahkan ini untuk .GeneratePdf()
using System; // <-- Tambahkan ini jika belum ada
using System.Linq; // <-- Tambahkan ini jika belum ada
using System.Threading.Tasks; // <-- Tambahkan ini jika belum ada

[ApiController]
[Route("api/approval")]
public class ApprovalController : ControllerBase
{
    private readonly PerjanjianKontenRepository _repository;
    private readonly ApplicationDbContext _context; // Untuk simplicity, kita akses DbContext langsung
    private readonly IChangeLogService _logService;

    public ApprovalController(PerjanjianKontenRepository repository, ApplicationDbContext context, IChangeLogService logService)
    {
        _repository = repository;
        _context = context;
        _logService = logService;
    }

    /// <summary>
    /// Membuat snapshot PDF dari editan terkini dan mengajukannya untuk persetujuan.
    /// </summary>
    [HttpPost("submit/{perjanjianId}")]
    public async Task<IActionResult> SubmitForApproval(int perjanjianId)
    {
        // 1. Generate PDF dari state saat ini (logika yang sama dengan endpoint preview)
        var perjanjianModel = _repository.GetPerjanjianModelKustom(perjanjianId);
        if (perjanjianModel == null) return NotFound();
        
        var document = new pdfquestAPI.Documents.PerjanjianDocument(perjanjianModel);
        byte[] pdfBytes = document.GeneratePdf();

        // 2. Buat entri baru di tabel PermintaanPersetujuan
        var approvalRequest = new PermintaanPersetujuan
        {
            PerjanjianId = perjanjianId,
            PdfSnapshot = pdfBytes,
            Status = "Pending",
            DiajukanOleh = "editor_user" // Ganti dengan user yang login
        };
        _context.PermintaanPersetujuan.Add(approvalRequest);
        await _context.SaveChangesAsync();

        // 3. Catat ke riwayat
        await _logService.LogAsync(perjanjianId, "editor_user", "SUBMIT_APPROVAL", $"Mengajukan persetujuan untuk versi editan terbaru.");

        return Ok(new { message = "PDF berhasil diajukan untuk persetujuan.", approvalId = approvalRequest.Id });
    }

    /// <summary>
    /// Mengambil daftar semua PDF yang menunggu persetujuan.
    /// </summary>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingApprovals()
    {
        var pending = await _context.PermintaanPersetujuan
            .Where(p => p.Status == "Pending")
            .Select(p => new { p.Id, p.PerjanjianId, p.DiajukanOleh, p.TanggalDiajukan })
            .ToListAsync();
        return Ok(pending);
    }
    
    /// <summary>
    /// Mengunduh file PDF dari sebuah permintaan persetujuan untuk direview.
    /// </summary>
    [HttpGet("{approvalId}/pdf")]
    public async Task<IActionResult> DownloadApprovalPdf(int approvalId)
    {
        var request = await _context.PermintaanPersetujuan.FindAsync(approvalId);
        if (request == null) return NotFound();

        return File(request.PdfSnapshot, "application/pdf", $"REVIEW_PKS_{request.PerjanjianId}_{request.TanggalDiajukan:yyyyMMdd}.pdf");
    }

    /// <summary>
    /// Memberikan persetujuan atau menolak sebuah permintaan.
    /// </summary>
    [HttpPost("review/{approvalId}")]
    public async Task<IActionResult> ReviewRequest(int approvalId, [FromBody] ReviewDto review)
    {
        var request = await _context.PermintaanPersetujuan.FindAsync(approvalId);
        if (request == null) return NotFound();
        if (request.Status != "Pending") return BadRequest("Permintaan ini sudah direview.");

        request.Status = review.IsApproved ? "Approved" : "Rejected";
        request.DireviewOleh = "approver_user"; // Ganti dengan user yang login
        request.TanggalDireview = DateTime.Now;
        request.CatatanReview = review.Catatan;
        
        await _context.SaveChangesAsync();

        // Catat ke riwayat
        string aksi = review.IsApproved ? "APPROVE" : "REJECT";
        await _logService.LogAsync(request.PerjanjianId, "approver_user", aksi, $"Review permintaan persetujuan ID {approvalId}. Catatan: {review.Catatan}");

        return Ok(new { message = $"Permintaan berhasil di-{request.Status.ToLower()}." });
    }
}

public class ReviewDto
{
    public bool IsApproved { get; set; }
    public string? Catatan { get; set; } // <-- Ubah string menjadi string?
}