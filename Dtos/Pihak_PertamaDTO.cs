using System.ComponentModel.DataAnnotations;

 public class CreatePihakPertamaDto
    {
        [Required]
        public required string NomorNibPihakPertama { get; set; }
        [Required]
        public required string NamaPerwakilanPihakPertama { get; set; }
        [Required]
        public required  string JabatanPerwakilanPihakPertama { get; set; }
    }