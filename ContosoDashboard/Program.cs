using Microsoft.EntityFrameworkCore;
using ContosoDashboard.Data;
using ContosoDashboard.Models;
using ContosoDashboard.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add authentication state provider for Blazor
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// Configure Database
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine($"DefaultConnection: {defaultConnection}");

// If running in Development and the configured DefaultConnection points to LocalDB,
// prefer a local SQLite file for a smoother developer experience when LocalDB isn't available.
var usesLocalDb = !string.IsNullOrWhiteSpace(defaultConnection) &&
    defaultConnection.IndexOf("localdb", StringComparison.OrdinalIgnoreCase) >= 0;

var useSqlServer = !string.IsNullOrWhiteSpace(defaultConnection) && !(builder.Environment.IsDevelopment() && usesLocalDb);

if (useSqlServer)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(defaultConnection));
}
else
{
    // Use a local SQLite file for development fallback
    var dataDir = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
    Directory.CreateDirectory(dataDir);
    var sqlitePath = Path.Combine(dataDir, "contosodashboard.db");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite($"Data Source={sqlitePath}"));
}

// Configure Mock Authentication (Cookie-based for training purposes)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Employee", policy => policy.RequireRole("Employee", "TeamLead", "ProjectManager", "Administrator"));
    options.AddPolicy("TeamLead", policy => policy.RequireRole("TeamLead", "ProjectManager", "Administrator"));
    options.AddPolicy("ProjectManager", policy => policy.RequireRole("ProjectManager", "Administrator"));
    options.AddPolicy("Administrator", policy => policy.RequireRole("Administrator"));
});

// Register application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddSingleton<IDocumentValidationService, DocumentValidationService>();
builder.Services.Configure<DocumentStorageOptions>(builder.Configuration.GetSection("DocumentStorage"));

// Add HttpContextAccessor for accessing user claims
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated(); // For development - use migrations in production

        if (app.Environment.IsDevelopment())
        {
            var connection = context.Database.GetDbConnection();
            try
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Documents';";
                var tableName = command.ExecuteScalar();

                if (tableName == null)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogWarning("Documents table missing in the SQLite database. Repairing development database schema without deleting the file.");
                    CreateMissingDocumentTables(context);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    // Use HSTS even in development for training purposes
    app.UseHsts();
}

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    
    // Content Security Policy for Blazor Server
    context.Response.Headers["Content-Security-Policy"] = 
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; " +
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
        "font-src 'self' https://cdn.jsdelivr.net; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self' wss: ws:;";
    
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


void CreateMissingDocumentTables(ApplicationDbContext context)
{
    context.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS Documents (
            DocumentId INTEGER PRIMARY KEY AUTOINCREMENT,
            Title TEXT NOT NULL,
            Description TEXT,
            Category TEXT NOT NULL,
            Tags TEXT,
            OriginalFileName TEXT NOT NULL,
            ContentType TEXT NOT NULL,
            StoragePath TEXT NOT NULL,
            FileSizeBytes INTEGER NOT NULL,
            UploadDateUtc TEXT NOT NULL,
            UploadedById INTEGER NOT NULL,
            ProjectId INTEGER NULL,
            IsShared INTEGER NOT NULL DEFAULT 0,
            FOREIGN KEY (UploadedById) REFERENCES Users(UserId) ON DELETE RESTRICT,
            FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId) ON DELETE SET NULL
        );
        CREATE TABLE IF NOT EXISTS DocumentShares (
            DocumentShareId INTEGER PRIMARY KEY AUTOINCREMENT,
            DocumentId INTEGER NOT NULL,
            RecipientUserId INTEGER NULL,
            RecipientTeam TEXT,
            SharedByUserId INTEGER NOT NULL,
            SharedOnUtc TEXT NOT NULL,
            FOREIGN KEY (DocumentId) REFERENCES Documents(DocumentId) ON DELETE CASCADE,
            FOREIGN KEY (RecipientUserId) REFERENCES Users(UserId) ON DELETE RESTRICT,
            FOREIGN KEY (SharedByUserId) REFERENCES Users(UserId) ON DELETE RESTRICT
        );
        CREATE INDEX IF NOT EXISTS IX_Documents_UploadedById ON Documents (UploadedById);
        CREATE INDEX IF NOT EXISTS IX_Documents_ProjectId ON Documents (ProjectId);
        CREATE INDEX IF NOT EXISTS IX_Documents_Category ON Documents (Category);
        CREATE INDEX IF NOT EXISTS IX_DocumentShares_DocumentId ON DocumentShares (DocumentId);
        CREATE INDEX IF NOT EXISTS IX_DocumentShares_RecipientUserId ON DocumentShares (RecipientUserId);
        CREATE INDEX IF NOT EXISTS IX_DocumentShares_SharedByUserId ON DocumentShares (SharedByUserId);
    ");
}

app.Run();
