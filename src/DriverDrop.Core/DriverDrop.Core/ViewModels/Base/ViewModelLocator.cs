using Autofac;
using DriverDrop.Core.Services;
using DriverDrop.Core.Services.Identity;
using DriverDrop.Core.Services.Locations;
using DriverDrop.Core.Services.OpenUrl;
using DriverDrop.Core.Services.RequestProvider;
using DriverDrop.Core.Services.Locations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DriverDrop.Core.ViewModels.Base
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

        public static void RegisterDependencies(bool useMockServices)
        {
            var builder = new ContainerBuilder();

            // View models
           
            builder.RegisterType<LoginViewModel>();
            builder.RegisterType<SettingsViewModel>();
            
            // Services
            builder.RegisterType<INavigationService>().As<INavigationService>().SingleInstance();
            builder.RegisterType<DialogService>().As<IDialogService>();
            builder.RegisterType<OpenUrlService>().As<IOpenUrlService>();
            builder.RegisterType<IdentityService>().As<IIdentityService>();
            builder.RegisterType<RequestProvider>().As<IRequestProvider>();
            builder.RegisterType<LocationService>().As<ILocationService>().SingleInstance();

            //if (useMockServices)
            //{
            //    builder.RegisterInstance(new CatalogMockService()).As<ICatalogService>();
            //    builder.RegisterInstance(new BasketMockService()).As<IBasketService>();
            //    builder.RegisterInstance(new OrderMockService()).As<IOrderService>();
            //    builder.RegisterInstance(new UserMockService()).As<IUserService>();
            //    builder.RegisterInstance(new CampaignMockService()).As<ICampaignService>();

            //    UseMockService = true;
            //}
            //else
            //{
            //    builder.RegisterType<CatalogService>().As<ICatalogService>().SingleInstance();
            //    builder.RegisterType<BasketService>().As<IBasketService>().SingleInstance();
            //    builder.RegisterType<OrderService>().As<IOrderService>().SingleInstance();
            //    builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
            //    builder.RegisterType<CampaignService>().As<ICampaignService>().SingleInstance();

            //    UseMockService = false;
            //}

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
