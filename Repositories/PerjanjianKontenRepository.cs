using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Data;
using pdfquestAPI.Documents.Models;
using pdfquestAPI.Dtos;
using pdfquestAPI.Models;

namespace pdfquestAPI.Repositories
{
    // DTO untuk operasi penambahan konten (dipertahankan sesuai asal)
    public class CreateKontenDto
    {
        public int IdPerjanjian { get; set; }
        public int? ParentId { get; set; }
        public string LevelType { get; set; } = string.Empty;
        public string Konten { get; set; } = string.Empty;
        public int UrutanTampil { get; set; }
    }

    public class PerjanjianKontenRepository
    {
        #region Fields & Konstruktor
        private readonly string _connectionString;

        public PerjanjianKontenRepository(ApplicationDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var cs = context.Database.GetConnectionString();
            _connectionString = cs ?? throw new InvalidOperationException("Database connection string is not configured.");
        }
        #endregion

        #region Internal DTO untuk pemrosesan
        // Struktur data sementara yang merepresentasikan baris Perjanjian_Konten
        private class PerjanjianKontenData
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public string LevelType { get; set; } = string.Empty;
            public string? Konten { get; set; }
            public int UrutanTampil { get; set; }
        }
        #endregion

        #region Public API utama
        /// <summary>
        /// Bangun model dokumen Perjanjian berdasarkan data DB.
        /// Mengembalikan null apabila perjanjian tidak ditemukan.
        /// </summary>
        public PerjanjianDocumentModel GetPerjanjianModelKustom(int perjanjianId)
        {
            var perjanjian = GetPerjanjianFromDb(perjanjianId);
            if (perjanjian == null) return null!;

            var pihakPertama = GetPihakPertamaFromDb(perjanjian.IdPihakPertama);
            var pihakKedua = GetPenyediaLayananFromDb(perjanjian.IdPenyedia);
            var flatKontenList = GetFlatKontenListFromDb(perjanjianId);
            var lampiranPicData = GetLampiranPicFromDb(perjanjian.IdPenyedia);
            var lampiranTindakanMedisData = GetLampiranTindakanMedisFromDb(perjanjian.IdPenyedia);

            // Map daftar datar ke model dokumen (ketentuan + lampiran)
            var (ketentuanKhusus, lampiran) = MapToDocumentModel(flatKontenList);

            return new PerjanjianDocumentModel
            {
                PihakPertama = pihakPertama!,
                PihakKedua = pihakKedua!,
                Perjanjian = perjanjian!,
                KetentuanKhusus = ketentuanKhusus,
                Lampiran = lampiran,
                LampiranPic = lampiranPicData,
                LampiranTindakanMedis = lampiranTindakanMedisData
            };
        }

