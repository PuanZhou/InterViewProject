try
{
    Log.Information("Starting web host");

    var builder = WebApplication.CreateBuilder(args);
   
    Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // 從appsettings.json 讀取Logger設定
    .CreateLogger();
    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Host.UseSerilog();
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

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}