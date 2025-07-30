namespace pdfquestAPI.Interfaces
{
    using pdfquestAPI.Dtos.PoinKetentuanKhusus;

    public interface IPoinKetentuanKhususService
    {
        Task<IEnumerable<PoinKetentuanKhususDto>> GetAllAsync();
        Task<PoinKetentuanKhususDto> GetByIdAsync(int id);
        Task<PoinKetentuanKhususDto> CreateAsync(CreatePoinKetentuanKhususDto createDto);
        Task UpdateAsync(int id, UpdatePoinKetentuanKhususDto updateDto);
        Task DeleteAsync(int id);
    }
}