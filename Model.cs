using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;
namespace ConverterCurrency.Model
{
    public class Currency
    {
        [JsonPropertyName("ID")]
        public string? ID { get; set; }
        [JsonPropertyName("NumCode")]
        public string? NumCode { get; set; }
        [JsonPropertyName("CharCode")]
        public string? CharCode { get; set; }
        [JsonPropertyName("Nominal")]
        public double Nominal { get; set; }
        [JsonPropertyName("Name")]
        public string? Name { get; set; }
        [JsonPropertyName("Value")]
        public double Value { get; set; }
    }

    public class ExchangeRatesResponse
    {
        [JsonPropertyName("Date")]
        public DateTime Date { get; set; }


        [JsonPropertyName("Timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("Valute")]
        public Dictionary<string, Currency> Valutes { get; set; } = new();
    }
}
