namespace pdfquestAPI.Interfaces
{
    using pdfquestAPI.Dtos.SubBabKetentuanKhusus;

    public interface ISubBabKetentuanKhususService
    {
        Task<IEnumerable<SubBabKetentuanKhususDto>> GetAllAsync();
        Task<SubBabKetentuanKhususDto> GetByIdAsync(int id);
        Task<SubBabKetentuanKhususDto> CreateAsync(CreateSubBabKetentuanKhususDto createDto);
        Task UpdateAsync(int id, UpdateSubBabKetentuanKhususDto updateDto);
        Task DeleteAsync(int id);
    }
}
