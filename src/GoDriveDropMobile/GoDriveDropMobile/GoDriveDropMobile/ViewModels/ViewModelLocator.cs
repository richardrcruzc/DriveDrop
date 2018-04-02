 using Autofac;
using GoDriveDrop.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Reflection;
using System.Globalization;
using GoDriveDrop.Core.Identity;
using GoDriveDrop.Core.Services.Driver;
using GoDriveDrop.Core.Services.Common;
using GoDriveDrop.Core.Services.RequestProvider;
using GoDriveDrop.Core.Services.Navigation;
using GoDriveDrop.Core.Services.OpenUrl;
using GoDriveDrop.Core.Views;
using GoDriveDrop.Core.Services.Location;
using GoDriveDrop.Core.Services.User;
using GoDriveDrop.Core.Services.Address;

namespace GoDriveDrop.Core.ViewModels
{
    public static class ViewModelLocator
    {
        private static IContainer _container;

        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);

        public static bool GetAutoWireViewModel(BindableObject bindable)
        {
            return (bool)bindable.GetValue(ViewModelLocator.AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(BindableObject bindable, bool value)
        {
            bindable.SetValue(ViewModelLocator.AutoWireViewModelProperty, value);
        }

        public static bool UseMockService { get; set; }

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            // View models
            
                  builder.RegisterType<RootPage>();
            builder.RegisterType<SplashViewModel>();

            builder.RegisterType<LoginViewModel>();

            builder.RegisterType<NewDriverViewModel>();
            builder.RegisterType<NewSenderViewModel>();
            builder.RegisterType<PersonalInfoViewModel>();
            builder.RegisterType<AddressesViewModel>();
            builder.RegisterType<PackageViewModel>();
            builder.RegisterType<NewPackageViewModel>();

            // Services
            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();

            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<DialogService>().As<IDialogService>();
            builder.RegisterType<OpenUrlService>().As<IOpenUrlService>();
            builder.RegisterType<IdentityService>().As<IIdentityService>();
            builder.RegisterType<RequestProvider>().As<IRequestProvider>();
            builder.RegisterType<LocationService>().As<ILocationService>().SingleInstance();

            builder.RegisterType<Commons>().As<ICommons>();
            builder.RegisterType<GoogleAddress>().As<IGoogleAddress>();
            builder.RegisterType<DriverService>().As<IDriverService>();
            builder.RegisterType<SenderService>().As<ISenderService>();
            builder.RegisterType<AddressService>().As<IAddressService>();
            


            if (_container != null)
            {
                _container.Dispose();
            }
            _container = builder.Build();
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as Element;
            if (view == null)
            {
                return;
            }

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);

            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType == null)
            {
                return;
            }
            var viewModel = _container.Resolve(viewModelType);
            view.BindingContext = viewModel;
        }
    }
}
