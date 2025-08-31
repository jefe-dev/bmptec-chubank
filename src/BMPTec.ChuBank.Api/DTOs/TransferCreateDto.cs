namespace BMPTec.ChuBank.Api.DTOs { 
    public record TransferCreateDto(Guid FromAccountId, Guid ToAccountId, decimal Amount); 
}