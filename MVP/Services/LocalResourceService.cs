using System;
using System.Reflection;

namespace MVP.Services
{
    public static class LocalResourceService
    {
        public static string GetFile(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream($"{fileName}.json");
            var text = "";

            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }
    }
}
