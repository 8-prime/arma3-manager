using System.Text.Json.Serialization;

namespace ArmA3Manager.Application.Common.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ServerLogSeverity
{
    Info,
    Error
}