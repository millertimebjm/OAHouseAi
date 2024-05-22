
using System.Net.WebSockets;

namespace OAHouseChatGpt.Services.Discord;

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