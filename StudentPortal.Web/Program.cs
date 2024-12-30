using Microsoft.EntityFrameworkCore;
using StudentPortal.Web.Data;
using Microsoft.AspNetCore.Identity;
using NLog;
using NLog.Web;

var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

try
{
    logger.Debug("Application starting... HEHEHE");

    var builder = WebApplication.CreateBuilder(args);

    // Configure NLog as the logging provider
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("StudentPortal")));


    builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
        .AddRoles<IdentityRole>() //Roles
        .AddEntityFrameworkStores<ApplicationDbContext>();

    var app = builder.Build();

    // Test logging at various levels
    logger.Debug("This is a Debug message");
    logger.Warn("This is a Warning message");
    logger.Error("This is an Error message");


    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;

        try
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Add default roles if they don't exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Optionally, create an admin user and assign the Admin role
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
                await userManager.CreateAsync(adminUser, "Admin@123"); // Ensure a strong password
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }


        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred during role initialization.");
        }
    }

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

    app.UseAuthentication(); // Enable Authentication
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    // Log fatal errors
    logger.Fatal(ex, "Application failed to start.");
    throw;
}
finally
{
    // Ensure proper shutdown of NLog
    LogManager.Shutdown();
}







