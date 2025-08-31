using BMPTec.ChuBank.Api.DTOs;

namespace BMPTec.ChuBank.Api.Services
{
    public interface ITransferService
    {
        Task<TransferResult> TransferAsync(TransferCreateDto dto);
    }
    public record TransferResult(bool Success, string? Message);
}
