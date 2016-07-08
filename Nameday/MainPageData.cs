using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Popups;

namespace Nameday
{
    class MainPageData : INotifyPropertyChanged
    {
        private string _greeting { get; set; } = "Hey there!!";
        public event PropertyChangedEventHandler PropertyChanged; //for any property changed

        private List<NameDayModel> _allNamedays = new List<NameDayModel>(); 
        public ObservableCollection<NameDayModel> Namedays { get; set; } //Dynamic Lists

        public ObservableCollection<ContactEx> Contacts { get; } = new ObservableCollection<ContactEx>();

        //constructor
        public MainPageData()
        {
            Namedays = new ObservableCollection<NameDayModel>();

            if(Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Contacts = new ObservableCollection<ContactEx>
                {
                    new ContactEx("Contact", "1"),
                    new ContactEx("Contact", "2"),
                    new ContactEx("Contact", "3")
                };

                for (int month = 1; month <= 12; month++)
                {
                    _allNamedays.Add(new NameDayModel(month, 1, new string[] { "Sharma" }));
                    _allNamedays.Add(new NameDayModel(month, 24, new string[] { "Raikar", "Verma" }));
                }

                performFiltering(); //filter to show full list
            }

            else
                LoadData();                        
        }

        private async void LoadData()
        {
            _allNamedays = await NamedayRepository.GetAllNamedaysAsync();
            performFiltering();            
        }

        //Storing & chekcing if parameter changed
        public string Greeting
        {
            get { return _greeting; }
            set { if (value == _greeting)
                    return;

                _greeting = value;
                /*if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Hey bud!!"));*/
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Greeting)));
            }
        }        

        //Filter search results
        private void performFiltering()
        {
             if (_filter == null)
              _filter = "";            

            //Converting searched string & list items to lowercase
            var lowerCaseFilter = Filter.ToLowerInvariant().Trim();
            //LINQ to get searched items
            var result = _allNamedays.Where(d => d.NameAsString.ToLowerInvariant().Contains(lowerCaseFilter)).ToList();
            
            //Getting data & removing unwanted data    
            var toRemove = Namedays.Except(result).ToList();
            foreach (var x in toRemove)
                Namedays.Remove(x);

            //getting old data back
            var resultCount = result.Count;
            for(int i=0; i<resultCount; i++)
            {
                var reultItem = result[i];
                if (i + 1 > Namedays.Count || !Namedays[i].Equals(reultItem))
                    Namedays.Insert(i, reultItem);
            }
        }

        //How selected nameday will be processed
        private NameDayModel _selectedNameday;
        public NameDayModel SelectedNameday
            {
                get { return _selectedNameday; }
                set {
                        _selectedNameday = value;

                        if (value == null)
                            Greeting = "Hi!!";
                        else
                            Greeting = "Hello " + value.NameAsString;

                           UpdateContact();
                     }
            }

        private async void UpdateContact()
        {
            Contacts.Clear();

            if(SelectedNameday != null)
            {
                var contactStore = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);

                foreach (var name in SelectedNameday.Names)
                    foreach (var contact in await contactStore.FindContactsAsync(name))
                        Contacts.Add(new ContactEx(contact));
            }
        }

        //Searching filter
        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set
            {
                if (value == _filter)
                    return;

                _filter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Filter));

                performFiltering();
            }
        }
      }        
    }
