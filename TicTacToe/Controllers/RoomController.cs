using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Controllers
{
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }
        [HttpPost("CreateGame")]
        public async Task<IActionResult> CreateGame()
        {
            return Ok(await _roomService.CreateGame());
        }

        [HttpPost("JoinGame/{gameId}")]
        public async Task<IActionResult> JoinGame(string gameId)
        {
            return Ok(await _roomService.JoinGame(gameId));
        }
        [HttpPost("MakeMove")]
        public async Task<IActionResult> MakeMove(string gameId, int row, int col, string player)
        {
            return Ok(await _roomService.MakeMove(gameId, row, col, player));
        }

    }
}
