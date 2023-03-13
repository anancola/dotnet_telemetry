// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace Microsoft.AzurePercept.Agents.Telemetry
{
    public class ExporterConfig
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("backendUrl")]
        public string BackendUrl { get; set; }

        [JsonPropertyName("configPath")]
        public string ConfigPath { get; set; }

        [JsonPropertyName("logLevel")]
        public string LogLevel { get; set; }

        [JsonPropertyName("service")]
        public ServiceConfig Service { get; set; }

        [JsonPropertyName("isEnableAzureMonitor")]
        public bool IsEnableAzureMonitor { get; set; }
    }
}
