using pdfquestAPI.Models;
using System;
using System.Collections.Generic;

namespace pdfquestAPI.Dtos
{
    // DTO untuk item respons
    public class ChangeRequestItemResponseDto
    {
        public int Id { get; set; }
        public ChangeActionType ActionType { get; set; }
        public int? TargetKontenId { get; set; }
        public string? KontenBaru { get; set; }
        public string? LevelType { get; set; }
        public int? ParentId { get; set; }
        public string? AlasanPerubahan { get; set; }
    }

    // DTO untuk respons request utama
    public class ChangeRequestResponseDto
    {
        public int Id { get; set; }
        public int PerjanjianId { get; set; }
        public DateTime TanggalRequest { get; set; }
        public required string DiajukanOleh { get; set; }
        public string? Deskripsi { get; set; }
        public required string Status { get; set; }
        public DateTime? TanggalDiputuskan { get; set; }
        public string? DiputuskanOleh { get; set; }
        public List<ChangeRequestItemResponseDto> Items { get; set; } = new();
    }
}