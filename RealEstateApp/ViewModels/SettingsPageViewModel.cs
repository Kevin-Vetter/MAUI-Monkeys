
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
		private int _timesStarted;

        public SettingsPageViewModel()
        {
            Debug.WriteLine(Preferences.Default.Get(nameof(TimesStarted), -1));
            SettingString = Preferences.Default.Get(nameof(SettingString),"");
        }
        public int TimesStarted
		{
			get { return _timesStarted; }
			set { SetProperty(ref _timesStarted, value); }
		}

        private string _settingString;

        public string SettingString
        {
            get { return _settingString; }
            set { SetProperty(ref _settingString, value); }
        }

        private string _GUID;

        public string GUID
        {
            get { return _GUID; }
            set { SetProperty(ref _GUID, value); }
        }

        private Command _saveSettings;
        public Command SaveSettings => _saveSettings ??= new Command(async () => {
            Preferences.Default.Set(nameof(SettingString), SettingString);
            await SecureStorage.Default.SetAsync(nameof(GUID), Guid.NewGuid().ToString());
            GUID = await SecureStorage.Default.GetAsync(nameof(GUID));
        });


    }
}
