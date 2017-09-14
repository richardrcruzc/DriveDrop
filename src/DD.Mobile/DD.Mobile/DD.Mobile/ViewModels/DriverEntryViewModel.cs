using DD.Mobile.Models.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DD.Mobile.ViewModels
{
    public class DriverEntryViewModel: BaseDriverViewModel
    {

        public DriverEntryViewModel() {

            FirstName = "New Driver";

        }


        Command _saveCommand;
        public Command SaveCommand
        {
            get {
                return _saveCommand ?? (_saveCommand = new Command(ExecuteSaveCommand, ValidateFormDetails));
            }

        }
        void ExecuteSaveCommand()
        {
            var newDriverItem = new DriverEntry
            {
                FirstName =this.FirstName,
                LastName= this.LastName,
                Latitude= this.Latitude,
                longitude=this.Longitude,
                PersonalPhotoUri = this.ImageUrl
            };
        }
        bool ValidateFormDetails()
        {
            return !string.IsNullOrWhiteSpace(FirstName);
            

        }

        string _fName;
        public string FirstName
        {
            get { return _fName; }
            set {
                _fName = value;
                OnPropertyChanged();
                SaveCommand.ChangeCanExecute();
            }
        }
        string _lName;
        public string LastName
        {
            get { return _lName; }
            set
            {
                _lName = value;
                OnPropertyChanged();
         
            }
        }

        double _latitude;
        public double Latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;
                OnPropertyChanged();
            }
        }

        double _longitude;
        public double Longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;
                OnPropertyChanged();
            }
        }
        string _imageUrl;
        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                OnPropertyChanged();
            }
        }
        


    }
}

