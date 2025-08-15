using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pdfquestAPI.Dtos.JudulIsi;
using pdfquestAPI.Interfaces;
using pdfquestAPI.Models;

namespace pdfquestAPI.Services
{
    /// <summary>
    /// Service untuk operasi CRUD terkait entitas JudulIsi.
    /// Menggunakan Unit of Work untuk akses repository dan transaksi.
    /// </summary>
    public class JudulIsiService : IJudulIsiService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Injeksi UnitOfWork melalui konstruktor.
        /// </summary>
        public JudulIsiService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Ambil semua JudulIsi dan map ke DTO.
        /// </summary>
        public async Task<IEnumerable<JudulIsiDto>> GetAllAsync()
        {
            var judulIsiList = await _unitOfWork.JudulIsi.GetAllAsync();

            // Map model ke DTO untuk menghindari ekspos properti model domain langsung
            return judulIsiList.Select(j => new JudulIsiDto
            {
                Id = j.Id,
                IdPenyedia = j.IdPenyedia,
                JudulTeks = j.JudulTeks,
                UrutanTampil = j.UrutanTampil
            });
        }

        /// <summary>
        /// Ambil satu JudulIsi berdasarkan ID, lempar KeyNotFoundException jika tidak ditemukan.
        /// </summary>
        public async Task<JudulIsiDto> GetByIdAsync(int id)
        {
            var judulIsi = await _unitOfWork.JudulIsi.GetByIdAsync(id);
            if (judulIsi == null)
            {
                throw new KeyNotFoundException($"JudulIsi dengan ID {id} tidak ditemukan.");
            }

            return new JudulIsiDto
            {
                Id = judulIsi.Id,
                IdPenyedia = judulIsi.IdPenyedia,
                JudulTeks = judulIsi.JudulTeks,
                UrutanTampil = judulIsi.UrutanTampil
            };
        }

        /// <summary>
        /// Buat entitas JudulIsi baru dari DTO, simpan, lalu kembalikan DTO hasil penyimpanan.
        /// </summary>
        public async Task<JudulIsiDto> CreateAsync(CreateJudulIsiDto createDto)
        {
            var newJudulIsi = new JudulIsi
            {
                IdPenyedia = createDto.IdPenyedia,
                JudulTeks = createDto.JudulTeks,
                UrutanTampil = createDto.UrutanTampil
            };

            await _unitOfWork.JudulIsi.AddAsync(newJudulIsi);
            await _unitOfWork.CompleteAsync();

            // Kembalikan DTO yang berasal dari entitas tersimpan (memastikan ID terisi)
            return await GetByIdAsync(newJudulIsi.Id);
        }

        /// <summary>
        /// Perbarui entitas JudulIsi yang ada berdasarkan ID. Lempar KeyNotFoundException jika tidak ditemukan.
        /// </summary>
        public async Task UpdateAsync(int id, UpdateJudulIsiDto updateDto)
        {
            var judulIsiToUpdate = await _unitOfWork.JudulIsi.GetByIdAsync(id);
            if (judulIsiToUpdate == null)
            {
                throw new KeyNotFoundException($"JudulIsi dengan ID {id} tidak ditemukan.");
            }

            // Update properti yang diizinkan
            judulIsiToUpdate.IdPenyedia = updateDto.IdPenyedia;
            judulIsiToUpdate.JudulTeks = updateDto.JudulTeks;
            judulIsiToUpdate.UrutanTampil = updateDto.UrutanTampil;

            _unitOfWork.JudulIsi.Update(judulIsiToUpdate);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Hapus entitas JudulIsi berdasarkan ID. Lempar KeyNotFoundException jika tidak ditemukan.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var judulIsiToDelete = await _unitOfWork.JudulIsi.GetByIdAsync(id);
            if (judulIsiToDelete == null)
            {
                throw new KeyNotFoundException($"JudulIsi dengan ID {id} tidak ditemukan.");
            }

            _unitOfWork.JudulIsi.Delete(judulIsiToDelete);
            await _unitOfWork.CompleteAsync();
        }
    }
}
