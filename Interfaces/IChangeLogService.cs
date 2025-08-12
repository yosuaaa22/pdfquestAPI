using System.Threading.Tasks;

namespace pdfquestAPI.Interfaces
{
    public interface IChangeLogService
    {
        Task LogAsync(int perjanjianId, string pengguna, string aksi, string deskripsi);
    }
}