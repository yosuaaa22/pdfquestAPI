using pdfquestAPI.Dtos;
using pdfquestAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pdfquestAPI.Interfaces
{
    public interface IPenyediaLayananService
    {
        Task<IEnumerable<PenyediaLayanan>> GetAllAsync();
        Task<PenyediaLayanan?> GetByIdAsync(int id);
        Task<PenyediaLayanan> CreateAsync(CreatePenyediaLayananDto dto);
    }
}