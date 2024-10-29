using overhaul_teste.Libraries.Login;
using overhaul_teste.Repositorio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Adicionado para manipular a Sessão
builder.Services.AddHttpContextAccessor();
//Adicionar a Interface como um serviço 
// Adicionar serviços 
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<ICarroRepositorio, CarroRepositorio>();
builder.Services.AddScoped<ICarrinhoRepositorio, CarrinhoRepositorio>();
builder.Services.AddScoped<ICompraRepositorio, CompraRepositorio>();
builder.Services.AddScoped<ITestDriveRepositorio, TestDriveRepositorio>();

builder.Services.AddScoped<overhaul_teste.Libraries.Sessao.Sessao>();
builder.Services.AddScoped<LoginCliente>();

//Add Gerenciador Arquivo como servi?os

builder.Services.AddScoped<overhaul_teste.Cookie.Cookie>();
builder.Services.AddScoped<overhaul_teste.CarrinhoCompra.CookieCarrinhoCompra>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
