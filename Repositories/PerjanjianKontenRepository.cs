using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using pdfquestAPI.Documents.Models;
using pdfquestAPI.Models;

namespace pdfquestAPI.Repositories
{
    // DTOs untuk operasi CRUD Konten
    public class UpdateKontenDto { public string Konten { get; set; } }
    public class CreateKontenDto { public int IdPerjanjian { get; set; } public int? ParentId { get; set; } public string LevelType { get; set; } public string Konten { get; set; } }

    public class PerjanjianKontenRepository
    {
        private readonly string _connectionString;

        public PerjanjianKontenRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Kelas internal privat untuk menampung data mentah dari database
        private class PerjanjianKontenData
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public string LevelType { get; set; }
            public string Konten { get; set; }
            public int UrutanTampil { get; set; }
        }

        /// <summary>
        /// Metode utama yang dipanggil oleh Controller untuk membangun model PDF.
        /// </summary>
        public PerjanjianDocumentModel GetPerjanjianModelKustom(int perjanjianId)
        {
            var perjanjian = GetPerjanjianFromDb(perjanjianId);
            if (perjanjian == null) return null;

            var pihakPertama = GetPihakPertamaFromDb(perjanjian.IdPihakPertama);
            var pihakKedua = GetPenyediaLayananFromDb(perjanjian.IdPenyedia);
            var flatKontenList = GetFlatKontenListFromDb(perjanjianId);

            var (ketentuanKhusus, lampiran) = MapToDocumentModel(flatKontenList);

            return new PerjanjianDocumentModel
            {
                PihakPertama = pihakPertama,
                PihakKedua = pihakKedua,
                Perjanjian = perjanjian,
                KetentuanKhusus = ketentuanKhusus,
                Lampiran = lampiran
            };
        }

        /// <summary>
        /// Mengubah daftar data datar dari database menjadi struktur hierarkis yang dibutuhkan oleh PDF.
        /// </summary>
        private (List<BabModel> KetentuanKhusus, List<BabModel> Lampiran) MapToDocumentModel(List<PerjanjianKontenData> flatList)
        {
            var ketentuanKhususList = new List<BabModel>();
            var lampiranList = new List<BabModel>();

            if (!flatList.Any()) return (ketentuanKhususList, lampiranList);

            var itemsByParent = flatList.ToLookup(item => item.ParentId);
            var rootItems = itemsByParent[null].Where(i => i.LevelType == "Judul").OrderBy(i => i.UrutanTampil);

            foreach (var babData in rootItems)
            {
                var babModel = new BabModel
                {
                    JudulTeks = babData.Konten,
                    UrutanTampil = babData.UrutanTampil,
                    SubBab = new List<SubBabModel>()
                };

                var subBabItems = itemsByParent[babData.Id].Where(i => i.LevelType == "SubBab").OrderBy(i => i.UrutanTampil);

                foreach (var subBabData in subBabItems)
                {
                    var allDescendantPoints = new List<PoinModel>();
                    FindAllDescendantPoints(subBabData.Id, itemsByParent, allDescendantPoints);

                    var subBabModel = new SubBabModel
                    {
                        Konten = subBabData.Konten,
                        UrutanTampil = subBabData.UrutanTampil,
                        Poin = allDescendantPoints
                    };
                    babModel.SubBab.Add(subBabModel);
                }

                if (babData.Konten != null && babData.Konten.ToUpper().Contains("LAMPIRAN"))
                    lampiranList.Add(babModel);
                else
                    ketentuanKhususList.Add(babModel);
            }

            return (ketentuanKhususList, lampiranList);
        }

        /// <summary>
        /// Fungsi pembantu rekursif untuk mencari semua turunan poin dan menyusunnya menjadi satu daftar datar.
        /// </summary>
        private void FindAllDescendantPoints(int parentId, ILookup<int?, PerjanjianKontenData> itemsByParent, List<PoinModel> collectedPoints)
        {
            var children = itemsByParent[parentId].OrderBy(i => i.UrutanTampil);

            foreach (var childData in children)
            {
                collectedPoints.Add(new PoinModel
                {
                    Id = childData.Id,
                    ParentId = childData.ParentId,
                    TeksPoin = childData.Konten,
                    UrutanTampil = childData.UrutanTampil
                });
                FindAllDescendantPoints(childData.Id, itemsByParent, collectedPoints);
            }
        }

