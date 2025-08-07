// Mengimpor namespace yang dibutuhkan dari .NET dan proyek Anda
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using pdfquestAPI.Documents;
using pdfquestAPI.Documents.Models;
using pdfquestAPI.Models;
using pdfquestAPI.Repositories;

// =================================================================
// BAGIAN 1: REPOSITORY UNTUK MENGAMBIL DAN MENGOLAH DATA KONTEN (VERSI LENGKAP)
// =================================================================
// Kelas ini adalah "jembatan" antara database dan generator PDF Anda.

public class PerjanjianKontenRepository
{
    private readonly string _connectionString;

    public PerjanjianKontenRepository(IConfiguration configuration)
    {
        // Pastikan nama koneksi di appsettings.json sudah sesuai
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    /// <summary>
    /// DTO (Data Transfer Object) internal untuk menampung data mentah dari tabel hirarkis.
    /// </summary>
    private class PerjanjianKontenData
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string LevelType { get; set; }
        public string Konten { get; set; }
        public int UrutanTampil { get; set; }
    }

    /// <summary>
    /// Metode utama untuk mengambil semua data dan menyusunnya menjadi model dokumen.
    /// </summary>
    /// <param name="perjanjianId">ID dari perjanjian yang akan dibuat.</param>
    /// <returns>Model lengkap yang siap digunakan oleh QuestPDF.</returns>
    public PerjanjianDocumentModel GetPerjanjianModelKustom(int perjanjianId)
    {
        // 1. Ambil data utama perjanjian dari database
        var perjanjian = GetPerjanjianFromDb(perjanjianId);
        if (perjanjian == null)
        {
            // Jika perjanjian tidak ditemukan, kembalikan null
            return null;
        }

        // 2. Ambil data pihak-pihak terkait berdasarkan ID dari data perjanjian
        var pihakPertama = GetPihakPertamaFromDb(perjanjian.IdPihakPertama);
        var pihakKedua = GetPenyediaLayananFromDb(perjanjian.IdPenyedia);

        // 3. Ambil semua konten dinamis (bab, sub-bab, poin) sebagai daftar datar
        var flatKontenList = GetFlatKontenListFromDb(perjanjianId);

        // 4. Ubah daftar datar menjadi struktur hierarkis yang dibutuhkan oleh model PDF
        var (ketentuanKhusus, lampiran) = MapToDocumentModel(flatKontenList);

        // 5. Gabungkan semua data yang telah diambil ke dalam satu model akhir
        var finalModel = new PerjanjianDocumentModel
        {
            PihakPertama = pihakPertama,
            PihakKedua = pihakKedua,
            Perjanjian = perjanjian,
            KetentuanKhusus = ketentuanKhusus,
            Lampiran = lampiran
        };

        return finalModel;
    }

    // --- METODE HELPER UNTUK MENGAMBIL DATA DARI DATABASE ---

