using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Enable WebSockets
app.UseWebSockets();

// Add middleware to handle WebSocket requests
app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await Echo(context, webSocket);  // Implement the WebSocket logic in Echo method
    }
    else
    {
        await next();
    }
});

// Define other middleware (like HTTPS redirection, routing, etc.)
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();  // Map your API controllers

app.Run();

// Define the Echo method that handles WebSocket messages
async Task Echo(HttpContext context, WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];  // Set buffer size
    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    while (!result.CloseStatus.HasValue)
    {
        // Echo the received message back to the client
        await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    }

    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
}
