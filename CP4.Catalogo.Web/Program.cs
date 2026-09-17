using CP4.Catalogo.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços do MVC
builder.Services.AddControllersWithViews();

// Configura o HttpClient para o ApiService injetando a BaseAddress definida no appsettings.json
builder.Services.AddHttpClient<ApiService>(client =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7030/";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

// Configura o pipeline de requisições HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Rota padrão do MVC apontando para Produtos/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Produtos}/{action=Index}/{id?}");

app.Run();