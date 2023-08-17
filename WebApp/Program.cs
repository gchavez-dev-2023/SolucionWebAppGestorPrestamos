using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using WebApp.Data;
using WebApp.Extension;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

//Agregar DinkToPDF
var architectureFolder = (IntPtr.Size == 8) ? "x_64" : "x_86";
var wkHtmlToPdfFileName = "libwkhtmltox";
if (architectureFolder == "x_64") //Solo paquetes 64 bit
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) //Linux para el docker
    {
        wkHtmlToPdfFileName += ".so";
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) //Windows para el Visual Basic
    {
        wkHtmlToPdfFileName += ".dll";
    }
}
else
{
    Console.WriteLine("DinktoPDF no es soportado para la versión del Sistema Operativo");
}

var wkHtmlToPdfPath = Path.Combine(
    new string[] {
        Directory.GetCurrentDirectory(),
        wkHtmlToPdfFileName
    });

CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(wkHtmlToPdfPath);
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
//Para convertir RAZOR a HTML para DINKTOPDF
builder.Services.AddTransient<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();

// Add services to the container.
/*var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));*/

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    try
    {
        options.UseMySql(connectionString,
            ServerVersion.AutoDetect(connectionString)
            );
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error al configurar DbContext: " + ex.Message);
        throw; // Re-lanzar la excepcion para que la aplicacion no continue si la configuracion falla.
    }
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>() //Se agrega para realizar la administración de roles
                .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    //Agregado para debug
    app.UseDeveloperExceptionPage();
    //app.UseDatabaseErrorPage();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
