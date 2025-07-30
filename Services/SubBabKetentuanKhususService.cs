namespace pdfquestAPI.Services
{
    using pdfquestAPI.Dtos.SubBabKetentuanKhusus;
    using pdfquestAPI.Interfaces;
    using pdfquestAPI.Models;

    public class SubBabKetentuanKhususService : ISubBabKetentuanKhususService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubBabKetentuanKhususService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SubBabKetentuanKhususDto>> GetAllAsync()
        {
            var subBabList = await _unitOfWork.SubBabKetentuanKhusus.GetAllAsync();
            return subBabList.Select(sb => MapToDto(sb));
        }

        public async Task<SubBabKetentuanKhususDto> GetByIdAsync(int id)
        {
            var subBab = await _unitOfWork.SubBabKetentuanKhusus.GetByIdAsync(id);
            if (subBab == null)
            {
                throw new KeyNotFoundException($"SubBabKetentuanKhusus dengan ID {id} tidak ditemukan.");
            }
            return MapToDto(subBab);
        }

        public async Task<SubBabKetentuanKhususDto> CreateAsync(CreateSubBabKetentuanKhususDto createDto)
        {
            // Validasi: Pastikan Judul (parent) ada sebelum membuat SubBab
            var judulExists = await _unitOfWork.JudulIsi.GetByIdAsync(createDto.IdJudul);
            if (judulExists == null)
            {
                throw new ArgumentException($"JudulIsi dengan ID {createDto.IdJudul} tidak ditemukan.");
            }

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

        public async Task UpdateAsync(int id, UpdateSubBabKetentuanKhususDto updateDto)
        {
            var subBabToUpdate = await _unitOfWork.SubBabKetentuanKhusus.GetByIdAsync(id);
            if (subBabToUpdate == null)
            {
                throw new KeyNotFoundException($"SubBabKetentuanKhusus dengan ID {id} tidak ditemukan.");
            }

            // Validasi: Pastikan Judul (parent) yang baru juga ada
            var judulExists = await _unitOfWork.JudulIsi.GetByIdAsync(updateDto.IdJudul);
            if (judulExists == null)
            {
                throw new ArgumentException($"JudulIsi dengan ID {updateDto.IdJudul} tidak ditemukan.");
            }

            // Update properti
            subBabToUpdate.IdJudul = updateDto.IdJudul;
            subBabToUpdate.Konten = updateDto.Konten;
            subBabToUpdate.UrutanTampil = updateDto.UrutanTampil;

            _unitOfWork.SubBabKetentuanKhusus.Update(subBabToUpdate);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var subBabToDelete = await _unitOfWork.SubBabKetentuanKhusus.GetByIdAsync(id);
            if (subBabToDelete == null)
            {
                throw new KeyNotFoundException($"SubBabKetentuanKhusus dengan ID {id} tidak ditemukan.");
            }

            _unitOfWork.SubBabKetentuanKhusus.Delete(subBabToDelete);
            await _unitOfWork.CompleteAsync();
        }

        // Helper method untuk mapping agar tidak duplikat kode
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
