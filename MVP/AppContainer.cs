using System;
using FreshTinyIoC;
using MVP.Services;
using MVP.Services.Interfaces;

namespace MVP
{
    public static class AppContainer
    {
        private static FreshTinyIoCContainer Container { get; set; } = FreshTinyIoCContainer.Current;

        public static void Build()
        {
            FreshTinyIoCContainer.Current.Register<IAnalyticsService, AnalyticsService>();
            FreshTinyIoCContainer.Current.Register<IAuthService, AuthService>();
            FreshTinyIoCContainer.Current.Register<IDialogService, DialogService>();
            FreshTinyIoCContainer.Current.Register<IMvpApiService, MvpApiService>();
        }

        public static void Register<T>()
            where T : class
        {
            Container.Register<T>();
        }

        public static void Register<T>(T instance)
            where T : class
        {
            Container.Register(instance);
        }

        public static void Register<T, I>()
            where T : class
            where I : class, T
        {
            Container.Register<T, I>();
        }

        public static T Resolve<T>(string name = "") where T : class
        {
            return Container.Resolve<T>(name);
        }
    }
}