        /// <summary>
        /// Ambil teks konten berdasarkan id.
        /// </summary>
        public async Task<string?> GetKontenTextByIdAsync(int kontenId)
        {
            string? konten = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT konten FROM Perjanjian_Konten WHERE id = @id", connection);
                command.Parameters.AddWithValue("@id", kontenId);
                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    konten = result as string;
                }
            }
            return konten;
        }
        #endregion

        #region Mapping ke model dokumen
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
                    JudulTeks = babData.Konten ?? string.Empty,
                    UrutanTampil = babData.UrutanTampil,
                    SubBab = new()
                };

                var subBabItems = itemsByParent[babData.Id].Where(i => i.LevelType == "SubBab").OrderBy(i => i.UrutanTampil);

                foreach (var subBabData in subBabItems)
                {
                    var allDescendantPoints = new List<PoinModel>();
                    FindAllDescendantPoints(subBabData.Id, itemsByParent, allDescendantPoints);

                    // Penyesuaian: jadikan titik (poin) tingkat pertama anak SubBab memiliki ParentId = null
                    // agar renderer/penyusun dokumen dapat mengenali poin level pertama sebagai root poin dalam SubBab.
                    foreach (var poin in allDescendantPoints)
                    {
                        if (poin.ParentId == subBabData.Id)
                        {
                            poin.ParentId = null;
                        }
                    }

                    var subBabModel = new SubBabModel
                    {
                        Konten = subBabData.Konten ?? string.Empty,
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
        #endregion

        #region Helper rekursif untuk poin
        /// <summary>
        /// Kumpulkan semua turunan poin dimulai dari parentId ke dalam list datar, mempertahankan urutan.
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
                    TeksPoin = childData.Konten ?? string.Empty,
                    UrutanTampil = childData.UrutanTampil
                });
                FindAllDescendantPoints(childData.Id, itemsByParent, collectedPoints);
            }
        }
        #endregion

        #region Akses data dari database (Query)
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

        private Perjanjian? GetPerjanjianFromDb(int perjanjianId)
        {
            Perjanjian? perjanjian = null;
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

        private PihakPertama? GetPihakPertamaFromDb(int pihakPertamaId)
        {
            PihakPertama? pihakPertama = null;
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

        private PenyediaLayanan? GetPenyediaLayananFromDb(int penyediaId)
        {
            PenyediaLayanan? penyedia = null;
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
                            NamaEntitasCalonProvider = reader["nama_Entitas_calon_provider"] as string ?? string.Empty,
                            JenisPerusahaan = reader["jenis_perusahaan"] as string ?? string.Empty,
                            NoNibPihakKedua = reader["no_nib_pihak_kedua"] as string ?? string.Empty,
                            AlamatPemegangPolis = reader["alamat_pemegang_polis"] as string ?? string.Empty,
                            NamaPerwakilan = reader["nama_perwakilan"] as string ?? string.Empty,
                            JabatanPerwakilan = reader["jabatan_perwakilan"] as string ?? string.Empty,
                            JenisFasilitas = reader["jenis_fasilitas"] as string ?? string.Empty,
                            NamaFasilitasKesehatan = reader["nama_fasilitas_kesehatan"] as string ?? string.Empty,
                            NamaDokumenIzin = reader["nama_dokumen_izin"] as string ?? string.Empty,
                            NomorDokumenIzin = reader["nomor_dokumen_izin"] as string ?? string.Empty,
                            TanggalDokumenIzin = reader.IsDBNull(reader.GetOrdinal("tanggal_dokumen_izin")) ? null : (DateTime?)reader["tanggal_dokumen_izin"],
                            InstansiPenerbitIzin = reader["instansi_penerbit_izin"] as string ?? string.Empty,
                            NamaBank = reader["Nama_Bank"] as string ?? string.Empty,
                            NamaCabangBank = reader["nama_cabang_bank"] as string ?? string.Empty,
                            NomorRekening = reader["Nomor_rekening"] as string ?? string.Empty,
                            PemilikRekening = reader["pemilik_rekening"] as string ?? string.Empty,
                            NamaPemilikNpwp = reader["NAMA_PEMILIK_NPWP"] as string ?? string.Empty,
                            NoNpwp = reader["NO_NPWP"] as string ?? string.Empty,
                            JenisNpwp = reader["jenis_npwp"] as string ?? string.Empty
                        };
                    }
                }
            }
            return penyedia;
        }

        private List<PicModel> GetLampiranPicFromDb(int idPenyedia)
        {
            var list = new List<PicModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT JenisPIC, NamaPIC, NomorTelepon, AlamatEmail FROM Lampiran_PIC WHERE IdPenyedia = @idPenyedia", connection);
                command.Parameters.AddWithValue("@idPenyedia", idPenyedia);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PicModel
                        {
                            JenisPIC = reader["JenisPIC"] as string ?? string.Empty,
                            NamaPIC = reader["NamaPIC"] as string ?? string.Empty,
                            NomorTelepon = reader["NomorTelepon"] as string ?? string.Empty,
                            AlamatEmail = reader["AlamatEmail"] as string ?? string.Empty
                        });
                    }
                }
            }
            return list;
        }

        private List<TindakanMedisModel> GetLampiranTindakanMedisFromDb(int idPenyedia)
        {
            var list = new List<TindakanMedisModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT JenisTindakanMedis, Tarif, Keterangan FROM Lampiran_TindakanMedis WHERE IdPenyedia = @idPenyedia", connection);
                command.Parameters.AddWithValue("@idPenyedia", idPenyedia);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new TindakanMedisModel
                        {
                            JenisTindakanMedis = reader["JenisTindakanMedis"] as string,
                            Tarif = reader.IsDBNull(reader.GetOrdinal("Tarif")) ? null : (decimal?)reader["Tarif"],
                            Keterangan = reader["Keterangan"] as string
                        });
                    }
                }
            }
            return list;
        }
        #endregion

        #region Metode CRUD Konten
        /// <summary>
        /// Cari semua konten (struktur) berdasarkan keywords (dipisah koma).
        /// </summary>
        public async Task<List<KontenStrukturDto>> CariSemuaKontenByKeywordsAsync(int perjanjianId, string kataKunci)
        {
            var hasil = new List<KontenStrukturDto>();
            var keywords = kataKunci.Split(',')
                                    .Select(k => k.Trim())
                                    .Where(k => !string.IsNullOrEmpty(k))
                                    .ToList();

            if (!keywords.Any()) return hasil;

            using (var connection = new SqlConnection(_connectionString))
            {
                var sqlBuilder = new System.Text.StringBuilder(
                    "SELECT id, parent_id, level_type, konten, urutan_tampil FROM Perjanjian_Konten WHERE id_perjanjian = @idPerjanjian"
                );

                for (int i = 0; i < keywords.Count; i++)
                {
                    sqlBuilder.Append($" AND konten LIKE @kataKunci{i}");
                }
                sqlBuilder.Append(" ORDER BY urutan_tampil, id");

                var command = new SqlCommand(sqlBuilder.ToString(), connection);
                command.Parameters.AddWithValue("@idPerjanjian", perjanjianId);

                for (int i = 0; i < keywords.Count; i++)
                {
                    command.Parameters.AddWithValue($"@kataKunci{i}", $"%{keywords[i]}%");
                }

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        hasil.Add(new KontenStrukturDto
                        {
                            Id = (int)reader["id"],
                            ParentId = reader.IsDBNull(reader.GetOrdinal("parent_id")) ? (int?)null : (int)reader["parent_id"],
                            LevelType = (string)reader["level_type"],
                            Konten = reader["konten"] as string,
                            UrutanTampil = (int)reader["urutan_tampil"]
                        });
                    }
                }
            }
            return hasil;
        }

        /// <summary>
        /// Tambah konten baru dan panggil stored procedure untuk mengurutkan ulang.
        /// </summary>
        public async Task TambahDanUrutkanUlangKontenAsync(CreateKontenDto dto)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("sp_CreateKontenDanUrutkan", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@idPerjanjian", dto.IdPerjanjian);
                cmd.Parameters.AddWithValue("@parentId", dto.ParentId.HasValue ? (object)dto.ParentId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@levelType", dto.LevelType);
                cmd.Parameters.AddWithValue("@konten", (object)dto.Konten ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@urutanTampil", dto.UrutanTampil);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateKontenAsync(int kontenId, UpdateKontenDto dto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_UpdateKontenDanUrutkan", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@kontenId", kontenId);
                command.Parameters.AddWithValue("@newKonten", dto.Konten);
                command.Parameters.AddWithValue("@newUrutanTampil", dto.UrutanTampil);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Hapus konten (berserta anaknya) via stored procedure.
        /// Parameter perjanjianId dipertahankan pada signature untuk kompatibilitas, namun tidak digunakan di dalam metode.
        /// </summary>
        public async Task HapusDanUrutkanUlangKontenAsync(int kontenId, int perjanjianId)
        {
            // Tegaskan bahwa perjanjianId tidak digunakan agar tidak menimbulkan warning kompilasi.
            _ = perjanjianId;

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