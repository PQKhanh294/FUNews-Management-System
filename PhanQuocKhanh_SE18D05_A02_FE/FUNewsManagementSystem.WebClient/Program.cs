var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<
    FUNewsManagementSystem.WebClient.Services.IPublicNewsApiClient,
    FUNewsManagementSystem.WebClient.Services.PublicNewsApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7101");
    client.Timeout = TimeSpan.FromSeconds(15);
});
builder.Services.AddSingleton<
    FUNewsManagementSystem.WebClient.Services.IRichTextSanitizer,
    FUNewsManagementSystem.WebClient.Services.RichTextSanitizer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

