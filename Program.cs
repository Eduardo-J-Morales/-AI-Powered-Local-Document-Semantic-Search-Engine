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
        var ext = Path.GetExtension(file.FileName).ToLower();
        using var stream = file.OpenReadStream();

        if (ext == ".text")
        {
            using var reader = new StreamReader(stream);
            text = await reader.ReadToEndAsync();
        }
        else if (ext == ".pdf")
        {
            using (var pdf = PdfDocument.Open(stream))
            {
                foreach (var page in pdf.GetPages())
                {
                    text += page.Text + "\n";
                }
            }
        }
        else if (ext == ".docx")
        {
            using var ms = new MemoryStream();
        }
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