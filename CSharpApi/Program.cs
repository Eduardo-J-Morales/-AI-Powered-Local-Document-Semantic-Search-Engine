using Npgsql;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseStaticFiles();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

app.MapGet("/api/documents", async () =>
    {
        var result = new List<FileMetadataDto>();

        using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT filename, content_type, size FROM file_metadata", conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new FileMetadataDto(
                    reader.GetString(0),
                    reader.IsDBNull(1) ? null : reader.GetString(1),
                    reader.GetInt64(2)
                    ));
        }
        return Results.Ok(result);
    }
); 


app.MapPost("/api/files", async (FileMetadataDto dto) =>
    {
          using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

        using var cmd = new Npgsqlcommand("INSERT INTO file_metadata (filename, content_type, size) VALUES (@filename, @content_ type, @size)", conn);
        cmd.Parameters.addWithValue("filename", dto.Filename);
        cmd.Parameters.addWithValue("content_type", dto.ContentType ?? (object)DBNull.Value);
        cmd.Parameters.addWithValue("size", dto.Size);
    }
);
app.MapGet("/", () => " Hellow world");

app.Run();

public record FileMetadataDto(string Filename, string ContentType, long Size);
