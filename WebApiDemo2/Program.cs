using Microsoft.EntityFrameworkCore;
using WebApiDemo2.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); //apo 1 kai auto xreiazete giati milame gia controllers

/*builder.Services.AddAuthentication("Bearer") //dependency injection gia authorization kai authentication
.AddJwtBearer("Bearer", options => 
{
    options.Authority = "https://localhost:7182"; //Identity Server URL
    options.Audience = "ShirtStoreManagementApi"; //API resource name
});   2 */

builder.Services.AddDbContext<AplicationDbContext>(options => //to onomazoume kai dependency inject ousiastika kanoume inject to dbContext sto service list
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); //me auto ton tropo orizoume to service gia tin basi
}); //auta ta options ta dinoume sto constructor sto aplicationDbContext




builder.Services.AddHttpContextAccessor();   


//enabling session to save the JwtToken


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// routing 

app.MapControllers(); //1 ayto xreiazete gia na kanoume map ta http request sta controller

/*app.UseAuthentication(); //diathesima mesw jwt bearer nugget package
app.UseAuthorization();  3 */



//apo 2 kai 3 mporeis na kanei authorization xwris na exeis filtro alla prepei na oriseis kai tous parametrous(validationParameters) kai den xreiazete oute na ftiakseis to authenticator.cs oute to filtro jwtTokenAuthFilterAttribute
app.Run();