        #region Metode Akses Data dari Database

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
                            IdPihakPertama = (int)reader["id_pihak_pertama"],
                            IdPenyedia = (int)reader["id_penyedia"],
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
                            NamaEntitasCalonProvider = reader["nama_Entitas_calon_provider"] as string,
                            JenisPerusahaan = reader["jenis_perusahaan"] as string,
                            NoNibPihakKedua = reader["no_nib_pihak_kedua"] as string,
                            AlamatPemegangPolis = reader["alamat_pemegang_polis"] as string,
                            NamaPerwakilan = reader["nama_perwakilan"] as string,
                            JabatanPerwakilan = reader["jabatan_perwakilan"] as string,
                            JenisFasilitas = reader["jenis_fasilitas"] as string,
                            NamaFasilitasKesehatan = reader["nama_fasilitas_kesehatan"] as string,
                            NamaDokumenIzin = reader["nama_dokumen_izin"] as string,
                            NomorDokumenIzin = reader["nomor_dokumen_izin"] as string,
                            TanggalDokumenIzin = reader.IsDBNull(reader.GetOrdinal("tanggal_dokumen_izin")) ? null : (DateTime?)reader["tanggal_dokumen_izin"],
                            InstansiPenerbitIzin = reader["instansi_penerbit_izin"] as string,
                            NamaBank = reader["Nama_Bank"] as string,
                            NamaCabangBank = reader["nama_cabang_bank"] as string,
                            NomorRekening = reader["Nomor_rekening"] as string,
                            PemilikRekening = reader["pemilik_rekening"] as string,
                            NamaPemilikNpwp = reader["NAMA_PEMILIK_NPWP"] as string,
                            NoNpwp = reader["NO_NPWP"] as string,
                            JenisNpwp = reader["jenis_npwp"] as string
                        };
                    }
                }
            }
            return penyedia;
        }

        #endregion

        #region Metode CRUD Konten

        public async Task<object> CariKontenByKeywordAsync(int perjanjianId, string kataKunci)
        {
            object result = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT TOP 1 id, parent_id, konten, urutan_tampil FROM Perjanjian_Konten " +
                    "WHERE id_perjanjian = @idPerjanjian AND konten LIKE @kataKunci " +
                    "ORDER BY urutan_tampil, id", connection);

                command.Parameters.AddWithValue("@idPerjanjian", perjanjianId);
                command.Parameters.AddWithValue("@kataKunci", $"%{kataKunci}%");

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        result = new
                        {
                            id = (int)reader["id"],
                            parentId = reader.IsDBNull(reader.GetOrdinal("parent_id")) ? (int?)null : (int)reader["parent_id"],
                            konten = reader["konten"] as string,
                            urutanTampil = (int)reader["urutan_tampil"]
                        };
                    }
                }
            }
            return result;
        }

        public async Task TambahDanUrutkanUlangKontenAsync(CreateKontenDto dto)
        {
            // Note: PerbaikiSeluruhPenomoranAsync is a complex method that re-writes content. 
            // It might be better to call it separately or ensure its logic is what you intend.
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("INSERT INTO Perjanjian_Konten (id_perjanjian, parent_id, level_type, konten, urutan_tampil) VALUES (@idPerjanjian, @parentId, @levelType, @konten, 999);", conn);
                cmd.Parameters.AddWithValue("@idPerjanjian", dto.IdPerjanjian);
                cmd.Parameters.AddWithValue("@parentId", (object)dto.ParentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@levelType", dto.LevelType);
                cmd.Parameters.AddWithValue("@konten", dto.Konten);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateKontenAsync(int kontenId, string newKonten)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("UPDATE Perjanjian_Konten SET konten = @konten WHERE id = @id", connection);
                command.Parameters.AddWithValue("@konten", newKonten);
                command.Parameters.AddWithValue("@id", kontenId);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task HapusDanUrutkanUlangKontenAsync(int kontenId, int perjanjianId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("sp_HapusKontenDanAnaknya", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@id_konten_dihapus", kontenId);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        #endregion
    }
}