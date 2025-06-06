using Microsoft.EntityFrameworkCore;
using UglyToad.PdfPig;
using Xceed.Words.NET;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

public class FileData {
    public int Id { get; set; }
    public string FileName { get; set; } = "";
    public long Length { get; set; }
    public string ExtractedText { get; set; } = "";
}

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {  }
}

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();


app.Run();
