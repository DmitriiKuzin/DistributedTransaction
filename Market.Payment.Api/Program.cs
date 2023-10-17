var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/proceedPayment", () => "Hello World!");

app.Run();