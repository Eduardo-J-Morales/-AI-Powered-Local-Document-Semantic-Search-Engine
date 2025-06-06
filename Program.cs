using Microsoft.EntityFrameworkCore;
using UglyToad.PdfPig;
using Xceed.Words.NET;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=yourdb;Username=youruser;Password=yourpassword"));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/api/upload", async (HttpRequest request, AppDbContext db) =>
{
    var form = await request.ReadFormAsync();
    var files = form.Files;
    var results = new List<object>();

    foreach (var file in files)
    {
        string text = "";
    }
});

app.Run();

public class FileData
{
    public int Id { get; set; }
    public string FileName { get; set; } = "";
    public long Length { get; set; }
    public string ExtractedText { get; set; } = "";
}
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<FileData> Files { get; set; }
}