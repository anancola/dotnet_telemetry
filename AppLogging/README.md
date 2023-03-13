# Application Telemetry implementation for .NET with Azure Monitor Exporter client library 

The [OpenTelemetry .NET](https://github.com/open-telemetry/opentelemetry-dotnet) exporters which send [telemetry data](https://docs.microsoft.com/azure/azure-monitor/app/data-model) to [Azure Monitor](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview) following the [OpenTelemetry Specification](https://github.com/open-telemetry/opentelemetry-specification).

## Getting started

### Prerequisites

- **Azure Subscription:**  To use Azure services, including Azure Monitor Exporter for [OpenTelemetry .NET](https://github.com/open-telemetry/opentelemetry-dotnet), you'll need a subscription.  If you do not have an existing Azure account, you may sign up for a [free trial](https://azure.microsoft.com/free/dotnet/) or use your [Visual Studio Subscription](https://visualstudio.microsoft.com/subscriptions/) benefits when you [create an account](https://account.windowsazure.com/Home/Index).
- **Azure Application Insights Connection String:** To send telemetry data to the monitoring service you'll need connection string from Azure Application Insights. If you are not familiar with creating Azure resources, you may wish to follow the step-by-step guide for [Create an Application Insights resource](https://docs.microsoft.com/azure/azure-monitor/app/create-new-resource) and [copy the connection string](https://docs.microsoft.com/azure/azure-monitor/app/sdk-connection-string?tabs=net#finding-my-connection-string).

### Install the package

Latest Version: [![Nuget](https://img.shields.io/nuget/vpre/Azure.Monitor.OpenTelemetry.Exporter.svg)](https://www.nuget.org/packages/Azure.Monitor.OpenTelemetry.Exporter/)  

Install the Azure Monitor Exporter for OpenTelemetry .NET with [NuGet](https://www.nuget.org/):
```dotnetcli
dotnet add package AppLogging
```

### Authenticate the client

AppLogging does not use authentication. 

## Key concepts

This exporter sends traces to the configured Azure Monitor Resource using HTTPS. IP addresses used by the Azure Monitor is documented in [IP addresses used by Application Insights and Log Analytics](https://docs.microsoft.com/azure/azure-monitor/app/ip-addresses#outgoing-ports).

## Examples

Refer to `Example\Program.cs` for a complete demo.

```csharp

using Microsoft.AzurePercept.Agents.Telemetry;

AppLogging.LogDebug<ClassName>(message);
AppLogging.LogInformation<ClassName>(message);
AppLogging.LogWarning<ClassName>(message);
AppLogging.LogError<ClassName>(message);
```

## Troubleshooting
Before using the function of AppLogging, you need to construct the factory by following.
```csharp
    // settings
    var serviceConfig = new ServiceConfig();
    serviceConfig.NameSpace = "productName";
    serviceConfig.Name = "functionName";

    var exporterConfig = new ExporterConfig();
    exporterConfig.Type = "configPath";
    exporterConfig.ConfigPath = "/etc/percept/telemetry/config.json";
    //set a default log level is a must (we can set to Information)
    exporterConfig.LogLevel = "Information";
    exporterConfig.Service = serviceConfig;

    AppLogging.CreateTelemetryFactory(exporterConfig);
```

## Next steps


## Contributing


## Release Schedule

This exporter is under active development.

The library is not yet _generally available_, and is not officially supported. Future releases will not attempt to maintain backwards compatibility with previous releases. Each beta release includes significant changes to the exporter package, making them incompatible with each other.
