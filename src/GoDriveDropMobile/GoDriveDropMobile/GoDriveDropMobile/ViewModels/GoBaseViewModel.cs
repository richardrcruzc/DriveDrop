//using System;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;
//using System.Threading.Tasks;
//using GoDriveDrop.Core.Services;

//namespace GoDriveDrop.Core.ViewModels
//{
//    public abstract class GoBaseViewModel : INotifyPropertyChanged
//    {
//        protected IGoNavService NavService { get; private set; }

//        bool _isProcessBusy;
//        public bool IsProcessBusy
//        {
//            get { return _isProcessBusy; }
//            set
//            {
//                _isProcessBusy = value;
//                OnPropertyChanged();
//                OnIsBusyChanged();
//            }
//        }

//        IGoDataService _azureDatabaseService;
//        public IGoDataService AzureDatabaseService
//        {
//            get { return _azureDatabaseService; }
//            set
//            {
//                _azureDatabaseService = value;
//                OnPropertyChanged();
//            }
//        }

//        protected GoBaseViewModel(IGoNavService navService)
//        {
//            // Declare our Navigation Service and Azure Database URL
//            var WALKS_URL = "https://trackmywalks.azurewebsites.net";

//            NavService = navService;
//            AzureDatabaseService = new GoDataService(new Uri(WALKS_URL, UriKind.RelativeOrAbsolute), null);
//        }

//        public abstract Task Init();

//        public event PropertyChangedEventHandler PropertyChanged;

//        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            var handler = PropertyChanged;
//            if (handler != null)
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        protected virtual void OnIsBusyChanged()
//        {
//            // We are processing our Walks Trail Information
//        }
//    }

//    public abstract class GoBaseViewModel<WalkParam> :GoBaseViewModel
//    {
//        protected GoBaseViewModel(IGoNavService navService) : base(navService)
//        {
//        }

//        public override async Task Init()
//        {
//            await Init(default(WalkParam));
//        }

//        public abstract Task Init(WalkParam walkDetails);
//    }
//}
