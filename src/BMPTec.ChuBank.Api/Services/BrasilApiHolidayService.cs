using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace BMPTec.ChuBank.Api.Services
{
    public class BrasilApiHolidayService : IHolidayService
    {
        private readonly HttpClient _http;
        private readonly IMemoryCache _cache;
        private readonly string _url;

        public BrasilApiHolidayService(HttpClient http, IConfiguration config, IMemoryCache cache)
        {
            _http = http;
            _cache = cache;
            _url = config["BrasilApi:HolidaysUrl"] ?? "https://brasilapi.com.br/api/feriados/v1/2025";
        }

        public async Task<bool> IsBusinessDayAsync(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                return false;

            var holidays = await _cache.GetOrCreateAsync("holidays_2025", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);
                var list = await _http.GetFromJsonAsync<List<HolidayDto>>(_url);
                return list ?? new List<HolidayDto>();
            });

            return !holidays.Any(h => DateTime.Parse(h.date).Date == date.Date);
        }

        private record HolidayDto(string date, string name);
    }
}
