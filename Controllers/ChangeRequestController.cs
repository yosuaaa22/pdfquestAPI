using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Data;
using pdfquestAPI.Dtos;
using pdfquestAPI.Models;
using pdfquestAPI.Repositories;

namespace pdfquestAPI.Controllers
{
    /// <summary>
    /// Controller untuk mengelola ChangeRequest (permintaan perubahan) pada perjanjian.
    /// </summary>
    [ApiController]
    [Route("api/changerequests")]
    public class ChangeRequestController : ControllerBase
    {
        #region Dependencies

        private readonly ApplicationDbContext _context;
        private readonly PerjanjianKontenRepository _kontenRepository;

        #endregion

        #region Constructor

        public ChangeRequestController(ApplicationDbContext context, PerjanjianKontenRepository kontenRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _kontenRepository = kontenRepository ?? throw new ArgumentNullException(nameof(kontenRepository));
        }

        #endregion

        #region Create Change Request

        /// <summary>
        /// Membuat sesi permintaan perubahan baru untuk sebuah perjanjian.
        /// Validasi sederhana dilakukan: dto dan item harus ada.
        /// </summary>
        [HttpPost("perjanjian/{perjanjianId}")]
        public async Task<ActionResult<ChangeRequestResponseDto>> CreateChangeRequest(int perjanjianId, [FromBody] ChangeRequestDto dto)
        {
            if (dto == null) return BadRequest("Request body tidak boleh kosong.");
            var items = dto.Items?.ToList();
            if (items == null || items.Count == 0) return BadRequest("Items tidak boleh kosong.");

            var perjanjianExists = await _context.Perjanjian.AnyAsync(p => p.IdPerjanjian == perjanjianId).ConfigureAwait(false);
            if (!perjanjianExists)
            {
                return NotFound($"Perjanjian dengan ID {perjanjianId} tidak ditemukan.");
            }

            var newRequest = new ChangeRequest
            {
                PerjanjianId = perjanjianId,
                DiajukanOleh = dto.DiajukanOleh,
                Deskripsi = dto.Deskripsi,
                Status = "Correction", // Status awal
                Items = new List<ChangeRequestItem>()
            };

            foreach (var i in items)
            {
                // Jika aksi UPDATE atau DELETE baca konten sebelumnya (jika ada target id)
                string? kontenSebelumnya = null;
                if ((i.ActionType == ChangeActionType.UPDATE || i.ActionType == ChangeActionType.DELETE) && i.TargetKontenId.HasValue)
                {
                    kontenSebelumnya = await _kontenRepository.GetKontenTextByIdAsync(i.TargetKontenId.Value).ConfigureAwait(false);
                }

                var itemEntity = new ChangeRequestItem
                {
                    ActionType = i.ActionType,
                    TargetKontenId = i.TargetKontenId,
                    KontenBaru = i.KontenBaru,
                    KontenSebelumnya = kontenSebelumnya,
                    LevelType = i.LevelType,
                    ParentId = i.ParentId,
                    UrutanTampilBaru = i.UrutanTampilBaru,
                    AlasanPerubahan = i.AlasanPerubahan
                };

                newRequest.Items.Add(itemEntity);
            }

            _context.ChangeRequests.Add(newRequest);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            var responseDto = MapToResponseDto(newRequest);

            // Mengembalikan lokasi resource yang baru dibuat (GetRequestById -> route param requestId)
            return CreatedAtAction(nameof(GetRequestById), new { requestId = newRequest.Id }, responseDto);
        }

        #endregion

        #region Get Request

        /// <summary>
        /// Mengambil detail sebuah permintaan perubahan beserta item-itemnya.
        /// </summary>
        [HttpGet("{requestId}")]
        public async Task<ActionResult<ChangeRequestResponseDto>> GetRequestById(int requestId)
        {
            var requestEntity = await _context.ChangeRequests
                .AsNoTracking()
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == requestId)
                .ConfigureAwait(false);

            if (requestEntity == null) return NotFound();

            var responseDto = MapToResponseDto(requestEntity);
            return Ok(responseDto);
        }

        #endregion

        #region Approve Request

        /// <summary>
        /// Menyetujui sebuah permintaan perubahan dan mengeksekusi semua item di dalamnya.
        /// Hanya request dengan status "Correction" yang dapat disetujui.
        /// </summary>
        [HttpPost("{requestId}/approve")]
        public async Task<IActionResult> ApproveRequest(int requestId, [FromQuery] string approverName)
        {
            var request = await _context.ChangeRequests
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == requestId)
                .ConfigureAwait(false);

