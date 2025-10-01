using Carter;
using SketchSync;
using SketchSync.Hubs;

var builder = WebApplication.CreateBuilder(args);
var startup = new Startup(builder.Configuration, builder.Environment);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

app.MapCarter();
app.MapHub<BoardHub>("/hubs/board");

Startup.Configure(app);

await app.RunAsync();