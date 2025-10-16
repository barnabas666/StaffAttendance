using Microsoft.AspNetCore.Identity;
using StaffAtt.Web.StartupConfig;

var builder = WebApplication.CreateBuilder(args);

builder.AddIdentityAndMvcServices();
builder.AddCustomServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Staff}/{action=Details}/{id?}");
app.MapRazorPages();

await IdentitySeedExtensions.SeedRolesAndAdminAsync(app.Services);

app.Run();
