using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MVP.Services.Interfaces;

namespace MVP.Services
{
    public class AnalyticsService : IAnalyticsService
    {

#if AppStore
        const string iOSKey = "d93d1cb9-8a66-494a-8478-b73ac5f0516d";
        const string androidKey = "3aa628e6-2d4e-428a-b064-88a2d358763e";
#else
        const string iOSKey = "71df115f-45d9-49da-bf13-de09ff4a3aff";
        const string androidKey = "36d4c425-d4b8-4712-baa3-681cc6586c14";
#endif

        public AnalyticsService() => AppCenter.Start(ApiKey, typeof(Analytics), typeof(Crashes));

        string ApiKey => Xamarin.Forms.Device.RuntimePlatform switch
        {
            Xamarin.Forms.Device.iOS => iOSKey,
            Xamarin.Forms.Device.Android => androidKey,
            _ => throw new NotSupportedException()
        };

        public void Track(string trackIdentifier, IDictionary<string, string>? table = null) =>
            Analytics.TrackEvent(trackIdentifier, table);

        public void Track(string trackIdentifier, string key, string value) =>
            Analytics.TrackEvent(trackIdentifier, new Dictionary<string, string> { { key, value } });

        public ITimedEvent TrackTime(string trackIdentifier, IDictionary<string, string>? table = null) =>
            new TimedEvent(trackIdentifier, table);

        public ITimedEvent TrackTime(string trackIdentifier, string key, string value) =>
            TrackTime(trackIdentifier, new Dictionary<string, string> { { key, value } });

        public void Report(Exception exception,
                                  IDictionary<string, string>? properties = null,
                                  [CallerMemberName] string callerMemberName = "",
                                  [CallerLineNumber] int lineNumber = 0,
                                  [CallerFilePath] string filePath = "")
        {
            PrintException(exception, callerMemberName, lineNumber, filePath, properties);

            Crashes.TrackError(exception, properties);
        }

        [Conditional("DEBUG")]
        void PrintException(Exception exception, string callerMemberName, int lineNumber, string filePath, IDictionary<string, string>? properties = null)
        {
            var fileName = System.IO.Path.GetFileName(filePath);

            Debug.WriteLine(exception.GetType());
            Debug.WriteLine($"Error: {exception.Message}");
            Debug.WriteLine($"Line Number: {lineNumber}");
            Debug.WriteLine($"Caller Name: {callerMemberName}");
            Debug.WriteLine($"File Name: {fileName}");

            if (properties != null)
            {
                foreach (var property in properties)
                    Debug.WriteLine($"{property.Key}: {property.Value}");
            }

            Debug.WriteLine(exception);
        }

        public class TimedEvent : ITimedEvent
        {
            readonly Stopwatch stopwatch;
            readonly string trackIdentifier;

            public TimedEvent(string trackId, IDictionary<string, string>? dictionary)
            {
                Data = dictionary ?? new Dictionary<string, string>();
                trackIdentifier = trackId;
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }

            public IDictionary<string, string> Data { get; }

            public void Dispose()
            {
                stopwatch.Stop();
                Data.Add("Timed Event", $"{stopwatch.Elapsed:ss\\.fff}s");
                Analytics.TrackEvent($"{trackIdentifier} [Timed Event]", Data);
            }
        }

        public interface ITimedEvent : IDisposable
        {
            IDictionary<string, string> Data { get; }
        }
    }
}
