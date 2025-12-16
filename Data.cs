using ConverterCurrency.Model;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConverterCurrency.Data
{
    internal class DataSer
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<DateTime, ExchangeRatesResponse> _cache = new();

        public DataSer()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ExchangeRatesResponse?> GetCurrencyRatesAsync(DateTime date)
        {
            if (_cache.TryGetValue(date.Date, out var cachedResponse))
            {
                return cachedResponse;
            }
            try
            {
                var url = date.Date == DateTime.Today
                    ? "https://www.cbr-xml-daily.ru/daily_json.js"
                    : $"https://www.cbr-xml-daily.ru/archive/{date.Year}/{date.Month:00}/{date.Day:00}/daily_json.js";


                var response = await new HttpClient().GetStringAsync(url);
                var currencyResponse = JsonSerializer.Deserialize<ExchangeRatesResponse>(response);


                if (currencyResponse != null)
                {
                    currencyResponse.Valutes["RUB"] = new Currency
                    {
                        ID = "R00001",
                        NumCode = "643",
                        CharCode = "RUB",
                        Nominal = 1,
                        Name = "Российский рубль",
                        Value = 1
                    };

                    _cache[date.Date] = currencyResponse;
                }

                return currencyResponse;
            }
            catch (Exception ex)
            {
                int retries = 0;
                do
                {
                    retries++;
                    date = date.AddDays(-1);
                    return await GetCurrencyRatesAsync(date);
                } while (retries < 31);

                // Если достигнут предел, выбросить исключение вверх по стеку
                throw new Exception($"Не удалось получить данные за период нескольких дней назад", ex);
            }
        }

        public double ConvertCurrency(double amount, Currency fromCurrency, Currency toCurrency)
        {
            if (fromCurrency == null || toCurrency == null)
                return 0;
            var convertedAmount = amount * (fromCurrency.Value / fromCurrency.Nominal) / (toCurrency.Value / toCurrency.Nominal);

            return Math.Round(convertedAmount, 2);
        }
    }
}
