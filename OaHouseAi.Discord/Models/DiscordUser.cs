using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using OaHouseAi.Discord.Services;

namespace OaHouseAi.Discord.Models;

public class DiscordUser
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("discriminator")]
    public string Discriminator { get; set; } 
    [JsonPropertyName("avatar")]
    public string Avatar { get; set; }

    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            TypeInfoResolver = new DiscordJsonSerializerContext(),
        };
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, GetJsonSerializerOptions());
    }
}