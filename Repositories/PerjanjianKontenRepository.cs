using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using pdfquestAPI.Documents.Models;
using pdfquestAPI.Models;

namespace pdfquestAPI.Repositories
{
    public class UpdateKontenDto { public string Konten { get; set; } }
    public class CreateKontenDto { public int IdPerjanjian { get; set; } public int? ParentId { get; set; } public string LevelType { get; set; } public string Konten { get; set; } }

    public class PerjanjianKontenRepository
    {
        private readonly string _connectionString;
        public PerjanjianKontenRepository(IConfiguration configuration) { _connectionString = configuration.GetConnectionString("DefaultConnection"); }

        private class PerjanjianKontenData
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public string LevelType { get; set; }
            public string Konten { get; set; }
            public int UrutanTampil { get; set; }
        }

        public async Task TambahDanUrutkanUlangKontenAsync(CreateKontenDto dto)
        {
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
            await PerbaikiSeluruhPenomoranAsync(dto.IdPerjanjian);
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
            await PerbaikiSeluruhPenomoranAsync(perjanjianId);
        }

        public async Task PerbaikiSeluruhPenomoranAsync(int perjanjianId)
        {
            var allItems = new List<PerjanjianKontenData>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT id, parent_id, konten, level_type, urutan_tampil FROM Perjanjian_Konten WHERE id_perjanjian = @idPerjanjian ORDER BY urutan_tampil, id", conn);
                cmd.Parameters.AddWithValue("@idPerjanjian", perjanjianId);
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        allItems.Add(new PerjanjianKontenData
                        {
                            Id = (int)reader["id"],
                            ParentId = reader.IsDBNull(reader.GetOrdinal("parent_id")) ? null : (int?)reader["parent_id"],
                            Konten = reader["konten"] as string,
                            LevelType = reader["level_type"] as string,
                            UrutanTampil = (int)reader["urutan_tampil"]
                        });
                    }
                }
            }

            if (!allItems.Any()) return;

            var itemLookup = allItems.ToDictionary(item => item.Id);
            var childrenLookup = allItems.Where(item => item.ParentId.HasValue).GroupBy(item => item.ParentId.Value)
                                        .ToDictionary(g => g.Key, g => g.OrderBy(i => i.UrutanTampil).ThenBy(i => i.Id).ToList());
            var updateCommands = new List<SqlCommand>();

            RecursiveRenumber(null, "", null);

            void RecursiveRenumber(int? currentParentId, string parentPrefix, string parentLevelType)
            {
                List<PerjanjianKontenData> children;
                if (currentParentId == null) children = allItems.Where(i => i.ParentId == null).OrderBy(i => i.UrutanTampil).ThenBy(i => i.Id).ToList();
                else if (!childrenLookup.TryGetValue(currentParentId.Value, out children)) return;

                if (parentLevelType == "Judul" && children.Count == 1)
                {
                    var singleChild = children.First();
                    var cleanText = GetCleanText(singleChild.Konten);
                    var updateCmd = new SqlCommand("UPDATE Perjanjian_Konten SET konten = @konten, urutan_tampil = @urutan WHERE id = @id");
                    updateCmd.Parameters.AddWithValue("@konten", cleanText);
                    updateCmd.Parameters.AddWithValue("@urutan", 1);
                    updateCmd.Parameters.AddWithValue("@id", singleChild.Id);
                    updateCommands.Add(updateCmd);
                    RecursiveRenumber(singleChild.Id, "", singleChild.LevelType);
                    return;
                }

                string numberingStyle = "numeric";
                if (children.Any()) {
                    var firstChild = children.First();
                    if (firstChild.LevelType == "Judul") numberingStyle = "numeric_bab";
                    else if (firstChild.LevelType == "SubBab") numberingStyle = "numeric_subbab";
                    else {
                        var firstChildParts = firstChild.Konten?.Trim().Split(new[] { ' ' }, 2);
                        if (firstChildParts?.Length > 1) {
                            var prefix = firstChildParts[0];
                            if (Regex.IsMatch(prefix, @"^[ivxlcdm]+\.$", RegexOptions.IgnoreCase)) numberingStyle = "roman";
                            else if (Regex.IsMatch(prefix, @"^[a-z]\.$", RegexOptions.IgnoreCase)) numberingStyle = "alphabetic";
                        }
                    }
                }

                for (int i = 0; i < children.Count; i++)
                {
                    var item = children[i];
                    int newUrutan = i + 1;
                    string newPrefix;
                    
                    switch (numberingStyle)
                    {
                        case "alphabetic": newPrefix = $"{ToAlphabetic(newUrutan)}."; break;
                        case "roman": newPrefix = $"{ToRoman(newUrutan).ToLower()}."; break;
                        case "numeric_bab": newPrefix = $"{newUrutan}."; break;
                        case "numeric_subbab": newPrefix = $"{parentPrefix}{newUrutan}"; break;
                        default: newPrefix = $"{parentPrefix}{newUrutan}."; break;
                    }

                    var cleanText = GetCleanText(item.Konten);
                    var newKonten = $"{newPrefix} {cleanText}";
                    
                    var updateCommand = new SqlCommand("UPDATE Perjanjian_Konten SET konten = @konten, urutan_tampil = @urutan WHERE id = @id");
                    updateCommand.Parameters.AddWithValue("@konten", newKonten);
                    updateCommand.Parameters.AddWithValue("@urutan", newUrutan);
                    updateCommand.Parameters.AddWithValue("@id", item.Id);
                    updateCommands.Add(updateCommand);
                    
                    string nextPrefix = numberingStyle == "numeric_subbab" ? newPrefix + "." : newPrefix;
                    RecursiveRenumber(item.Id, nextPrefix, item.LevelType);
                }
            }

            if (updateCommands.Any())
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var transaction = conn.BeginTransaction()) {
                        foreach (var cmd in updateCommands) {
                            cmd.Connection = conn;
                            cmd.Transaction = transaction;
                            await cmd.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                    }
                }
            }
        }
        
        private string GetCleanText(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            var parts = text.Trim().Split(new[] { ' ' }, 2);
            if (parts.Length > 1 && IsValidPrefix(parts[0]))
            {
                return parts[1];
            }
            return text;
        }

        private bool IsValidPrefix(string prefix)
        {
            return Regex.IsMatch(prefix, @"^([\d\.]+|[a-zA-Z]+\.|[ivxlcdm]+\.)$");
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

        private string ToAlphabetic(int number)
        {
            string result = string.Empty;
            while (number > 0)
            {
                int remainder = (number - 1) % 26;
                result = (char)('a' + remainder) + result;
                number = (number - remainder) / 26;
            }
            return result;
        }

        private string ToRoman(int number)
        {
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException(nameof(number), "Nomor Romawi di luar jangkauan");
        }

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
                var command = new SqlCommand("SELECT id, parent_id, level_type, konten, urutan_tampil FROM Perjanjian_Konten WHERE id_perjanjian = @idPerjanjian ORDER BY urutan_tampil, id", connection);
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

        private (List<BabModel> KetentuanKhusus, List<BabModel> Lampiran) MapToDocumentModel(List<PerjanjianKontenData> flatList)
        {
            var babLookup = new Dictionary<int, BabModel>();
            var subBabLookup = new Dictionary<int, SubBabModel>();
            var poinLookup = new Dictionary<int, PoinModel>();
            var ketentuanKhususList = new List<BabModel>();
            var lampiranList = new List<BabModel>();

            foreach (var item in flatList.Where(x => x.LevelType == "Judul").OrderBy(x => x.UrutanTampil))
            {
                var bab = new BabModel { JudulTeks = item.Konten, UrutanTampil = item.UrutanTampil, SubBab = new List<SubBabModel>() };
                babLookup[item.Id] = bab;
                if (item.Konten != null && item.Konten.ToUpper().Contains("LAMPIRAN")) lampiranList.Add(bab);
                else ketentuanKhususList.Add(bab);
            }

            foreach (var item in flatList.Where(x => x.LevelType == "SubBab").OrderBy(x => x.UrutanTampil))
            {
                if (item.ParentId.HasValue && babLookup.TryGetValue(item.ParentId.Value, out var parentBab))
                {
                    var subBab = new SubBabModel { Konten = item.Konten, UrutanTampil = item.UrutanTampil, Poin = new List<PoinModel>() };
                    subBabLookup[item.Id] = subBab;
                    parentBab.SubBab.Add(subBab);
                }
            }

            foreach (var item in flatList.Where(x => x.LevelType == "Poin").OrderBy(x => x.UrutanTampil))
            {
                var poin = new PoinModel { Id = item.Id, ParentId = item.ParentId, TeksPoin = item.Konten, UrutanTampil = item.UrutanTampil, SubPoin = new List<PoinModel>() };
                poinLookup[item.Id] = poin;
                if (item.ParentId.HasValue)
                {
                    if (subBabLookup.TryGetValue(item.ParentId.Value, out var parentSubBab)) parentSubBab.Poin.Add(poin);
                    else if (poinLookup.TryGetValue(item.ParentId.Value, out var parentPoin)) parentPoin.SubPoin.Add(poin);
                }
            }
            return (ketentuanKhususList, lampiranList);
        }
    }
}