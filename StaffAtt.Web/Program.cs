using StaffAtt.Web.StartupConfig;

var builder = WebApplication.CreateBuilder(args);

builder.AddAuthAndMvcServices();
builder.AddCustomServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();   // <--- REQUIRED FOR SESSION TO WORK
app.UseSession();        // must be after cookie policy

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Staff}/{action=Details}/{id?}");

app.MapRazorPages(); // keep, because you still have Razor views (Identity UI)

app.Run();