var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TicTacToe>("tictactoe");

builder.Build().Run();
