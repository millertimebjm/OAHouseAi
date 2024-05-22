using System.Net.WebSockets;

public interface IClientWebSocketWrapper
{
    Task SendAsync(
        ClientWebSocket clientWebSocket,
        ArraySegment<byte> bytes, 
        WebSocketMessageType webSocketMessageType, 
        bool endOfMessage, 
        CancellationToken cancellationToken);
}