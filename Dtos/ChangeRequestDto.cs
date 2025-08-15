using pdfquestAPI.Models;
using System.Collections.Generic;

namespace pdfquestAPI.Dtos
{
    // DTO untuk body request saat membuat sesi perubahan baru
    public class ChangeRequestDto
    {
        public required string DiajukanOleh { get; set; }
        public string? Deskripsi { get; set; }
        public List<ChangeRequestItemDto> Items { get; set; } = new();
    }

    // DTO untuk setiap item perubahan di dalam body request
    public class ChangeRequestItemDto
    {
        public ChangeActionType ActionType { get; set; }
        public int? TargetKontenId { get; set; }
        public string? KontenBaru { get; set; }
        public string? LevelType { get; set; }
        public int? ParentId { get; set; }
        public string? AlasanPerubahan { get; set; }
        public int? UrutanTampilBaru { get; set; }
    }
}