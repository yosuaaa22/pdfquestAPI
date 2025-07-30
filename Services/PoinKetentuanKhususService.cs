namespace pdfquestAPI.Services
{
    using pdfquestAPI.Dtos.PoinKetentuanKhusus;
    using pdfquestAPI.Interfaces;
    using pdfquestAPI.Models;

    public class PoinKetentuanKhususService : IPoinKetentuanKhususService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PoinKetentuanKhususService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PoinKetentuanKhususDto>> GetAllAsync()
        {
            var poinList = await _unitOfWork.PoinKetentuanKhusus.GetAllAsync();
            return poinList.Select(p => MapToDto(p));
        }

        public async Task<PoinKetentuanKhususDto> GetByIdAsync(int id)
        {
            var poin = await _unitOfWork.PoinKetentuanKhusus.GetByIdAsync(id);
            if (poin == null)
            {
                throw new KeyNotFoundException($"PoinKetentuanKhusus dengan ID {id} tidak ditemukan.");
            }
            return MapToDto(poin);
        }

        public async Task<PoinKetentuanKhususDto> CreateAsync(CreatePoinKetentuanKhususDto createDto)
        {
            // Validasi: Pastikan SubBab (parent) ada sebelum membuat Poin
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

        public async Task UpdateAsync(int id, UpdatePoinKetentuanKhususDto updateDto)
        {
            var poinToUpdate = await _unitOfWork.PoinKetentuanKhusus.GetByIdAsync(id);
            if (poinToUpdate == null)
            {
                throw new KeyNotFoundException($"PoinKetentuanKhusus dengan ID {id} tidak ditemukan.");
            }

            // Validasi: Pastikan SubBab (parent) yang baru juga ada
            var subBabExists = await _unitOfWork.SubBabKetentuanKhusus.GetByIdAsync(updateDto.IdSubBab);
            if (subBabExists == null)
            {
                throw new ArgumentException($"SubBabKetentuanKhusus dengan ID {updateDto.IdSubBab} tidak ditemukan.");
            }

            // Update properti
            poinToUpdate.IdSubBab = updateDto.IdSubBab;
            poinToUpdate.TeksPoin = updateDto.TeksPoin;
            poinToUpdate.Parent = updateDto.Parent;
            poinToUpdate.UrutanTampil = updateDto.UrutanTampil;

            _unitOfWork.PoinKetentuanKhusus.Update(poinToUpdate);
            await _unitOfWork.CompleteAsync();
        }

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

        // Helper method untuk mapping agar tidak duplikat kode
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
