// Mengimpor namespace yang dibutuhkan dari .NET dan proyek Anda
using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Data;
using pdfquestAPI.Interfaces;
using pdfquestAPI.Repositories;
using pdfquestAPI.Services;
using QuestPDF.Infrastructure;

// 1. Membuat WebApplicationBuilder
// Ini adalah langkah awal untuk mengonfigurasi aplikasi web Anda.
var builder = WebApplication.CreateBuilder(args);

// 2. Menambahkan Layanan ke Dependency Injection Container
// Di sinilah Anda mendaftarkan semua layanan yang akan digunakan aplikasi.

// Menambahkan DbContext untuk koneksi ke database SQL Server.
// Connection string "DefaultConnection" diambil dari file appsettings.json.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Mendaftarkan Unit of Work dan Repositories.
// AddScoped berarti satu instance akan dibuat untuk setiap HTTP request.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Mendaftarkan semua service aplikasi Anda.
builder.Services.AddScoped<IPerjanjianService, PerjanjianService>();
builder.Services.AddScoped<IJudulIsiService, JudulIsiService>();
builder.Services.AddScoped<ISubBabKetentuanKhususService, SubBabKetentuanKhususService>();
builder.Services.AddScoped<IPoinKetentuanKhususService, PoinKetentuanKhususService>();
builder.Services.AddScoped<IPdfService, PdfService>();
// Jika Anda membuat service baru, daftarkan di sini.

// Menambahkan layanan dasar untuk API, seperti Controller dan Swagger.
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Diperlukan untuk API explorer
builder.Services.AddSwaggerGen(); // Untuk dokumentasi API interaktif

// 3. Membangun Aplikasi
// Setelah semua layanan dikonfigurasi, kita membangun aplikasi.
var app = builder.Build();

// 4. Mengonfigurasi HTTP Request Pipeline
// Di sini kita menentukan bagaimana aplikasi akan merespons setiap request yang masuk.

// Mengaktifkan Swagger hanya di lingkungan "Development" untuk keamanan.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mengalihkan semua request HTTP ke HTTPS.
app.UseHttpsRedirection();

// Mengaktifkan otorisasi (jika Anda akan menggunakannya nanti).
app.UseAuthorization();

// Memetakan request yang masuk ke Controller yang sesuai.
app.MapControllers();

// 5. Menjalankan Aplikasi
// Baris ini akan memulai server dan mendengarkan request yang masuk.
app.Run();