using System;
using System.Linq;
using System.Reflection;
using Autofac;
using MVP.Services.Interfaces;
using MVP.ViewModels;
using TinyMvvm;
using TinyMvvm.Forms;
using Xamarin.Forms;

namespace MVP.Services
{
    public static class ContainerService
    {
        readonly static Lazy<IContainer> containerHolder = new Lazy<IContainer>(CreateContainer);

        public static IContainer Container
            => containerHolder.Value;

        static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            // Set up TinyMVVM
            var currentAssembly = Assembly.GetExecutingAssembly();
            var navigationHelper = new FormsNavigationHelper();

            navigationHelper.RegisterViewsInAssembly(currentAssembly);
            builder.RegisterInstance<INavigationHelper>(navigationHelper);

            var appAssembly = typeof(App).GetTypeInfo().Assembly;
            builder.RegisterAssemblyTypes(appAssembly).Where(x => x.IsSubclassOf(typeof(Page)));
            builder.RegisterAssemblyTypes(appAssembly).Where(x => x.IsSubclassOf(typeof(BaseViewModel)));

            //Register additional services
            builder.RegisterType<AnalyticsService>().As<IAnalyticsService>().SingleInstance();
            builder.RegisterType<AuthService>().As<IAuthService>().SingleInstance();
            builder.RegisterType<DialogService>().As<IDialogService>().SingleInstance();
            builder.RegisterType<MvpApiService>().As<IMvpApiService>().SingleInstance();

            return builder.Build();
        }
    }
}
