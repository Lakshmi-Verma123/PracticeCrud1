using PracticeCrud1.Common;
using PracticeCrud1.Common.DAL;
using PracticeCrud1.Common.Repository;

var builder = WebApplication.CreateBuilder(args);

MyConnection.DefaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddSingleton<EmailService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new EmailService(config);
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();