using pdfquestAPI.Dtos;
using pdfquestAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pdfquestAPI.Interfaces
{
    public interface IPihakPertamaService
    {
        Task<IEnumerable<PihakPertama>> GetAllAsync();
        Task<PihakPertama?> GetByIdAsync(int id);
        Task<PihakPertama> CreateAsync(CreatePihakPertamaDto dto);
    }
}