using LienWorksSharp.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var dataRoot = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "LienWorks_Data"));
Directory.CreateDirectory(dataRoot);

builder.Services.Configure<DataStorageOptions>(options => options.RootPath = dataRoot);
builder.Services.AddSingleton<DataPaths>();
builder.Services.AddSingleton<TemplateService>();
builder.Services.AddSingleton<ClientService>();
builder.Services.AddSingleton<WorkOrderService>();
builder.Services.AddSingleton<DataSeeder>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.EnsureSeededAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(dataRoot),
    RequestPath = "/data"
});

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
