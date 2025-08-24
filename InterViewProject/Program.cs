try
{
    Log.Information("Starting web host");

    var builder = WebApplication.CreateBuilder(args);
   
    Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // 從appsettings.json 讀取Logger設定
    .CreateLogger();
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
    // 注入IAuthorizationRepository介面，實作類別為AuthorizationRepositoryService
    builder.Services.AddScoped<IAuthorizationRepository, AuthorizationRepositoryService>();
    // 注入ProductExistsResourceFilter
    builder.Services.AddScoped<ProductExistsResourceFilter>();
    //啟用Razor Pages
    builder.Services.AddRazorPages();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;//默認的驗證方案
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//默認的挑戰方案
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;//默認的登入方案
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, //驗證金鑰
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("SecretKey") ?? throw new InvalidOperationException("SecretKey 未設定"))), //驗證金鑰
            ValidateLifetime = true, //驗證生命週期
            ValidateAudience = false, //驗證接收者
            ValidateIssuer = false, //驗證發行者這邊不需要因為我們只有一個
            ClockSkew = TimeSpan.Zero //設定時間偏移量
        };

        // 新增以下事件處理器，從 Cookie 中讀取 JWT
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // 檢查請求中是否包含名為 "access_token" 的 Cookie
                if (context.Request.Cookies.ContainsKey("access_token"))
                {
                    context.Token = context.Request.Cookies["access_token"];
                }
                return Task.CompletedTask;
            }
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
    });

    builder.Services.AddHttpClient("OurWebAPI", client =>
    {
        client.BaseAddress = new Uri("https://localhost:7266/api");//設定API的基底位址
    });


    // 添加 HttpContextAccessor
    builder.Services.AddHttpContextAccessor();


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

    //開啟Razor Pages
    app.MapRazorPages();
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