using pdfquestAPI.Dtos;
using pdfquestAPI.Interfaces;
using pdfquestAPI.Models;

namespace pdfquestAPI.Services
{
    // File: Services/PenyediaLayananService.cs
    public class PenyediaLayananService : IPenyediaLayananService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PenyediaLayananService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        public async Task<IEnumerable<PenyediaLayanan>> GetAllAsync() => await _unitOfWork.PenyediaLayanan.GetAllAsync();
        public async Task<PenyediaLayanan?> GetByIdAsync(int id) => await _unitOfWork.PenyediaLayanan.GetByIdAsync(id);

        public async Task<PenyediaLayanan> CreateAsync(CreatePenyediaLayananDto dto)
        {
            var penyediaLayanan = new PenyediaLayanan
            {
                NamaEntitasCalonProvider = dto.NamaEntitasCalonProvider,
                JenisPerusahaan = dto.JenisPerusahaan,
                NoNibPihakKedua = dto.NoNibPihakKedua,
                AlamatPemegangPolis = dto.AlamatPemegangPolis,
                NamaPerwakilan = dto.NamaPerwakilan,
                JabatanPerwakilan = dto.JabatanPerwakilan,
                JenisFasilitas = dto.JenisFasilitas,
                NamaFasilitasKesehatan = dto.NamaFasilitasKesehatan,
                NamaDokumenIzin = dto.NamaDokumenIzin,
                NomorDokumenIzin = dto.NomorDokumenIzin,
                TanggalDokumenIzin = dto.TanggalDokumenIzin,
                InstansiPenerbitIzin = dto.InstansiPenerbitIzin,
                NamaBank = dto.NamaBank,
                NamaCabangBank = dto.NamaCabangBank,
                NomorRekening = dto.NomorRekening,
                PemilikRekening = dto.PemilikRekening,
                NamaPemilikNpwp = dto.NamaPemilikNpwp,
                NoNpwp = dto.NoNpwp,
                JenisNpwp = dto.JenisNpwp
            };
            await _unitOfWork.PenyediaLayanan.AddAsync(penyediaLayanan);
            await _unitOfWork.CompleteAsync();
            return penyediaLayanan;
        }
    }

}