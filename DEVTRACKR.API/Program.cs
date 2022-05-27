using DEVTRACKR.API.Persistence;
using DEVTRACKR.API.Persistence.Repository;
using DEVTRACKR.API.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DevTrackRCs");
builder.Services.AddDbContext<DevTrackRContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddScoped<IPackageRepository, PackageRepository>();

//var sendGridApiKey = builder.Configuration.GetSection(key: "SendeApiKey").Value;

builder.Services.AddSendGrid(o => o.ApiKey = sendGridApiKey);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => {
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DevTrackR.API",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Mateus de Moraes",
            Email = "mateusdmoraes@yahoo.com.br",
            Url = new Uri("https://github.com/mateusmmoraes")
        }
    });

    //var xmlPath = Path.Combine(AppContext.BaseDirectory, "DevTrackR.API.xml");
    //o.IncludeXmlComments(xmlPath);
});


var app = builder.Build();

if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.MapControllers();

app.Run();
