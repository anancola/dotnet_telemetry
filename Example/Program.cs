using System;
using Microsoft.AzurePercept.Agents.Telemetry;

class Program
{
    static void Main(string[] args)
    {
        // settings
        var serviceConfig = new ServiceConfig();
        serviceConfig.NameSpace = "percept";
        serviceConfig.Name = "solution";

        var exporterConfig = new ExporterConfig();
        exporterConfig.Type = "configPath";
        exporterConfig.ConfigPath = "/etc/percept/telemetry/config.json";
        //set a default log level is a must (we can set to Information)
        exporterConfig.LogLevel = "Information";
        exporterConfig.Service = serviceConfig;

        AppLogging.CreateTelemetryFactory(exporterConfig);

        // For every classes, includes the library
        // using Microsoft.AzurePercept.Agents.Telemetry;
        // log 
        // AppLogging.LogXxx<ClassName>(message);
        AppLogging.LogDebug<Program>("You won't see this log because the default logLevel was set to Information");
        AppLogging.LogInformation<Program>("Program start, LogInformation");
        AppLogging.LogWarning<Program>("Program start, LogWarning");
        AppLogging.LogError<Program>("Program start, LogError");
    }
}