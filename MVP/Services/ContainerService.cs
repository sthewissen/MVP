using System;
using System.Linq;
using System.Reflection;
using Autofac;
using MVP.Services.Interfaces;
using MVP.ViewModels;
using TinyNavigationHelper;
using TinyNavigationHelper.Forms;
using Xamarin.Forms;

namespace MVP.Services
{
    /// <summary>
    /// Initializes all of the dependency injection bits and bobs.
    /// </summary>
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
            // TODO: One day... builder.RegisterType<MsalAuthService>().As<IAuthService>().SingleInstance();
            builder.RegisterType<LiveAuthService>().As<IAuthService>().SingleInstance();
            builder.RegisterType<MvpApiService>().As<IMvpApiService>().SingleInstance();
            builder.RegisterType<LanguageService>().As<LanguageService>().SingleInstance();

            return builder.Build();
        }
    }
}
