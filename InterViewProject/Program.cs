try
{
    Log.Information("Starting web host");

    var builder = WebApplication.CreateBuilder(args);
   
    Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // 從appsettings.json 讀取Logger設定
    .CreateLogger();
    //啟用Razor Pages
    builder.Services.AddRazorPages();
    // 註冊DbContext，並設定使用SQL Server資料庫
    builder.Services.AddDbContext<CoffeeContext>(
   options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Host.UseSerilog();
    // 注入IProductListRepository介面，實作類別為ProductListRepositoryService
    builder.Services.AddScoped<IProductListRepository, ProductListRepositoryService>();
    // 注入IProductRepository介面，實作類別為ProductRepositoryService
    builder.Services.AddScoped<IProductRepository, ProductRepositoryService>();
    // 注入ProductExistsResourceFilter
    builder.Services.AddScoped<ProductExistsResourceFilter>();
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
    //開啟Razor Pages
    app.MapRazorPages();
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