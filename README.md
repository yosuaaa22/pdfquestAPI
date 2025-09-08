# pdfquestAPI

API .NET 8 untuk mengelola konten perjanjian dan menghasilkan dokumen PDF (QuestPDF). Proyek ini menggunakan Entity Framework Core (SQL Server), pola Repository + Unit of Work, serta menyediakan endpoint untuk:

- CRUD struktur konten (Judul, SubBab, Poin) melalui layanan dan repository
- CRUD Perjanjian melalui layanan dan repository
- Change Request (ajukan perubahan, cek status, setujui/execute)
- Generate PDF perjanjian standar maupun versi final kustom

## Fitur Utama

- Manajemen struktur dokumen (Bab/SubBab/Poin) dengan urutan tampil
- Mekanisme Change Request yang aman dan transaksional
- Generator PDF berbasis QuestPDF dengan model dokumen yang terstruktur
- Pola arsitektur bersih: Repository + Unit of Work + DTO
- Swagger UI untuk eksplorasi API di lingkungan Development

## Teknologi

- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core 9 (SqlServer)
- QuestPDF 2024.3.4
- Swashbuckle (Swagger UI)

## Struktur Proyek (ringkas)


Controllers/                # Endpoint API (PDF, ChangeRequest, Perjanjian kustom)
Data/                       # ApplicationDbContext (DbSet entitas)
Documents/                  # Generator PDF (QuestPDF) + model dokumen
Dtos/                       # Kontrak data (request/response)
Interfaces/                 # Abstraksi Repository, Services, UnitOfWork
Models/                     # Entitas domain (EF Core)
Repositories/               # Implementasi repository dan UnitOfWork
Services/                   # Logika aplikasi (CRUD + PDF builder)
Program.cs                  # Bootstrap aplikasi & DI
appsettings*.json           # Konfigurasi (connection string, logging)


## Menjalankan Secara Lokal (Tanpa Docker)

1. Prasyarat

- Windows dengan .NET SDK 8 terinstal
- SQL Server lokal (Developer/Express/LocalDB) dan akses kredensial

2. Konfigurasi koneksi database

- Pilihan A: edit appsettings.Development.json -> ConnectionStrings:DefaultConnection
- Pilihan B (disarankan di dev): gunakan Secret Manager
  powershell
  dotnet user-secrets init
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=Pdf16;User Id=sa;Password=<password>;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True;"
  

3. (Opsional) Terapkan migrasi database (bila tersedia)

powershell
dotnet tool update --global dotnet-ef
dotnet ef database update


4. Jalankan aplikasi

powershell
dotnet build
dotnet run


Endpoint default (launchSettings):

- HTTP: http://localhost:5118
- HTTPS: https://localhost:7114

5. Swagger (Dev)

- Buka http://localhost:5118/swagger saat environment Development

## Endpoint API

### PDF

- GET api/Pdf/perjanjian/{perjanjianId}
  - Mengembalikan file PDF (Content-Type: application/pdf) untuk perjanjian terkait.
- GET api/perjanjian/{id}/pdf/final
  - Menghasilkan PDF final berdasarkan konten yang sudah disetujui.

### Pencarian Struktur Konten

- GET api/perjanjian/{id}/konten/struktur?kataKunci=...
  - Mengembalikan struktur konten yang cocok dengan kata kunci pada perjanjian tertentu.

### Change Requests

- POST api/changerequests/perjanjian/{perjanjianId}
  - Body (contoh ringkas):
    json
    {
      "DiajukanOleh": "user@example.com",
      "Deskripsi": "Perubahan struktur Bab",
      "Items": [
        {
          "ActionType": "CREATE",
          "LevelType": "SubBab",
          "ParentId": 10,
          "KontenBaru": "Ketentuan baru",
          "UrutanTampilBaru": 3
        }
      ]
    }
    
  - 201 Created: mengembalikan detail ChangeRequest yang baru dibuat.
- GET api/changerequests/{requestId}
  - Mengambil detail ChangeRequest beserta itemnya.
- POST api/changerequests/{requestId}/approve?approverName=Nama
  - Menyetujui request dan mengeksekusi perubahan secara transaksional (rollback bila ada error).

Catatan: CRUD langsung untuk Judul/SubBab/Poin/Perjanjian tersedia di layer Service/Repository. Jika membutuhkan endpoint publik, buat Controller mengikuti pola di ChangeRequestController.

## Kontrak Data (DTO) - ringkas

- ChangeRequestDto
  - DiajukanOleh (string, required)
  - Deskripsi (string, opsional)
  - Items[] berisi: ActionType (CREATE/UPDATE/DELETE), TargetKontenId?, KontenBaru?, LevelType?, ParentId?, UrutanTampilBaru?, AlasanPerubahan?
- UpdateKontenDto: Konten (string), UrutanTampil (int)
- KontenStrukturDto: Id, ParentId?, LevelType, Konten?, UrutanTampil

## Pola Desain & Praktik

- Repository generik + UnitOfWork untuk konsistensi akses data dan transaksi
- DTO memisahkan kontrak API dari entitas domain (keamanan dan stabilitas)
- QuestPDF menyusun dokumen dari model terstruktur (bab/subbab/poin/lampiran)

## Troubleshooting

- Tidak bisa konek DB: cek SQL Server aktif, port 1433 terbuka, user/password benar, dan Encrypt/TrustServerCertificate sesuai
- HTTPS error di dev:
  powershell
  dotnet dev-certs https --trust
  
- File Docker masih terlihat: proyek ini tidak memakai Docker. Pastikan file Dockerfile dan docker-compose.yml telah dihapus dan perubahan di-commit.
- Ikon “panah/link” pada file JSON di VS Code: itu menandakan file shortcut/symlink. Salin isi file, hapus file tersebut, lalu buat file baru biasa dengan nama yang sama dan tempelkan isi.

## Keamanan

- Hindari menyimpan kredensial di appsettings.*.json pada repo. Gunakan Secret Manager untuk dev dan KeyVault/konfigurasi aman untuk prod.

## Lisensi

Proyek internal. Sesuaikan bagian ini jika akan dipublikasikan.
