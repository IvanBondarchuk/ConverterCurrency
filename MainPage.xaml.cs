using ConverterCurrency.ViewModel;
using static System.Net.Mime.MediaTypeNames;

namespace ConverterCurrency
{
    public partial class MainPage : ContentPage
    {
        private MainViewModel _vm;
        public MainPage()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            BindingContext = _vm;
        }

        private void SwapValuteFunc(object sender, EventArgs e)
        {
            if(_vm.SelectedFromCurrency != _vm.SelectedToCurrency && _vm.SelectedFromCurrency != null || _vm.SelectedToCurrency != null)
            {
                var swap = _vm.SelectedFromCurrency;

                _vm.SelectedFromCurrency = _vm.SelectedToCurrency;
                _vm.SelectedToCurrency = swap;
            }
        }

        private void NumberEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                bool isWholeNumber = double.TryParse(e.NewTextValue, out double value) && value > 0;
                if (!isWholeNumber)
                {
                    ((Entry)sender).Text = e.OldTextValue;
                }
            }
            else
            {
                ((Entry)sender).Text = null;
            }
        }
    }
}
