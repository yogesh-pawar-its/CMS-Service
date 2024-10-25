using ContactsWebApi;
using ContactsWebApi.Repositories;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

#region Extensions

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed((host) => true);
                      });
});


builder.Services.AddControllers().AddJsonOptions(jsonOptions =>
{
    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contacts API", Version = "v1" });
});
builder.Services.AddScoped<IContactRepository, ContactRepository>();

builder.Services.AddMvc();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

#endregion

#region App Setup

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contacts API v1"); }
);

app.UseCors("*");
app.UseForwardedHeaders();
app.UseCors(MyAllowSpecificOrigins);
app.UseCors("AllowAllOrigins");
app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapControllers();

#endregion

app.Run();
