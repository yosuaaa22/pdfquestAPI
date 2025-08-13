using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Data;
using pdfquestAPI.Models;
using pdfquestAPI.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using pdfquestAPI.Dtos;

[ApiController]
[Route("api/changerequests")]
public class ChangeRequestController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly PerjanjianKontenRepository _kontenRepository;

    public ChangeRequestController(ApplicationDbContext context, PerjanjianKontenRepository kontenRepository)
    {
        _context = context;
        _kontenRepository = kontenRepository;
    }

    /// <summary>
    /// Membuat sesi permintaan perubahan baru untuk sebuah perjanjian.
    /// </summary>
    [HttpPost("perjanjian/{perjanjianId}")]
    public async Task<IActionResult> CreateChangeRequest(int perjanjianId, [FromBody] ChangeRequestDto dto)
    {
        var perjanjianExists = await _context.Perjanjian.AnyAsync(p => p.IdPerjanjian == perjanjianId);
        if (!perjanjianExists)
        {
            return NotFound($"Perjanjian dengan ID {perjanjianId} tidak ditemukan.");
        }

        var newRequest = new ChangeRequest
        {
            PerjanjianId = perjanjianId,
            DiajukanOleh = dto.DiajukanOleh,
            Deskripsi = dto.Deskripsi,
            Status = "Pending",
            Items = dto.Items.Select(i => new ChangeRequestItem
            {
                ActionType = i.ActionType,
                TargetKontenId = i.TargetKontenId,
                KontenBaru = i.KontenBaru,
                LevelType = i.LevelType,
                ParentId = i.ParentId,
                AlasanPerubahan = i.AlasanPerubahan
            }).ToList()

        };
        _context.ChangeRequests.Add(newRequest);
        await _context.SaveChangesAsync();

        var responseDto = new ChangeRequestResponseDto
        {
             Id = newRequest.Id,
             PerjanjianId = newRequest.PerjanjianId,
             TanggalRequest = newRequest.TanggalRequest,
             DiajukanOleh = newRequest.DiajukanOleh,
             Deskripsi = newRequest.Deskripsi,
             Status = newRequest.Status,
             Items = newRequest.Items.Select(itemEntity => new ChangeRequestItemResponseDto
                {
                    Id = itemEntity.Id,
                    ActionType = itemEntity.ActionType,
                    TargetKontenId = itemEntity.TargetKontenId,
                    KontenBaru = itemEntity.KontenBaru,
                    LevelType = itemEntity.LevelType,
                    ParentId = itemEntity.ParentId,
                    AlasanPerubahan = itemEntity.AlasanPerubahan
                }).ToList()
        };

        return CreatedAtAction(nameof(GetRequestById), new { requestId = newRequest.Id }, responseDto);
    }

    /// <summary>
    /// Mengambil detail sebuah permintaan perubahan beserta item-itemnya.
    /// </summary>
    [HttpGet("{requestId}")]
    public async Task<IActionResult> GetRequestById(int requestId)
    {
        var requestEntity = await _context.ChangeRequests
            .AsNoTracking()
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (requestEntity == null) return NotFound();

        var responseDto = new ChangeRequestResponseDto
        {
            Id = requestEntity.Id,
            PerjanjianId = requestEntity.PerjanjianId,
            TanggalRequest = requestEntity.TanggalRequest,
            DiajukanOleh = requestEntity.DiajukanOleh,
            Deskripsi = requestEntity.Deskripsi,
            Status = requestEntity.Status,
            TanggalDiputuskan = requestEntity.TanggalDiputuskan,
            DiputuskanOleh = requestEntity.DiputuskanOleh,
            Items = requestEntity.Items.Select(itemEntity => new ChangeRequestItemResponseDto
            {
                Id = itemEntity.Id,
                ActionType = itemEntity.ActionType,
                TargetKontenId = itemEntity.TargetKontenId,
                KontenBaru = itemEntity.KontenBaru,
                LevelType = itemEntity.LevelType,
                ParentId = itemEntity.ParentId,
                AlasanPerubahan = itemEntity.AlasanPerubahan
            }).ToList()
        };

        return Ok(responseDto);
    }

    /// <summary>
    /// Menyetujui sebuah permintaan perubahan dan MENGEKSEKUSI semua item di dalamnya.
    /// </summary>
    [HttpPost("{requestId}/approve")]
    public async Task<IActionResult> ApproveRequest(int requestId, [FromQuery] string approverName)
    {
        var request = await _context.ChangeRequests
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request == null) return NotFound("Request tidak ditemukan.");
        if (request.Status != "Pending") return BadRequest("Request ini sudah tidak pending lagi.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var item in request.Items)
            {
                switch (item.ActionType)
                {
                    case ChangeActionType.CREATE:
                        var createDto = new CreateKontenDto
                        {
                            IdPerjanjian = request.PerjanjianId,
                            ParentId = item.ParentId,
                            LevelType = item.LevelType,
                            Konten = item.KontenBaru
                        };
                        await _kontenRepository.TambahDanUrutkanUlangKontenAsync(createDto);
                        break;

                    case ChangeActionType.UPDATE:
                        if (!item.TargetKontenId.HasValue)
                            throw new InvalidOperationException("TargetKontenId harus ada untuk aksi UPDATE.");
                        await _kontenRepository.UpdateKontenAsync(item.TargetKontenId.Value, item.KontenBaru);
                        break;

                    case ChangeActionType.DELETE:
                        if (!item.TargetKontenId.HasValue)
                            throw new InvalidOperationException("TargetKontenId harus ada untuk aksi DELETE.");
                        await _kontenRepository.HapusDanUrutkanUlangKontenAsync(item.TargetKontenId.Value, request.PerjanjianId);
                        break;
                }
            }

            request.Status = "Approved";
            request.DiputuskanOleh = approverName;
            request.TanggalDiputuskan = DateTime.Now;
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { message = "Permintaan berhasil disetujui dan semua perubahan telah dieksekusi." });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Terjadi kesalahan saat mengeksekusi perubahan: {ex.Message}");
        }
    }
}