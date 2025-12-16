using ConverterCurrency.Data;
using ConverterCurrency.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ConverterCurrency.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class MainViewModel : BaseViewModel
    {
        private readonly DataSer _dataser;
        private ExchangeRatesResponse? _currentRates;
        private DateTime _selectedDate = DateTime.Today;
        private Currency? _selectedFromCurrency;
        private Currency? _selectedToCurrency;
        private double _amount;
        private double _convertedAmount = 0;

        private bool _isBusy;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<Currency> Currencies { get; } = new();
        public ObservableCollection<Currency> FilteredCurrencies { get; } = new();


        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public Currency? SelectedFromCurrency
        {
            get => _selectedFromCurrency;
            set
            {
                if (_selectedFromCurrency != value)
                {
                    _selectedFromCurrency = value;
                    OnPropertyChanged();
                    ConvertCurrency();
                }
            }
        }

        public Currency? SelectedToCurrency
        {
            get => _selectedToCurrency;
            set
            {
                if (_selectedToCurrency != value)
                {
                    _selectedToCurrency = value;
                    OnPropertyChanged();
                    ConvertCurrency();
                }
            }
        }

        public double Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged();
                    ConvertCurrency();
                }
            }
        }

        public double ConvertedAmount
        {
            get => _convertedAmount;
            set
            {
                if (_convertedAmount != value)
                {
                    _convertedAmount = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                    LoadAvailableCurrenciesAsync();
                }
            }
        }

        public MainViewModel()
        {
            _dataser = new DataSer();
            LoadAvailableCurrenciesAsync();
        }

        private async void LoadAvailableCurrenciesAsync()
        {
            IsBusy = true;
            string tempFromCurrency = string.Empty;
            string temptoCurrency = string.Empty;
            if (SelectedFromCurrency != null && SelectedToCurrency != null)
            {
                tempFromCurrency = SelectedFromCurrency.CharCode;
                temptoCurrency = SelectedToCurrency.CharCode;
            }

            try
            {
                _currentRates = await _dataser.GetCurrencyRatesAsync(SelectedDate);

                if (_currentRates?.Valutes != null)
                {
                    Currencies.Clear();
                    FilteredCurrencies.Clear();
                    foreach (var currency in _currentRates.Valutes.Values.OrderBy(c => c.CharCode))
                    {
                        Currencies.Add(currency);
                        FilteredCurrencies.Add(currency);
                    }
                }
                SelectedFromCurrency = Currencies.FirstOrDefault(c => c.CharCode == tempFromCurrency);
                SelectedToCurrency = Currencies.FirstOrDefault(c => c.CharCode == temptoCurrency);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void ConvertCurrency()
        {
            if (SelectedFromCurrency == null || SelectedToCurrency == null || Amount == 0)
            {
                ConvertedAmount = 0;
                return;
            }
            try
            {
                ConvertedAmount = _dataser.ConvertCurrency(
                    Amount,
                    SelectedFromCurrency,
                    SelectedToCurrency);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ConvertedAmount = 0;
            }
        }
    }
}
