using System.Net.WebSockets;

namespace OaHouseAi.Discord.Services.Interfaces;

public interface IClientWebSocketWrapper
{
    Task SendAsync(
        ClientWebSocket clientWebSocket,
        ArraySegment<byte> bytes, 
        WebSocketMessageType webSocketMessageType, 
        bool endOfMessage, 
        CancellationToken cancellationToken);
}