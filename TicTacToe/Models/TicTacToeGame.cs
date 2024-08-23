using System.Diagnostics.CodeAnalysis;

namespace TicTacToe.Models
{
    public class TicTacToeGame
    {
        public string Id { get; set; }

        [AllowNull]
        public string PlayerX { get; set; }
        [AllowNull]
        public string PlayerO { get; set; }
        public string Board { get; set; }
        public string CurrentTurn { get; set; } // Indicates whose turn it is
        public string Status { get; set; }
    }
}
