using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace Refiller;

public class RefillerConfig : BasePluginConfig
{
    public override int Version { get; set; } = 1;
    [JsonPropertyName("AssistRefill")] public bool AssistRefill { get; set; } = true; // true, false
    [JsonPropertyName("HealthRefill")] public string HealthRefill { get; set; } = "all"; // "all", "0", "100"
    [JsonPropertyName("AmmoRefill")] public string AmmoRefill { get; set; } = "all"; // "all", "current", "off"
    [JsonPropertyName("ArmorRefill")] public bool ArmorRefill { get; set; } = true; // true, false
}