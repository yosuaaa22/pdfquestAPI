using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pdfquestAPI.Dtos.PoinKetentuanKhusus;
using pdfquestAPI.Interfaces;
using pdfquestAPI.Models;

namespace pdfquestAPI.Services
{
    /// <summary>
    /// Service untuk operasi CRUD PoinKetentuanKhusus.
    /// Menggunakan pattern Unit of Work untuk mengakses repository terkait.
    /// </summary>
    public class PoinKetentuanKhususService : IPoinKetentuanKhususService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PoinKetentuanKhususService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Ambil semua poin dan map ke DTO.
        /// </summary>
        public async Task<IEnumerable<PoinKetentuanKhususDto>> GetAllAsync()
        {
            var poinList = await _unitOfWork.PoinKetentuanKhusus.GetAllAsync();
            return poinList.Select(MapToDto);
        }

        /// <summary>
        /// Ambil poin berdasarkan ID. Jika tidak ditemukan, lempar KeyNotFoundException.
        /// </summary>
        public async Task<PoinKetentuanKhususDto> GetByIdAsync(int id)
        {
            var poin = await _unitOfWork.PoinKetentuanKhusus.GetByIdAsync(id);
            if (poin == null)
            {
                throw new KeyNotFoundException($"PoinKetentuanKhusus dengan ID {id} tidak ditemukan.");
            }
            return MapToDto(poin);
        }

        /// <summary>
        /// Buat Poin baru. Memastikan SubBab (parent) ada sebelum menyimpan.
        /// </summary>
        public async Task<PoinKetentuanKhususDto> CreateAsync(CreatePoinKetentuanKhususDto createDto)
        {
            // Pastikan SubBab terkait ada
            var subBabExists = await _unitOfWork.SubBabKetentuanKhusus.GetByIdAsync(createDto.IdSubBab);
            if (subBabExists == null)
            {
                throw new ArgumentException($"SubBabKetentuanKhusus dengan ID {createDto.IdSubBab} tidak ditemukan.");
            }

            var newPoin = new PoinKetentuanKhusus
            {
                IdSubBab = createDto.IdSubBab,
                TeksPoin = createDto.TeksPoin,
                Parent = createDto.Parent,
                UrutanTampil = createDto.UrutanTampil
            };

            await _unitOfWork.PoinKetentuanKhusus.AddAsync(newPoin);
            await _unitOfWork.CompleteAsync();

            return MapToDto(newPoin);
        }

        /// <summary>
        /// Perbarui Poin yang sudah ada. Memastikan entitas sumber dan SubBab baru ada.
        /// </summary>
        public async Task UpdateAsync(int id, UpdatePoinKetentuanKhususDto updateDto)
        {
            var poinToUpdate = await _unitOfWork.PoinKetentuanKhusus.GetByIdAsync(id);
            if (poinToUpdate == null)
            {
                throw new KeyNotFoundException($"PoinKetentuanKhusus dengan ID {id} tidak ditemukan.");
            }

            // Pastikan SubBab baru ada
            var subBabExists = await _unitOfWork.SubBabKetentuanKhusus.GetByIdAsync(updateDto.IdSubBab);
            if (subBabExists == null)
            {
                throw new ArgumentException($"SubBabKetentuanKhusus dengan ID {updateDto.IdSubBab} tidak ditemukan.");
            }

            // Perbarui properti entitas
            poinToUpdate.IdSubBab = updateDto.IdSubBab;
            poinToUpdate.TeksPoin = updateDto.TeksPoin;
            poinToUpdate.Parent = updateDto.Parent;
            poinToUpdate.UrutanTampil = updateDto.UrutanTampil;

            _unitOfWork.PoinKetentuanKhusus.Update(poinToUpdate);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Hapus Poin berdasarkan ID. Jika tidak ada, lempar KeyNotFoundException.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var poinToDelete = await _unitOfWork.PoinKetentuanKhusus.GetByIdAsync(id);
            if (poinToDelete == null)
            {
                throw new KeyNotFoundException($"PoinKetentuanKhusus dengan ID {id} tidak ditemukan.");
            }

            _unitOfWork.PoinKetentuanKhusus.Delete(poinToDelete);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Helper untuk mapping entitas ke DTO agar tidak duplikasi kode.
        /// </summary>
        private PoinKetentuanKhususDto MapToDto(PoinKetentuanKhusus poin)
        {
            return new PoinKetentuanKhususDto
            {
                Id = poin.Id,
                IdSubBab = poin.IdSubBab,
                TeksPoin = poin.TeksPoin,
                Parent = poin.Parent,
                UrutanTampil = poin.UrutanTampil
            };
        }
    }
}
