using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using pdfquestAPI.Documents;
using pdfquestAPI.Documents.Models;
using pdfquestAPI.Repositories;
using QuestPDF.Fluent; // <-- PERBAIKAN: Menambahkan using statement yang hilang

// =================================================================
// CONTROLLER UNTUK ENDPOINT PDF KUSTOM
// =================================================================
// Kelas ini mendefinisikan endpoint API baru untuk menghasilkan PDF
// dari data yang sudah dikustomisasi di tabel Perjanjian_Konten.

[ApiController]
[Route("api")]
public class PerjanjianKustomPdfController : ControllerBase
{
    private readonly PerjanjianKontenRepository _repository;

    // Constructor ini akan menerima instance dari repository yang sudah
    // didaftarkan di Program.cs (dependency injection).
    public PerjanjianKustomPdfController(PerjanjianKontenRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Endpoint untuk menghasilkan PDF dari sebuah perjanjian yang kontennya sudah dikustomisasi.
    /// </summary>
    /// <param name="id">ID dari perjanjian yang akan dibuat PDF-nya.</param>
    /// <returns>File PDF atau pesan error.</returns>
    [HttpGet("perjanjian/{id}/pdf/kustom")]
    public IActionResult GenerateKustomPdf(int id)
    {
        try
        {
            // 1. Panggil repository untuk mendapatkan model data yang sudah lengkap dan hierarkis
            var perjanjianModel = _repository.GetPerjanjianModelKustom(id);

            // Jika repository mengembalikan null, berarti data tidak ditemukan
            if (perjanjianModel == null)
            {
                return NotFound($"Data perjanjian dengan ID {id} tidak ditemukan.");
            }

            // 2. Buat instance dari kelas dokumen QuestPDF Anda dengan model yang baru
            var document = new PerjanjianDocument(perjanjianModel);

            // 3. Hasilkan PDF dan kembalikan sebagai file untuk diunduh
            byte[] pdfBytes = document.GeneratePdf();
            string fileName = $"Perjanjian_Kustom_{id}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            // Jika terjadi error, catat error tersebut (opsional) dan kirim respons error ke client
            // Console.WriteLine(ex.ToString()); // Untuk debugging
            return StatusCode(500, "Terjadi kesalahan internal saat membuat PDF: " + ex.Message);
        }
    }
}
