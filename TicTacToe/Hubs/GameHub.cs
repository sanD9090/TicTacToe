using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TicTacToe.Data;

namespace TicTacToe.Hubs
{
    public class GameHub : Hub
    {
        private readonly AppDbContext _dbContext;

        public GameHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;

            // Find the game where the player is participating
            var game = await _dbContext.Games
                .FirstOrDefaultAsync();

            if (game != null)
            {
                game.Status = "waiting";
                game.Board = JsonConvert.SerializeObject(new string[3, 3]);
                game.PlayerX = "";
                game.PlayerO = "";
                game.CurrentTurn = "x";


                await _dbContext.SaveChangesAsync();

                // Optionally, notify other players that someone has left
                await Clients.Group(game.Id).SendAsync("PlayerDisconnected", connectionId);
            }

            await base.OnDisconnectedAsync(exception);
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
