using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Data;
using pdfquestAPI.Interfaces;
using pdfquestAPI.Repositories;
using pdfquestAPI.Services;
using QuestPDF.Infrastructure;
using System.Text.Json.Serialization;

// 1. Membuat WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);

// 2. Menambahkan Layanan ke Dependency Injection Container

// a. Konfigurasi Database (DbContext)
// Pastikan "DefaultConnection" ada di file appsettings.json Anda
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// b. Konfigurasi QuestPDF
QuestPDF.Settings.License = LicenseType.Community;

// c. Mendaftarkan Unit of Work dan Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<PerjanjianKontenRepository>(); // Repository kustom

// d. Mendaftarkan semua Service aplikasi Anda
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IPerjanjianService, PerjanjianService>();
builder.Services.AddScoped<IJudulIsiService, JudulIsiService>();
builder.Services.AddScoped<ISubBabKetentuanKhususService, SubBabKetentuanKhususService>();
builder.Services.AddScoped<IPoinKetentuanKhususService, PoinKetentuanKhususService>();
builder.Services.AddScoped<PerjanjianKontenRepository>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Baris ini memberitahu API untuk mengonversi string menjadi enum saat menerima JSON
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Tambahkan service lain di sini jika ada
// builder.Services.AddScoped<IPenyediaLayananService, PenyediaLayananService>();
// builder.Services.AddScoped<IPihakPertamaService, PihakPertamaService>();


// e. Mendaftarkan layanan bawaan ASP.NET Core
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Membangun Aplikasi
var app = builder.Build();

// 4. Mengonfigurasi HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// 5. Menjalankan Aplikasi
app.Run();
