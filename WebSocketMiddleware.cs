namespace DnC
{
    public class WebSocketMiddleware
    {

        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                //WebSocket websocket = await context.WebSockets.AcceptWebSocketAsync();
            }
        }
    }
}
