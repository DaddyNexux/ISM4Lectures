using FuckingLectures.Extensions;
using FuckingLectures.Extentions;
using FuckingLectures.Helpers;
using FuckingLectures.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigProvider.config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSizeLimit();
builder.Services.AddDbConnection();
builder.Services.AddCorss();
builder.Services.AddIdentityConfig();
builder.Services.AddAuthConfig();
builder.Services.AddSwaggerConfig();
builder.Services.CentralizeAPiRespose();
builder.Services.AddServices();
/*
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80);
});*/

var app = builder.Build();


app.UseHsts();
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
await app.UseIdentitySeedRoles(Roles.SuperAdmin, Roles.Admin, Roles.User);
app.UseAuth();

await app.UseSeeder();


app.UseCustomSwagger();
//app.UseContentSecurityPolicy();


app.MapControllers();
app.MapFallbackToFile("index.html");


app.Run();