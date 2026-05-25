using Hr_System.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.Cookie.Name = "HrSystemAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        options.SlidingExpiration = true;
        options.Cookie.MaxAge = options.ExpireTimeSpan;
        options.ClaimsIssuer = "HrSystem";
    });

builder.Services.AddAuthorization();

// Database Connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var launchHttpPort = GetNextAvailablePort(5031);
var launchHttpsPort = GetNextAvailablePort(7031);
builder.WebHost.UseUrls($"http://127.0.0.1:{launchHttpPort}", $"https://127.0.0.1:{launchHttpsPort}");

var app = builder.Build();


// ===== Check Database Connection =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        db.Database.Migrate();
        
        if (db.Database.CanConnect())
        {
            Console.WriteLine("=================================");
            Console.WriteLine("✓ Database Connection Successful");
            Console.WriteLine("Database : Hr_Sys");
            Console.WriteLine("Server   : localhost\\SQLEXPRESS");
            Console.WriteLine("=================================");
            SeedData.Initialize(db);
            Console.WriteLine("✓ Database Seeded Successfully");
        }
        else
        {
            Console.WriteLine("=================================");
            Console.WriteLine("✗ Database Connection Failed");
            Console.WriteLine("=================================");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("=================================");
        Console.WriteLine("✗ Database Connection Error");
        Console.WriteLine(ex.Message);
        Console.WriteLine("=================================");
    }
}
// ====================================


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        if (context.Response.ContentType is string contentType && contentType.Contains("text/html", StringComparison.OrdinalIgnoreCase))
        {
            // Set caching headers before the response starts to avoid read-only headers error
            context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";
        }
        return Task.CompletedTask;
    });

    await next();
});

app.MapStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();

static int GetNextAvailablePort(int startingPort)
{
    for (int port = startingPort; port < startingPort + 1000; port++)
    {
        try
        {
            using var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, port);
            listener.Start();
            listener.Stop();
            return port;
        }
        catch (System.Net.Sockets.SocketException)
        {
            // port in use, try next
        }
    }

    return startingPort;
}