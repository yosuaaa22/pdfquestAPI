using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pdfquestAPI.Dtos.Perjanjian;
using pdfquestAPI.Interfaces;
using pdfquestAPI.Models;

namespace pdfquestAPI.Services
{
    /// <summary>
    /// Layanan untuk operasi CRUD terkait entitas Perjanjian.
    /// Menggunakan UnitOfWork untuk mengakses repository terkait.
    /// </summary>
    public class PerjanjianService : IPerjanjianService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PerjanjianService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Ambil semua perjanjian yang tersedia.
        /// </summary>
        public async Task<IEnumerable<PerjanjianDto>> GetAllAsync()
        {
            var perjanjianList = await _unitOfWork.Perjanjian.GetAllAsync();
            return perjanjianList.Select(MapToDto);
        }

        /// <summary>
        /// Ambil perjanjian berdasarkan ID.
        /// Melempar KeyNotFoundException jika tidak ditemukan.
        /// </summary>
        public async Task<PerjanjianDto> GetByIdAsync(int id)
        {
            var perjanjian = await _unitOfWork.Perjanjian.GetByIdAsync(id);
            if (perjanjian == null)
            {
                throw new KeyNotFoundException($"Perjanjian dengan ID {id} tidak ditemukan.");
            }

            return MapToDto(perjanjian);
        }

        /// <summary>
        /// Buat perjanjian baru.
        /// Memastikan penyedia layanan yang dirujuk ada sebelum menyimpan.
        /// </summary>
        public async Task<PerjanjianDto> CreateAsync(CreatePerjanjianDto createDto)
        {
            // Validasi: Pastikan Penyedia Layanan ada sebelum membuat perjanjian
            var penyediaExists = await _unitOfWork.PenyediaLayanan.GetByIdAsync(createDto.IdPenyedia);
            if (penyediaExists == null)
            {
                throw new ArgumentException($"Penyedia Layanan dengan ID {createDto.IdPenyedia} tidak ditemukan.");
            }

            var newPerjanjian = new Perjanjian
            {
                IdPenyedia = createDto.IdPenyedia,
                NoPtInhealth = createDto.NoPtInhealth,
                NoPtPihakKedua = createDto.NoPtPihakKedua,
                TanggalTandaTangan = createDto.TanggalTandaTangan,
                NomorBeritaAcara = createDto.NomorBeritaAcara,
                TanggalBeritaAcara = createDto.TanggalBeritaAcara,
                JangkaWaktuPerjanjian = createDto.JangkaWaktuPerjanjian
            };

            await _unitOfWork.Perjanjian.AddAsync(newPerjanjian);
            await _unitOfWork.CompleteAsync();

            return MapToDto(newPerjanjian);
        }

        /// <summary>
        /// Perbarui data perjanjian berdasarkan ID.
        /// Memastikan entitas perjanjian dan penyedia layanan tujuan ada.
        /// </summary>
        public async Task UpdateAsync(int id, UpdatePerjanjianDto updateDto)
        {
            var perjanjianToUpdate = await _unitOfWork.Perjanjian.GetByIdAsync(id);
            if (perjanjianToUpdate == null)
            {
                throw new KeyNotFoundException($"Perjanjian dengan ID {id} tidak ditemukan.");
            }

            // Validasi: Pastikan Penyedia Layanan yang baru juga ada
            var penyediaExists = await _unitOfWork.PenyediaLayanan.GetByIdAsync(updateDto.IdPenyedia);
            if (penyediaExists == null)
            {
                throw new ArgumentException($"Penyedia Layanan dengan ID {updateDto.IdPenyedia} tidak ditemukan.");
            }

            // Lakukan update properti
            perjanjianToUpdate.IdPenyedia = updateDto.IdPenyedia;
            perjanjianToUpdate.NoPtInhealth = updateDto.NoPtInhealth;
            perjanjianToUpdate.NoPtPihakKedua = updateDto.NoPtPihakKedua;
            perjanjianToUpdate.TanggalTandaTangan = updateDto.TanggalTandaTangan;
            perjanjianToUpdate.NomorBeritaAcara = updateDto.NomorBeritaAcara;
            perjanjianToUpdate.TanggalBeritaAcara = updateDto.TanggalBeritaAcara;
            perjanjianToUpdate.JangkaWaktuPerjanjian = updateDto.JangkaWaktuPerjanjian;

            _unitOfWork.Perjanjian.Update(perjanjianToUpdate);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Hapus perjanjian berdasarkan ID.
        /// Melempar KeyNotFoundException jika tidak ditemukan.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var perjanjianToDelete = await _unitOfWork.Perjanjian.GetByIdAsync(id);
            if (perjanjianToDelete == null)
            {
                throw new KeyNotFoundException($"Perjanjian dengan ID {id} tidak ditemukan.");
            }

            _unitOfWork.Perjanjian.Delete(perjanjianToDelete);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Mapping model Perjanjian ke PerjanjianDto.
        /// Dipisahkan untuk menghindari duplikasi kode mapping.
        /// </summary>
        private PerjanjianDto MapToDto(Perjanjian perjanjian)
        {
            return new PerjanjianDto
            {
                IdPerjanjian = perjanjian.IdPerjanjian,
                IdPenyedia = perjanjian.IdPenyedia,
                NoPtInhealth = perjanjian.NoPtInhealth,
                NoPtPihakKedua = perjanjian.NoPtPihakKedua,
                TanggalTandaTangan = perjanjian.TanggalTandaTangan,
                NomorBeritaAcara = perjanjian.NomorBeritaAcara,
                TanggalBeritaAcara = perjanjian.TanggalBeritaAcara,
                JangkaWaktuPerjanjian = perjanjian.JangkaWaktuPerjanjian
            };
        }
    }
}