    private Perjanjian GetPerjanjianFromDb(int perjanjianId)
    {
        Perjanjian perjanjian = null;
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand("SELECT * FROM Perjanjian WHERE id_perjanjian = @id", connection);
            command.Parameters.AddWithValue("@id", perjanjianId);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    perjanjian = new Perjanjian
                    {
                        IdPerjanjian = (int)reader["id_perjanjian"],
                        IdPihakPertama = (int)reader["id_pihak_pertama"], // PERBAIKAN
                        IdPenyedia = (int)reader["id_penyedia"],         // PERBAIKAN
                        NoPtInhealth = reader["no_pt_inhealth"] as string,
                        NoPtPihakKedua = reader["no_pt_pihak_kedua"] as string,
                        TanggalTandaTangan = (DateTime)reader["tanggal_tanda_tangan"],
                        NomorBeritaAcara = reader["nomor_berita_acara"] as string,
                        TanggalBeritaAcara = (DateTime)reader["tanggal_berita_acara"],
                        JangkaWaktuPerjanjian = reader["jangka_waktu_perjanjian"] as string
                    };
                }
            }
        }
        return perjanjian;
    }

    private PihakPertama GetPihakPertamaFromDb(int pihakPertamaId)
    {
        PihakPertama pihakPertama = null;
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand("SELECT * FROM Pihak_Pertama WHERE id_pihak_pertama = @id", connection);
            command.Parameters.AddWithValue("@id", pihakPertamaId);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    pihakPertama = new PihakPertama
                    {
                        IdPihakPertama = (int)reader["id_pihak_pertama"],
                        NomorNibPihakPertama = (string)reader["nomor_nib_pihak_pertama"],
                        NamaPerwakilanPihakPertama = (string)reader["nama_perwakilan_pihak_pertama"],
                        JabatanPerwakilanPihakPertama = (string)reader["jabatan_perwakilan_pihak_pertama"]
                    };
                }
            }
        }
        return pihakPertama;
    }

    private PenyediaLayanan GetPenyediaLayananFromDb(int penyediaId)
    {
        PenyediaLayanan penyedia = null;
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand("SELECT * FROM Penyedia_Layanan WHERE id_penyedia = @id", connection);
            command.Parameters.AddWithValue("@id", penyediaId);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    penyedia = new PenyediaLayanan
                    {
                        IdPenyedia = (int)reader["id_penyedia"],
                        NamaEntitasCalonProvider = (string)reader["nama_Entitas_calon_provider"],
                        JenisPerusahaan = reader["jenis_perusahaan"] as string,
                        NoNibPihakKedua = (string)reader["no_nib_pihak_kedua"],
                        AlamatPemegangPolis = reader["alamat_pemegang_polis"] as string,
                        NamaPerwakilan = (string)reader["nama_perwakilan"],
                        JabatanPerwakilan = reader["jabatan_perwakilan"] as string,
                        JenisFasilitas = (string)reader["jenis_fasilitas"],
                        NamaFasilitasKesehatan = (string)reader["nama_fasilitas_kesehatan"],
                        NamaDokumenIzin = reader["nama_dokumen_izin"] as string,
                        NomorDokumenIzin = reader["nomor_dokumen_izin"] as string,
                        TanggalDokumenIzin = reader["tanggal_dokumen_izin"] as DateTime?,
                        InstansiPenerbitIzin = reader["instansi_penerbit_izin"] as string,
                        NamaBank = (string)reader["Nama_Bank"],
                        NamaCabangBank = reader["nama_cabang_bank"] as string,
                        NomorRekening = (string)reader["Nomor_rekening"],
                        PemilikRekening = (string)reader["pemilik_rekening"],
                        NamaPemilikNpwp = (string)reader["NAMA_PEMILIK_NPWP"],
                        NoNpwp = (string)reader["NO_NPWP"],
                        JenisNpwp = (string)reader["jenis_npwp"]
                    };
                }
            }
        }
        return penyedia;
    }

    private List<PerjanjianKontenData> GetFlatKontenListFromDb(int perjanjianId)
    {
        var list = new List<PerjanjianKontenData>();
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand("SELECT id, parent_id, level_type, konten, urutan_tampil FROM Perjanjian_Konten WHERE id_perjanjian = @idPerjanjian", connection);
            command.Parameters.AddWithValue("@idPerjanjian", perjanjianId);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new PerjanjianKontenData
                    {
                        Id = (int)reader["id"],
                        ParentId = reader.IsDBNull(reader.GetOrdinal("parent_id")) ? null : (int?)reader["parent_id"],
                        LevelType = (string)reader["level_type"],
                        Konten = reader["konten"] as string,
                        UrutanTampil = (int)reader["urutan_tampil"]
                    });
                }
            }
        }
        return list;
    }

    /// <summary>
    /// Mengubah daftar data datar dari database menjadi struktur objek hierarkis.
    /// </summary>
    private (List<BabModel> KetentuanKhusus, List<BabModel> Lampiran) MapToDocumentModel(List<PerjanjianKontenData> flatList)
    {
        var babLookup = new Dictionary<int, BabModel>();
        var subBabLookup = new Dictionary<int, SubBabModel>();
        var poinLookup = new Dictionary<int, PoinModel>();

        var ketentuanKhususList = new List<BabModel>();
        var lampiranList = new List<BabModel>();

        // Langkah 1: Proses semua item 'Judul' terlebih dahulu untuk membangun level teratas
        foreach (var item in flatList.Where(x => x.LevelType == "Judul").OrderBy(x => x.UrutanTampil))
        {
            var bab = new BabModel
            {
                JudulTeks = item.Konten,
                UrutanTampil = item.UrutanTampil,
                SubBab = new List<SubBabModel>()
            };
            babLookup[item.Id] = bab; // Simpan referensi ke BabModel

            // Pisahkan antara Ketentuan Khusus dan Lampiran
            if (item.Konten != null && item.Konten.ToUpper().Contains("LAMPIRAN"))
            {
                lampiranList.Add(bab);
            }
            else
            {
                ketentuanKhususList.Add(bab);
            }
        }

        // Langkah 2: Proses semua item 'SubBab' dan tautkan ke 'Judul' yang sesuai
        foreach (var item in flatList.Where(x => x.LevelType == "SubBab").OrderBy(x => x.UrutanTampil))
        {
            if (item.ParentId.HasValue && babLookup.TryGetValue(item.ParentId.Value, out var parentBab))
            {
                var subBab = new SubBabModel
                {
                    Konten = item.Konten,
                    UrutanTampil = item.UrutanTampil,
                    Poin = new List<PoinModel>()
                };
                subBabLookup[item.Id] = subBab; // Simpan referensi ke SubBabModel
                parentBab.SubBab.Add(subBab);
            }
        }

        // Langkah 3: Proses semua item 'Poin' dan tautkan ke 'SubBab' atau 'Poin' lain yang sesuai
        foreach (var item in flatList.Where(x => x.LevelType == "Poin").OrderBy(x => x.UrutanTampil))
        {
            var poin = new PoinModel
            {
                Id = item.Id,
                ParentId = item.ParentId,
                TeksPoin = item.Konten,
                UrutanTampil = item.UrutanTampil,
                SubPoin = new List<PoinModel>()
            };
            poinLookup[item.Id] = poin; // Simpan referensi ke PoinModel

            if (item.ParentId.HasValue)
            {
                // Cek apakah parent-nya adalah SubBab
                if (subBabLookup.TryGetValue(item.ParentId.Value, out var parentSubBab))
                {
                    parentSubBab.Poin.Add(poin);
                }
                // Cek apakah parent-nya adalah Poin lain (untuk sub-poin)
                else if (poinLookup.TryGetValue(item.ParentId.Value, out var parentPoin))
                {
                    parentPoin.SubPoin.Add(poin);
                }
            }
        }
        return (ketentuanKhususList, lampiranList);
    }
}
