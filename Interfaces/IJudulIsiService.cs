using pdfquestAPI.Dtos.JudulIsi;


namespace pdfquestAPI.Interfaces
{
    public interface IJudulIsiService
    {
        Task<IEnumerable<JudulIsiDto>> GetAllAsync();
        Task<JudulIsiDto> GetByIdAsync(int id);
        Task<JudulIsiDto> CreateAsync(CreateJudulIsiDto createDto);
        Task UpdateAsync(int id, UpdateJudulIsiDto updateDto);
        Task DeleteAsync(int id);
    }
}