﻿namespace TicTacToe.Models
{
    public class MoveModel
    {
        public string GameId {get;set;}
        public int Row {get;set;}
        public int Col {get;set;}
        public string Player { get; set; }
    }
}
