using Microsoft.AspNetCore.SignalR;

namespace TicTacToe.Hubs
{
    public class GameHub : Hub
    {

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }

        public async Task MakeMove(string gameId, int row, int col, string player)
        {
            // Notify all clients in the game group of the move
            await Clients.Group(gameId).SendAsync("ReceiveMove", row, col, player);
        }

        public async Task NotifyWin(string gameId, string player)
        {
            await Clients.Group(gameId).SendAsync("ReceiveWin", player);
        }

        public async Task NotifyDraw(string gameId)
        {
            await Clients.Group(gameId).SendAsync("ReceiveDraw");
        }

    }
}
