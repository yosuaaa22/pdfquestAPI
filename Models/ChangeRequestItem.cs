using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdfquestAPI.Models
{
    public enum ChangeActionType
    {
        CREATE,
        UPDATE,
        DELETE
    }

    [Table("ChangeRequestItems")]
    public class ChangeRequestItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ChangeRequestId { get; set; }

        [Required]
        public ChangeActionType ActionType { get; set; } // CREATE, UPDATE, DELETE

        // ID dari konten yang akan diubah/dihapus. Null jika ActionType = CREATE.
        public int? TargetKontenId { get; set; }

        // Untuk CREATE & UPDATE: Konten baru.
        public string? KontenBaru { get; set; }

        // Untuk CREATE: Tipe level (Judul, SubBab, Poin).
        public string? LevelType { get; set; }

        // Untuk CREATE: ID parent dari konten baru.
        public int? ParentId { get; set; }

        public string? AlasanPerubahan { get; set; }

        // Navigation Property
        [ForeignKey("ChangeRequestId")]
        public virtual ChangeRequest ChangeRequest { get; set; }
    }
}