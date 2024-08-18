namespace TicTacToe.Models
{
    public class TicTacToeGame
    {
        public string Id { get; set; }
        public string Player1 { get; set; } // Assuming players are identified by string, e.g., username or user ID
        public string Player2 { get; set; }
        public string  Board { get; set; } 
        public string CurrentTurn { get; set; } // Indicates whose turn it is
        public string Status { get; set; }
    }
}
