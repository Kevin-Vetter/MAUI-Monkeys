using AppProg1.Models;
using AppProg1.Services;
using AppProg1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppProg1.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private MonkeyService _monkeyService;
        public ObservableCollection<Monkey> Monkeys { get; set; } = new ObservableCollection<Monkey>();
        public MainPageViewModel(MonkeyService ms)
        {
            Title = "Monkey Finder";
            _monkeyService = ms;
        }

        #region Commands
        private Command _goToMonkeyDetails;
        private Command _getMonkeysCommand;

        public ICommand GoToMonkeyDetails =>
                        _goToMonkeyDetails ??= new Command<Monkey>(async x =>
                        {
                            Dictionary<string, object> dic = new Dictionary<string, object>  { { nameof(Monkey), x } };
                            await Shell.Current.GoToAsync(nameof(MonkeyDetails), dic);
                        });

        public ICommand GetMonkeysCommand =>
                        _getMonkeysCommand ??= new Command(async () => await GetMonkeysAsync());
        async Task GetMonkeysAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                IsLoading = true;
                var monkeys = await _monkeyService.GetMonkeys();

                if (Monkeys.Count != 0)
                    Monkeys.Clear();

                foreach (var monkey in monkeys)
                    Monkeys.Add(monkey);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get monkeys: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                IsLoading = false;
            }
        }
        #endregion
    }
}
