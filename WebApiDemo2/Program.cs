using Microsoft.EntityFrameworkCore;
using WebApiDemo2.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); //apo 1 kai auto xreiazete giati milame gia controllers

builder.Services.AddDbContext<AplicationDbContext>(options => //to onomazoume kai dependency inject ousiastika kanoume inject to dbContext sto service list
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); //me auto ton tropo orizoume to service gia tin basi
}); //auta ta options ta dinoume sto constructor sto aplicationDbContext


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// routing 

app.MapControllers(); //1 ayto xreiazete gia na kanoume map ta http request sta controller

app.Run();

