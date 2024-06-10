using System.Net.WebSockets;
using OaHouseAi.Discord.Services.Interfaces;

namespace OaHouseAi.Discord.Services;

public class ClientWebSocketWrapper : IClientWebSocketWrapper
{
    public ClientWebSocketWrapper()
    {
    }

    public async Task SendAsync(
        ClientWebSocket clientWebSocket,
        ArraySegment<byte> bytes, 
        WebSocketMessageType webSocketMessageType, 
        bool endOfMessage, 
        CancellationToken cancellationToken) => 
        await clientWebSocket.SendAsync(
            bytes,
            webSocketMessageType,
            endOfMessage,
            cancellationToken);
}