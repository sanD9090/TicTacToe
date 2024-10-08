using Microsoft.EntityFrameworkCore;
using TicTacToe.Data;
using TicTacToe.Hubs;
using TicTacToe.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.




builder.Services.AddTransient<IRoomService, RoomService>();

//builder.Services.AddOpenTelemetry().WithMetrics(metrics =>
//{
//    metrics.AddMeter("Microsoft.AspNetCore.Hosting");
//    metrics.AddMeter("Microsoft. AspNetCore.Server.Kestrel");
//    metrics.AddMeter("System.Net.Http");
//    metrics.AddPrometheusExporter();
//    metrics.AddOtlpExporter();
//});

//builder.Logging.AddOpenTelemetry(options =>
//{
//    options.AddOtlpExporter();
//});



builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Specific origin
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials(); // Allow credentials
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

app.MapDefaultEndpoints();


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");


app.UseAuthorization();
app.MapControllers();
app.MapHub<GameHub>("/gameHub");
app.Run();
