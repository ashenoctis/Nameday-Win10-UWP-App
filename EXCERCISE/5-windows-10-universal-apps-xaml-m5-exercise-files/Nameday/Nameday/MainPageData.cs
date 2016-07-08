using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nameday
{
    public class MainPageData : INotifyPropertyChanged
    {
        private string _greeting = "Hello world";

        private List<NamedayModel> _allNamedays = 
            new List<NamedayModel>();

        public ObservableCollection<NamedayModel> Namedays { get; set; }

        public MainPageData()
        {
            Namedays = new ObservableCollection<NamedayModel>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                for (int month = 1; month <= 12; month++)
                {
                    _allNamedays.Add(new NamedayModel(
                        month, 1, new string[] { "Adam" }));
                    _allNamedays.Add(new NamedayModel(
                        month, 24, new string[] { "Eve", "Andrew" }));
                }

                PerformFiltering();
            }
            else
                LoadData();
        }

        public async void LoadData()
        {
            _allNamedays = await NamedayRepository.GetAllNamedaysAsync();
            PerformFiltering();
        }


        public string Greeting
        {
            get { return _greeting; }
            set
            {
                if (value == _greeting)
                    return;

                _greeting = value;
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(nameof(Greeting)));
            }
        }

        private NamedayModel _selectedNameday;

        public event PropertyChangedEventHandler PropertyChanged;

        public NamedayModel SelectedNameday
        {
            get { return _selectedNameday; }
            set
            {
                _selectedNameday = value;

                if (value == null)
                    Greeting = "Hello World!";
                else
                    Greeting = "Hello " + value.NamesAsString;
            }
        }

        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set
            {
                if (value == _filter)
                    return;

                _filter = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(Filter)));

                PerformFiltering();
            }
        }

        private void PerformFiltering()
        {
            if (_filter == null)
                _filter = "";

            var lowerCaseFilter = Filter.ToLowerInvariant().Trim();

            var result =
                _allNamedays.Where(d => d.NamesAsString.ToLowerInvariant()
                .Contains(lowerCaseFilter))
                .ToList();

            var toRemove = Namedays.Except(result).ToList();

            foreach (var x in toRemove)
                Namedays.Remove(x);

            var resultCount = result.Count;
            for (int i = 0; i < resultCount; i++)
            {
                var resultItem = result[i];
                if (i + 1 > Namedays.Count || !Namedays[i].Equals(resultItem))
                    Namedays.Insert(i, resultItem);
            }
        }


    }
}
