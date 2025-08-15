namespace pdfquestAPI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using pdfquestAPI.Dtos.SubBabKetentuanKhusus;
    using pdfquestAPI.Interfaces;
    using pdfquestAPI.Models;

    /// <summary>
    /// Service untuk operasi CRUD SubBabKetentuanKhusus.
    /// Menggunakan IUnitOfWork untuk akses repository dan transaksi.
    /// </summary>
    public class SubBabKetentuanKhususService : ISubBabKetentuanKhususService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Inisialisasi service dengan unit of work.
        /// </summary>
        public SubBabKetentuanKhususService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Ambil semua SubBabKetentuanKhusus sebagai DTO.
        /// </summary>
        public async Task<IEnumerable<SubBabKetentuanKhususDto>> GetAllAsync()
        {
            var subBabList = await _unitOfWork.SubBabKetentuanKhusus.GetAllAsync();
            return subBabList.Select(MapToDto);
        }

        /// <summary>
        /// Ambil SubBab berdasarkan id. Jika tidak ditemukan, lempar KeyNotFoundException.
        /// </summary>
        public async Task<SubBabKetentuanKhususDto> GetByIdAsync(int id)
        {
            var subBab = await _unitOfWork.SubBabKetentuanKhusus.GetByIdAsync(id);
            if (subBab == null)
                throw new KeyNotFoundException($"SubBabKetentuanKhusus dengan ID {id} tidak ditemukan.");

            return MapToDto(subBab);
        }

        /// <summary>
        /// Buat SubBab baru.
        /// Validasi: pastikan parent (JudulIsi) tersedia sebelum menambah SubBab.
        /// </summary>
        public async Task<SubBabKetentuanKhususDto> CreateAsync(CreateSubBabKetentuanKhususDto createDto)
        {
            var judulExists = await _unitOfWork.JudulIsi.GetByIdAsync(createDto.IdJudul);
            if (judulExists == null)
                throw new ArgumentException($"JudulIsi dengan ID {createDto.IdJudul} tidak ditemukan.");

            var newSubBab = new SubBabKetentuanKhusus
            {
                IdJudul = createDto.IdJudul,
                Konten = createDto.Konten,
                UrutanTampil = createDto.UrutanTampil
            };

            await _unitOfWork.SubBabKetentuanKhusus.AddAsync(newSubBab);
            await _unitOfWork.CompleteAsync();

            return MapToDto(newSubBab);
        }

        /// <summary>
        /// Update SubBab yang sudah ada.
        /// Validasi: pastikan SubBab target dan Judul baru ada.
        /// </summary>
        public async Task UpdateAsync(int id, UpdateSubBabKetentuanKhususDto updateDto)
        {
            var subBabToUpdate = await _unitOfWork.SubBabKetentuanKhusus.GetByIdAsync(id);
            if (subBabToUpdate == null)
                throw new KeyNotFoundException($"SubBabKetentuanKhusus dengan ID {id} tidak ditemukan.");

            var judulExists = await _unitOfWork.JudulIsi.GetByIdAsync(updateDto.IdJudul);
            if (judulExists == null)
                throw new ArgumentException($"JudulIsi dengan ID {updateDto.IdJudul} tidak ditemukan.");

            subBabToUpdate.IdJudul = updateDto.IdJudul;
            subBabToUpdate.Konten = updateDto.Konten;
            subBabToUpdate.UrutanTampil = updateDto.UrutanTampil;

            _unitOfWork.SubBabKetentuanKhusus.Update(subBabToUpdate);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Hapus SubBab berdasarkan id. Jika tidak ditemukan, lempar KeyNotFoundException.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var subBabToDelete = await _unitOfWork.SubBabKetentuanKhusus.GetByIdAsync(id);
            if (subBabToDelete == null)
                throw new KeyNotFoundException($"SubBabKetentuanKhusus dengan ID {id} tidak ditemukan.");

            _unitOfWork.SubBabKetentuanKhusus.Delete(subBabToDelete);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Mapping dari entity SubBabKetentuanKhusus ke DTO untuk menghindari duplikasi kode.
        /// </summary>
        private SubBabKetentuanKhususDto MapToDto(SubBabKetentuanKhusus subBab)
        {
            return new SubBabKetentuanKhususDto
            {
                Id = subBab.Id,
                IdJudul = subBab.IdJudul,
                Konten = subBab.Konten,
                UrutanTampil = subBab.UrutanTampil
            };
        }
    }
}
