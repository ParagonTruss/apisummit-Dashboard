using Dashboard.Components;
using Dashboard.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register OpenAPI parsing service
builder.Services.AddSingleton<OpenApiService>();

// Register Paragon API client
builder.Services.AddHttpClient<ParagonApiClient>(client =>
{
    var baseUrl = builder.Configuration.GetValue<string>("ParagonApi:BaseUrl") 
        ?? "https://designserver.paragontruss.com/";
    client.BaseAddress = new Uri(baseUrl);
    
    // Add API key if configured
    var apiKey = builder.Configuration.GetValue<string>("ParagonApi:ApiKey");
    if (!string.IsNullOrEmpty(apiKey))
    {

        client.DefaultRequestHeaders.Add("Authorization", $"JWT {apiKey}");
    }
});

// Register dashboard state service
builder.Services.AddScoped<DashboardStateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
