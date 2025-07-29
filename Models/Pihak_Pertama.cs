using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


[Table("Pihak_Pertama")]
    public class PihakPertama
    {
        [Key]
        [Column("id_pihak_pertama")]
        public int IdPihakPertama { get; set; }

        [Required]
        [Column("nomor_nib_pihak_pertama")]
        public string NomorNibPihakPertama { get; set; }

        [Required]
        [Column("nama_perwakilan_pihak_pertama")]
        public string NamaPerwakilanPihakPertama { get; set; }

        [Required]
        [Column("jabatan_perwakilan_pihak_pertama")]
        public string JabatanPerwakilanPihakPertama { get; set; }
    }
