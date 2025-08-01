namespace pdfquestAPI.Services
{
    using pdfquestAPI.Dtos.JudulIsi;
    using pdfquestAPI.Interfaces;
    using pdfquestAPI.Models;

    public class JudulIsiService : IJudulIsiService
    {
        private readonly IUnitOfWork _unitOfWork;

        public JudulIsiService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<JudulIsiDto>> GetAllAsync()
        {
            var judulIsiList = await _unitOfWork.JudulIsi.GetAllAsync();
            
            return judulIsiList.Select(j => new JudulIsiDto
            {
                Id = j.Id,
                IdPenyedia = j.IdPenyedia, // SUDAH BENAR
                JudulTeks = j.JudulTeks,
                UrutanTampil = j.UrutanTampil
            });
        }

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
                IdPenyedia = judulIsi.IdPenyedia, // SUDAH BENAR
                JudulTeks = judulIsi.JudulTeks,
                UrutanTampil = judulIsi.UrutanTampil
            };
        }

        public async Task<JudulIsiDto> CreateAsync(CreateJudulIsiDto createDto)
        {
            var newJudulIsi = new JudulIsi
            {
                IdPenyedia = createDto.IdPenyedia, // SUDAH BENAR
                JudulTeks = createDto.JudulTeks,
                UrutanTampil = createDto.UrutanTampil
            };

            await _unitOfWork.JudulIsi.AddAsync(newJudulIsi);
            await _unitOfWork.CompleteAsync();

            return await GetByIdAsync(newJudulIsi.Id);
        }

        public async Task UpdateAsync(int id, UpdateJudulIsiDto updateDto)
        {
            var judulIsiToUpdate = await _unitOfWork.JudulIsi.GetByIdAsync(id);
            if (judulIsiToUpdate == null)
            {
                throw new KeyNotFoundException($"JudulIsi dengan ID {id} tidak ditemukan.");
            }

            judulIsiToUpdate.IdPenyedia = updateDto.IdPenyedia; // SUDAH BENAR
            judulIsiToUpdate.JudulTeks = updateDto.JudulTeks;
            judulIsiToUpdate.UrutanTampil = updateDto.UrutanTampil;

            _unitOfWork.JudulIsi.Update(judulIsiToUpdate);
            await _unitOfWork.CompleteAsync();
        }

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
