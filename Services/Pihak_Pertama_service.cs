using pdfquestAPI.Interfaces;
using pdfquestAPI.Models;
using pdfquestAPI.Dtos;

public class PihakPertamaService : IPihakPertamaService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PihakPertamaService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
        
        public async Task<IEnumerable<PihakPertama>> GetAllAsync() => await _unitOfWork.PihakPertama.GetAllAsync();
        public async Task<PihakPertama?> GetByIdAsync(int id) => await _unitOfWork.PihakPertama.GetByIdAsync(id);

        public async Task<PihakPertama> CreateAsync(CreatePihakPertamaDto dto)
        {
            var pihakPertama = new PihakPertama
            {
                NomorNibPihakPertama = dto.NomorNibPihakPertama,
                NamaPerwakilanPihakPertama = dto.NamaPerwakilanPihakPertama,
                JabatanPerwakilanPihakPertama = dto.JabatanPerwakilanPihakPertama
            };
            await _unitOfWork.PihakPertama.AddAsync(pihakPertama);
            await _unitOfWork.CompleteAsync();
            return pihakPertama;
        }
    }
    