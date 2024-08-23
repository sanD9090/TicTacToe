using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TicTacToe.Hubs;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Controllers
{
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IHubContext<GameHub> _hubContext;


        public RoomController(IRoomService roomService, IHubContext<GameHub> hubContext)
        {
            _roomService = roomService;
            _hubContext = hubContext;
        }

        [HttpPost("CreateGame")]
        public async Task<IActionResult> CreateGame()
        {
            var response = await _roomService.CreateGame();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveGameCreated", response.Data);
            }
            return Ok(response);
        }

        [HttpPost("JoinGame/{gameId}/{connectionId}")]
        public async Task<IActionResult> JoinGame(string gameId, string connectionId)
        {
            var response = await _roomService.JoinGame(gameId, connectionId);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Add the current connection to the game's SignalR group
                await _hubContext.Groups.AddToGroupAsync(connectionId, gameId);

                // Notify all players in the group that someone has joined
                await _hubContext.Clients.Group(gameId).SendAsync("PlayerJoined", response.Data);
            }

            return Ok(response);
        }
        [HttpPost("MakeMove")]
        public async Task<IActionResult> MakeMove([FromBody] MoveModel move)
        {

            var response = await _roomService.MakeMove(move.GameId, move.Row, move.Col, move.Player);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await _hubContext.Clients.Group(move.GameId).SendAsync("ReceiveMove", move.Row, move.Col, move.Player);

                if (response.Message.Contains("wins"))
                {
                    await _hubContext.Clients.Group(move.GameId).SendAsync("ReceiveWin", $"{move.Player} wins");
                }
                else if (response.Message.Contains("draw"))
                {
                    await _hubContext.Clients.Group(move.GameId).SendAsync("ReceiveDraw");
                }
            }
            return Ok(response);
        }

        [HttpGet("ResetGame/{gameId}")]
        public async Task<IActionResult> ResetGame(string gameId)
        {
            var response = await _roomService.ResetGame(gameId);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await _hubContext.Clients.All.SendAsync("ResetGameCreated", response.Data);
            }
            return Ok(response);
        }
    }
}
