using System.Collections.Generic;
using System.Threading.Tasks;
using pdfquestAPI.Dtos;
using pdfquestAPI.Interfaces;
using pdfquestAPI.Models;

namespace pdfquestAPI.Services
{
    /// <summary>
    /// Service for managing PenyediaLayanan entities.
    /// Uses an IUnitOfWork to perform repository operations and persist changes.
    /// </summary>
    public class PenyediaLayananService : IPenyediaLayananService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Create a new instance of the service with the provided unit of work.
        /// </summary>
        public PenyediaLayananService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieve all PenyediaLayanan records.
        /// </summary>
        public async Task<IEnumerable<PenyediaLayanan>> GetAllAsync()
            => await _unitOfWork.PenyediaLayanan.GetAllAsync();

        /// <summary>
        /// Retrieve a single PenyediaLayanan by its identifier.
        /// Returns null if not found.
        /// </summary>
        public async Task<PenyediaLayanan?> GetByIdAsync(int id)
            => await _unitOfWork.PenyediaLayanan.GetByIdAsync(id);

        /// <summary>
        /// Create a new PenyediaLayanan from the provided DTO and persist it.
        /// Maps DTO fields to the domain model, adds the entity, then commits the unit of work.
        /// </summary>
        public async Task<PenyediaLayanan> CreateAsync(CreatePenyediaLayananDto dto)
        {
            // Map incoming DTO to the domain model
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

            // Add the entity and persist changes
            await _unitOfWork.PenyediaLayanan.AddAsync(penyediaLayanan);
            await _unitOfWork.CompleteAsync();

            return penyediaLayanan;
        }
    }
}