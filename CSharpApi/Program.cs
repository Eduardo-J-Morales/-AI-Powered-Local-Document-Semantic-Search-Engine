using System.Diagnostics;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseStaticFiles();
app.UseCors();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString is null)
    throw new InvalidOperationException("'DefaultConnection' was not found.");

app.MapGet("/api/documents", async () =>
{
    try
    {
        var result = new List<FileMetadataDto>();

        using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT filename, contenttype, size, route FROM file_metadata", conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new FileMetadataDto(
                reader.GetString(0),
                reader.IsDBNull(1) ? null : reader.GetString(1),
                reader.GetInt64(2),
                reader.GetString(3)
            ));
        }
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            title: "Failed to fetch documents",
            statusCode: 500,
            extensions: new Dictionary<string, object?>
            {
                { "exceptionType", ex.GetType().Name },
                { "stackTrace", ex.StackTrace }
            }
        );
    }
});

app.MapPost("/api/documents/upload", async (FileMetadataDto dto) =>
{
    try
    {
        using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("INSERT INTO file_metadata(filename, contenttype, size, route) VALUES (@filename, @content_type, @size, @route)", conn);
        cmd.Parameters.AddWithValue("filename", dto.Filename);
        cmd.Parameters.AddWithValue("content_type", dto.ContentType);
        cmd.Parameters.AddWithValue("size", dto.Size);
        cmd.Parameters.AddWithValue("route", dto.Route);

        await cmd.ExecuteNonQueryAsync();

        return Results.Ok(new { message = "File metadata uploaded successfully." });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            title: "Failed to upload document metadata",
            statusCode: 500,
            extensions: new Dictionary<string, object?>
            {
                { "exceptionType", ex.GetType().Name },
                { "stackTrace", ex.StackTrace }
            }
        );
    }
});

app.MapDelete("/api/documents/{filename}", async (string filename) => 
{
    try 
    {
        using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("DELETE FROM file_metadata WHERE filename = @filename", conn);
        cmd.Parameters.AddWithValue("filename", filename);

        int affectedRows = await cmd.ExecuteNonQueryAsync();

        if (affectedRows == 0)
        {
            return Results.NotFound(new { message = $"No document found with filename '{filename}'." });
        }

        return Results.Ok(new { message = $"Document '{filename}' deleted successfully." });

    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            title: "Failed to delete document",
            statusCode: 500,
            extensions: new Dictionary<string, object?> {
                { "exceptionType", ex.GetType().Name },
                { "stackTrace", ex.StackTrace }
            });
    }
});

app.MapGet("/api/documents/execute", async (string command, string? args) =>
{
    if (string.IsNullOrEmpty(command))
    {
        return Results.BadRequest("Command parameter is required.");
    }

    var processStartInfo = new ProcessStartInfo
    {
        FileName = command,
        Arguments = args,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using (var process = new Process { StartInfo = processStartInfo })
    {
        try
        {
            process.Start();
            string standardOutput = await process.StandardOutput.ReadToEndAsync();
            string standardError = await process.StandardError.ReadToEndAsync();

            return Results.Ok(standardOutput);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Results.Problem(
                detail: ex.Message,
                title: "Failed to execute command",
                statusCode: 500,
                extensions: new Dictionary<string, object?>
                {
                    { "exceptionType", ex.GetType().Name },
                    { "stackTrace", ex.StackTrace }
                }
            );
        }
    }
});

app.Run();

public record FileMetadataDto(string Filename, string ContentType, long Size, string Route);