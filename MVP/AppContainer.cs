using System;
using FreshTinyIoC;

namespace MVP
{
    public static class AppContainer
    {
        private static FreshTinyIoCContainer Container { get; set; } = FreshTinyIoCContainer.Current;

        public static void Build()
        {
            FreshTinyIoCContainer.Current.AutoRegister(new[] { typeof(App).Assembly },
                t => t.FullName.StartsWith("MVP.Services", StringComparison.Ordinal) &&
                t.Name.EndsWith("Service", StringComparison.Ordinal));
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
