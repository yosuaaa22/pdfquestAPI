// Mengimpor namespace yang dibutuhkan dari .NET dan proyek Anda
using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Data;
using pdfquestAPI.Interfaces;
using pdfquestAPI.Repositories;
using pdfquestAPI.Services;
using QuestPDF.Infrastructure;

// 1. Membuat WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);

// 2. Menambahkan Layanan ke Dependency Injection Container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Mendaftarkan semua service aplikasi Anda.
builder.Services.AddScoped<IPerjanjianService, PerjanjianService>();
builder.Services.AddScoped<IJudulIsiService, JudulIsiService>();
builder.Services.AddScoped<ISubBabKetentuanKhususService, SubBabKetentuanKhususService>();
builder.Services.AddScoped<IPoinKetentuanKhususService, PoinKetentuanKhususService>();
builder.Services.AddScoped<IPdfService, PdfService>();

// =================================================================
// ---> TAMBAHKAN BARIS INI <---
// Mendaftarkan repository baru untuk konten perjanjian kustom.
builder.Services.AddScoped<PerjanjianKontenRepository>();
// =================================================================

QuestPDF.Settings.License = LicenseType.Community;

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