using System.Data;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseStaticFiles();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

public record

app.MapGet("/", () => " Hellow world");

app.Run();