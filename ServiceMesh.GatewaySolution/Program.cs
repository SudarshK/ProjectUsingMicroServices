using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using ServiceMesh.Services.GatewaySolutions.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables(prefix: "SERVICEMESH_GATEWAY__");
builder.AddAppAuthentication();
if(builder.Environment.EnvironmentName.ToString().ToLower().Equals("production"))
{
    builder.Configuration.AddJsonFile("Oselot.Production.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("Oselot.json", optional: false, reloadOnChange: true);
}
builder.Services.AddOcelot(builder.Configuration);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseOcelot().GetAwaiter().GetResult();
app.Run();
