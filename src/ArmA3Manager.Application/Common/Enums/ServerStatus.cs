
using System.Text.Json.Serialization;

namespace ArmA3Manager.Application.Common.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ServerStatus
{
    NotInitialized = 0,
    Initialized = 1,
    Stopped = 2,
    Running = 3,
    Updating = 4,
}