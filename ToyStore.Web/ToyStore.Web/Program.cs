using ToyStore.Web.Components;
using ToyStore.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HttpClient tipado para o ProductService, com BaseAddress centralizada via configuração.
// Nenhuma instância de HttpClient é criada manualmente em nenhum outro lugar da aplicação.
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException("ApiSettings:BaseUrl não configurado em appsettings.json.");

builder.Services.AddHttpClient<ProductService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Carrinho em memória — um por circuito/sessão do usuário (Scoped).
builder.Services.AddScoped<CartService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
