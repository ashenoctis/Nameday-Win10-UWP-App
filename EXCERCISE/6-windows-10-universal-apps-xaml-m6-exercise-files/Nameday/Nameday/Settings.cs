using System;
using System.Globalization;
using Windows.Storage;
using System.ComponentModel;

namespace Nameday
{
    public class Settings : ObservableObject
    {
        private bool _notificationsEnabled = false;

        public bool NotificationsEnabled
        {
            get { return _notificationsEnabled; }
            set
            {
                if (Set(ref _notificationsEnabled, value))
                    _localSettings.Values[nameof(NotificationsEnabled)] = value;
            }
        }

        private bool _updatingLiveTileEnabled = false;

        public bool UpdatingLiveTileEnabled
        {
            get { return _updatingLiveTileEnabled; }
            set
            {
                if (Set(ref _updatingLiveTileEnabled, value))
                    _localSettings.Values[nameof(UpdatingLiveTileEnabled)] = value;
            }
        }

        private DateTime _lastSuccessfulRun = DateTime.MinValue;

        public DateTime LastSuccessfulRun
        {
            get { return _lastSuccessfulRun; }
            set
            {
                if (Set(ref _lastSuccessfulRun, value))
                    _localSettings.Values[nameof(LastSuccessfulRun)] =
                        value.ToString(CultureInfo.InvariantCulture);
            }
        }

        public Settings()
        {
            LoadSettings();
        }

        private readonly ApplicationDataContainer _localSettings =
            ApplicationData.Current.LocalSettings;

        public event PropertyChangedEventHandler PropertyChanged;

        private void LoadSettings()
        {
            var notificationsEnabled = _localSettings.Values[nameof(NotificationsEnabled)];
            if (notificationsEnabled != null)
                NotificationsEnabled = (bool)notificationsEnabled;

            var updatingLiveTileEnabled = _localSettings.Values[nameof(UpdatingLiveTileEnabled)];
            if (updatingLiveTileEnabled != null)
                UpdatingLiveTileEnabled = (bool)updatingLiveTileEnabled;

            var lastSuccessfulRun = _localSettings.Values[nameof(LastSuccessfulRun)];
            if (lastSuccessfulRun != null)
                LastSuccessfulRun = DateTime.Parse(lastSuccessfulRun.ToString(), 
                    CultureInfo.InvariantCulture);
        }
    }
}
