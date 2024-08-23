using System.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TicTacToe.Data;
using TicTacToe.Models;

namespace TicTacToe.Services
{
    public interface IRoomService
    {
        Task<ApiResponse> CreateGame();
        Task<ApiResponse> JoinGame(string gameId, string connectionId);
        Task<ApiResponse> MakeMove(string gameId, int row, int col, string player);
        Task<ApiResponse> ResetGame(string gameId);
    }
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;
        private readonly ApiResponse _response = new();
        private string? playerSymbol;

        public RoomService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> CreateGame()
        {
            var gameId = Guid.NewGuid().ToString();

            var game = new TicTacToeGame
            {
                Id = gameId,
                Status = "waiting",
                Board = JsonConvert.SerializeObject(new string[3, 3]),
                PlayerX = "",
                PlayerO = "",
                CurrentTurn = "x"
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();


            _response.StatusCode = System.Net.HttpStatusCode.OK;
            _response.Message = "Game created successfully";
            _response.Data = new { game, gameId };


            return _response;
        }

        public async Task<ApiResponse> JoinGame(string gameId, string connectionId)
        {
            // Retrieve the game from the database using the gameId
            var game = await _context.Games.SingleOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
            {
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                _response.Message = "Game Not Found.";
                _response.Data = null;

                return _response;
            }

            string assignedSymbol;

            // Check if PlayerX is not assigned
            if (string.IsNullOrEmpty(game.PlayerX))
            {
                game.PlayerX = "X";
                assignedSymbol = "X";
            }
            else if (string.IsNullOrEmpty(game.PlayerO)) // Check if PlayerO is not assigned
            {
                game.PlayerO = "O";
                assignedSymbol = "O";
            }
            else
            {

                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Message = "Game is already full.";
                return _response;

            }



            await _context.SaveChangesAsync();

            _response.StatusCode = System.Net.HttpStatusCode.OK;
            _response.Message = "Game joined.";
            _response.Data = new { Game = game, PlayingAs = assignedSymbol };

            return _response;
        }

        public async Task<ApiResponse> MakeMove(string gameId, int row, int col, string player)
        {
            var game = await _context.Games.SingleOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Message = "Game not found";
                return _response;
            }

            // Deserialize the board from JSON
            var board = JsonConvert.DeserializeObject<string[,]>(game.Board);

            // Check if it's the correct player's turn
            if (!string.Equals(game.CurrentTurn, player, StringComparison.OrdinalIgnoreCase))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Message = "Not your turn";
                return _response;
            }

            // Check if the selected cell is empty
            if (board[row, col] != null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Message = "Cell is already occupied";
                return _response;
            }

            // Update the board with the player's move
            board[row, col] = player;

            // **Check if the move resulted in a win**
            if (CheckForWin(board, player))
            {
                game.Status = $"{player} Won";
                game.Board = JsonConvert.SerializeObject(board);
                await _context.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = $"{player} wins!";
                _response.Data = game;

                return _response;
            }

            if (CheckForDraw(board))
            {
                game.Status = "Draw";
                game.Board = JsonConvert.SerializeObject(board);
                await _context.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "The game is a draw!";
                _response.Data = game;

                return _response;
            }

            game.Board = JsonConvert.SerializeObject(board);
            game.CurrentTurn = player == "X" ? "O" : "X";
            await _context.SaveChangesAsync();

            _response.StatusCode = HttpStatusCode.OK;
            _response.Message = "Move successful";
            _response.Data = game;

            return _response;
        }


        private static bool CheckForWin(string[,] board, string player)
        {
            for (int i = 0; i < 3; i++)
            {
                if ((board[i, 0] == player && board[i, 1] == player && board[i, 2] == player) ||
                    (board[0, i] == player && board[1, i] == player && board[2, i] == player))
                {
                    return true;
                }
            }
            if ((board[0, 0] == player && board[1, 1] == player && board[2, 2] == player) ||
                (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player))
            {
                return true;
            }

            return false;
        }

        private static bool CheckForDraw(string[,] board)
        {
            foreach (var cell in board)
            {
                if (cell == null) return false;
            }
            return true;
        }

        public async Task<ApiResponse> ResetGame(string gameId)
        {
            var game = await _context.Games.FirstOrDefaultAsync(item => item.Id == gameId);

            if (game is null)
            {
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                _response.Message = "Game not found.";
                _response.Data = null;
            }

            game.Status = "waiting";
            game.Board = JsonConvert.SerializeObject(new string[3, 3]);
            game.PlayerX = "";
            game.PlayerO = "";
            game.CurrentTurn = "x";

            await _context.SaveChangesAsync();


            _response.StatusCode = System.Net.HttpStatusCode.OK;
            _response.Message = "Game reset successfull.";
            _response.Data = new { game };

            return _response;
        }
    }




}
