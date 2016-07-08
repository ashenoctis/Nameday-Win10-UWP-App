using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Nameday
{
    public class ContactEx
    {
        public Contact Contact { get; }

        public ContactEx (Contact contact)
        {
            Contact = contact;
        }

        public ContactEx(string firsName, string lastName)
        {
            Contact = new Contact
            {
                FirstName = firsName,
                LastName = lastName
            };
        }

        private string GetFirstCharacter(string s) => string.IsNullOrEmpty(s) ? "" : s.Substring(0, 1);

        public string Initials => GetFirstCharacter(Contact.FirstName) + GetFirstCharacter(Contact.LastName);

        public Visibility EmailVisibility => DesignMode.DesignModeEnabled || Contact.Emails.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        public ImageBrush Picture
        {
            get
            {
                if (Contact.SmallDisplayPicture == null) //No thumbnail
                    return null;

                var image = new BitmapImage();

                image.SetSource(Contact.SmallDisplayPicture.OpenReadAsync().GetAwaiter().GetResult());

                return new ImageBrush { ImageSource = image };
            }
        } 

    }
}
