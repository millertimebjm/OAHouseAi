using System.Text.Json.Serialization;
using OaHouseAi.Discord.Models;

namespace OaHouseAi.Discord.Services;

[JsonSerializable(typeof(DiscordChannel))]
[JsonSerializable(typeof(DiscordGatewayIntent))]
[JsonSerializable(typeof(DiscordGatewayIntentProperties))]
[JsonSerializable(typeof(DiscordHeartbeat))]
[JsonSerializable(typeof(DiscordIdentify<DiscordHeartbeat>))]
[JsonSerializable(typeof(DiscordIdentify<DiscordGatewayIntent>))]
[JsonSerializable(typeof(DiscordIdentify<DiscordMessage>))]
[JsonSerializable(typeof(DiscordIdentifyBase))]
[JsonSerializable(typeof(DiscordMessage))]
[JsonSerializable(typeof(DiscordMessageReference))]
[JsonSerializable(typeof(DiscordMessageSend))]
[JsonSerializable(typeof(DiscordMessageSendReference))]
[JsonSerializable(typeof(DiscordUser))]
public partial class DiscordJsonSerializerContext : JsonSerializerContext
{

}