            if (request == null) return NotFound("Request tidak ditemukan.");
            if (!string.Equals(request.Status, "Correction", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Hanya request dengan status 'Correction' yang bisa disetujui.");

            var items = request.Items?.ToList() ?? new List<ChangeRequestItem>();

            await using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                foreach (var item in items)
                {
                    switch (item.ActionType)
                    {
                        case ChangeActionType.CREATE:
                            if (!item.UrutanTampilBaru.HasValue)
                                throw new InvalidOperationException("UrutanTampilBaru harus ada untuk aksi CREATE.");
                            if (item.LevelType == null)
                                throw new InvalidOperationException("LevelType harus ada untuk aksi CREATE.");
                            if (item.KontenBaru == null)
                                throw new InvalidOperationException("KontenBaru harus ada untuk aksi CREATE.");

                            var createDto = new CreateKontenDto
                            {
                                IdPerjanjian = request.PerjanjianId,
                                ParentId = item.ParentId,
                                LevelType = item.LevelType ?? throw new InvalidOperationException("LevelType harus ada untuk aksi CREATE."),
                                Konten = item.KontenBaru ?? throw new InvalidOperationException("KontenBaru harus ada untuk aksi CREATE."),
                                UrutanTampil = item.UrutanTampilBaru.Value
                            };
                            await _kontenRepository.TambahDanUrutkanUlangKontenAsync(createDto).ConfigureAwait(false);
                            break;

                        case ChangeActionType.UPDATE:
                            if (!item.TargetKontenId.HasValue)
                                throw new InvalidOperationException("TargetKontenId harus ada untuk aksi UPDATE.");
                            if (!item.UrutanTampilBaru.HasValue)
                                throw new InvalidOperationException("UrutanTampilBaru harus ada untuk aksi UPDATE.");

                            var updateDto = new UpdateKontenDto
                            {
                                Konten = item.KontenBaru ?? throw new InvalidOperationException("KontenBaru harus ada untuk aksi UPDATE."),
                                UrutanTampil = item.UrutanTampilBaru.Value
                            };
                            await _kontenRepository.UpdateKontenAsync(item.TargetKontenId.Value, updateDto).ConfigureAwait(false);
                            break;

                        case ChangeActionType.DELETE:
                            if (!item.TargetKontenId.HasValue)
                                throw new InvalidOperationException("TargetKontenId harus ada untuk aksi DELETE.");
                            await _kontenRepository.HapusDanUrutkanUlangKontenAsync(item.TargetKontenId.Value, request.PerjanjianId).ConfigureAwait(false);
                            break;

                        default:
                            throw new InvalidOperationException($"Aksi tidak dikenali: {item.ActionType}");
                    }
                }

                request.Status = "Approved";
                request.DiputuskanOleh = approverName;
                request.TanggalDiputuskan = DateTime.UtcNow;

                await _context.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                return Ok(new { message = "Permintaan berhasil disetujui dan semua perubahan telah dieksekusi." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                // Jangan membocorkan detail sensitif, hanya kembalikan pesan umum + exception message untuk debugging
                return StatusCode(500, $"Terjadi kesalahan saat mengeksekusi perubahan: {ex.Message}");
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Map entity ChangeRequest ke DTO response.
        /// Dipisahkan agar mapping konsisten dan mudah dibaca.
        /// </summary>
        private static ChangeRequestResponseDto MapToResponseDto(ChangeRequest requestEntity)
        {
            return new ChangeRequestResponseDto
            {
                Id = requestEntity.Id,
                PerjanjianId = requestEntity.PerjanjianId,
                TanggalRequest = requestEntity.TanggalRequest,
                DiajukanOleh = requestEntity.DiajukanOleh,
                Deskripsi = requestEntity.Deskripsi,
                Status = requestEntity.Status,
                TanggalDiputuskan = requestEntity.TanggalDiputuskan,
                DiputuskanOleh = requestEntity.DiputuskanOleh,
                Items = (requestEntity.Items ?? Enumerable.Empty<ChangeRequestItem>())
                    .Select(itemEntity => new ChangeRequestItemResponseDto
                    {
                        Id = itemEntity.Id,
                        ActionType = itemEntity.ActionType,
                        TargetKontenId = itemEntity.TargetKontenId,
                        KontenSebelumnya = itemEntity.KontenSebelumnya,
                        KontenBaru = itemEntity.KontenBaru,
                        LevelType = itemEntity.LevelType,
                        ParentId = itemEntity.ParentId,
                        UrutanTampilBaru = itemEntity.UrutanTampilBaru,
                        AlasanPerubahan = itemEntity.AlasanPerubahan
                    })
                    .ToList()
            };
        }

        #endregion
    }
}