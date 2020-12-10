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
        public AnalyticsService() => AppCenter.Start(ApiKey, typeof(Analytics), typeof(Crashes));

        string ApiKey => Xamarin.Forms.Device.RuntimePlatform switch
        {
#if AppStore
            Xamarin.Forms.Device.iOS => Constants.AppCenterProdiOSKey,
            Xamarin.Forms.Device.Android => Constants.AppCenterProdAndroidKey,
#else
            Xamarin.Forms.Device.iOS => Constants.AppCenterDeviOSKey,
            Xamarin.Forms.Device.Android => Constants.AppCenterDevAndroidKey,
#endif
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
