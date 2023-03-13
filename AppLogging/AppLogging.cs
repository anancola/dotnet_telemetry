
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AzurePercept.Agents.Telemetry
{
    // Setup logging for all classes to use
    public static class AppLogging
    {
        private static Dictionary<string, ILogger> LoggerDict;

        public static ILoggerFactory CentralLoggerFactory { get; set; } = CreateLoggerFactory();

        private static MeterProviderBuilder _meterProviderBuilder;

        public static MeterProvider MeterProvider { get; set; }

        private static TracerProviderBuilder _tracerProviderBuilder;

        public static TracerProvider TracerProvider { get; set; }

        private static ServiceConfig _serviceConfig;

        public static ILogger GetLogger<T>()
        {
            string className = typeof(T).FullName;
            if (LoggerDict.ContainsKey(className))
            {
                return LoggerDict[className];
            }
            else
            {
                var logger = CentralLoggerFactory.CreateLogger<T>();
                LoggerDict.Add(className, logger);
                return logger;
            }
        }
        public static void LogDebug<T>(string s) => GetLogger<T>().LogDebug(s);
        public static void LogInformation<T>(string s) => GetLogger<T>().LogInformation(s);
        public static void LogWarning<T>(string s) => GetLogger<T>().LogWarning(s);
        public static void LogError<T>(string s) => GetLogger<T>().LogError(s);

        public static void CreateTelemetryFactory(ExporterConfig exporterConfig)
        {
            CentralLoggerFactory = CreateLoggerFactory(exporterConfig);
            // MeterProvider = CreateMeterProvider(exporterConfig);
            TracerProvider = CreateTracerProvider(exporterConfig);
            _serviceConfig = exporterConfig.Service;
        }
        public static ILoggerFactory CreateLoggerFactory()
        {
            LoggerDict = new();
            return CreateLoggerFactory(null);
        }
        public static ILoggerFactory CreateLoggerFactory(ExporterConfig exporterConfig)
        {
            ILoggerFactory loggerFactory = null;
            LoggerDict = new();
            if (exporterConfig != null && exporterConfig.Service != null)
            {
                Dictionary<string, object> resourceAttributes = exporterConfig.Service.GetResourceAttributes();
                var resourceBuilder = ResourceBuilder.CreateDefault().AddAttributes(resourceAttributes);
                LogLevel logLevel = GetLogLevel(exporterConfig.LogLevel);

                loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddOpenTelemetry(options =>
                    {
                        options.IncludeFormattedMessage = false;
                        if (!string.IsNullOrWhiteSpace(exporterConfig.BackendUrl)
                            && exporterConfig.IsEnableAzureMonitor)
                        {
                            options.AddAzureMonitorLogExporter(o =>
                            {
                                o.ConnectionString = exporterConfig.BackendUrl;
                            });
                        }
                        options.SetResourceBuilder(resourceBuilder);
                        options.AddConsoleExporter();
                    });
                    builder.SetMinimumLevel(logLevel);
                });
            }
            else
            {
                loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddOpenTelemetry(options =>
                    {
                        options.IncludeFormattedMessage = false;
                        options.AddConsoleExporter();
                    });
                    builder.SetMinimumLevel(GetLogLevel("Debug"));
                });
            }
            return loggerFactory;
        }

        private static LogLevel GetLogLevel(string logLevelStr)
        {
            return logLevelStr switch
            {
                "Critical" => LogLevel.Critical,
                "Error" => LogLevel.Error,
                "Warning" => LogLevel.Warning,
                "Information" => LogLevel.Information,
                "Debug" => LogLevel.Debug,
                "Trace" => LogLevel.Trace,
                _ => throw new ArgumentException($"{4000}, Invalid LogLevel: '{logLevelStr}'")
            };
        }

        public static MeterProvider CreateMeterProvider(ExporterConfig exporterConfig)
        {
            if (exporterConfig != null && exporterConfig.Service != null)
            {
                ServiceConfig serviceConfig = exporterConfig.Service;
                Meter meter = serviceConfig.GetMeter();
                Dictionary<string, object> resourceAttributes = serviceConfig.GetResourceAttributes();
                var resourceBuilder = ResourceBuilder.CreateDefault().AddAttributes(resourceAttributes);

                _meterProviderBuilder = OpenTelemetry.Sdk.CreateMeterProviderBuilder()
                    .AddMeter(meter.Name)
                    .SetResourceBuilder(resourceBuilder);

                if (exporterConfig.IsEnableAzureMonitor &&
                    !string.IsNullOrWhiteSpace(exporterConfig.BackendUrl))
                {
                    _meterProviderBuilder.AddAzureMonitorMetricExporter(o =>
                        {
                            o.ConnectionString = exporterConfig.BackendUrl;
                        });
                }
                //send to Azure Monitor only
                // _meterProviderBuilder.AddConsoleExporter();
                return _meterProviderBuilder.Build();
            }
            throw new ArgumentException($"{4000}, ExporterConfig is null");
        }

        public static TracerProvider CreateTracerProvider(ExporterConfig exporterConfig)
        {
            if (exporterConfig != null && exporterConfig.Service != null)
            {
                ServiceConfig serviceConfig = exporterConfig.Service;
                Dictionary<string, object> resourceAttributes = serviceConfig.GetResourceAttributes();
                var resourceBuilder = ResourceBuilder.CreateDefault().AddAttributes(resourceAttributes);

                _tracerProviderBuilder = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
                    .AddSource(serviceConfig.GetSourceName())
                    .SetResourceBuilder(resourceBuilder)
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();
                if (exporterConfig.IsEnableAzureMonitor
                    && !string.IsNullOrWhiteSpace(exporterConfig.BackendUrl))
                {
                    _tracerProviderBuilder.AddAzureMonitorTraceExporter(o =>
                        {
                            o.ConnectionString = exporterConfig.BackendUrl;
                        });
                }
                //send to Azure Monitor only
                // _tracerProviderBuilder.AddConsoleExporter();
                return _tracerProviderBuilder.Build();
            }
            throw new ArgumentException($"{4000}, ExporterConfig is null");
        }
    }
}