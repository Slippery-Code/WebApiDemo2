using WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("ShirtsApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7182/api/"); // base address apo to web api theloume to prwto url apo to launch settings (https) /api / giati etsi to exoume orisei sto controller tou backend programmatos
    client.DefaultRequestHeaders.Add("Accept", "application/json"); //an o provider gia to web api apaitei alla headers ta prosthetoume edw
}); //dependency injection  to onomazeis opws theleis h proti parametros

builder.Services.AddHttpClient("AuthorityApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7182/"); // base address apo to web api theloume to prwto url apo to launch settings (https) /api / giati etsi to exoume orisei sto controller tou backend programmatos
    client.DefaultRequestHeaders.Add("Accept", "application/json"); //an o provider gia to web api apaitei alla headers ta prosthetoume edw
});

builder.Services.AddTransient<IWebApiExecuter, WebApiExecuter>(); //dependency injection me interface thewreite  kai concrete implementation

//enabling session to save the JwtToken
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromHours(5);
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor(); //enabling sessions to save the jwtToken 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseSession();//dependency injection epeidh den einai prosbasimo apeutheias se custom klasi



app.MapControllerRoute( //default routing configured
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}") //edw mas leei oti to default pou emfanizetai otan anoigei to site einai autos o controller
    .WithStaticAssets();



 

app.Run();
