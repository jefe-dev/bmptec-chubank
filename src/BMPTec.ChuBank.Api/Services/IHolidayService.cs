namespace BMPTec.ChuBank.Api.Services
{
    public interface IHolidayService
    {
        Task<bool> IsBusinessDayAsync(DateTime date);
    }
}
