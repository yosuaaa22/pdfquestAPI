namespace pdfquestAPI.Interfaces
{
    using pdfquestAPI.Dtos.Perjanjian;

    public interface IPerjanjianService
    {
        Task<IEnumerable<PerjanjianDto>> GetAllAsync();
        Task<PerjanjianDto> GetByIdAsync(int id);
        Task<PerjanjianDto> CreateAsync(CreatePerjanjianDto createDto);
        Task UpdateAsync(int id, UpdatePerjanjianDto updateDto);
        Task DeleteAsync(int id);
    }
}