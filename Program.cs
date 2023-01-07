using Azure.Communication.Identity;
using Microsoft.EntityFrameworkCore;
using ChatBackend.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
builder.Services.AddDbContext<ChatBackendContext>(options =>
{
    string dbConnectionString = builder.Configuration.GetConnectionString("ChatBackendContext");
    options.UseSqlServer(dbConnectionString);
});


// CORSを一部許可 - 1
var client_app = "client_app";
var client_origin = builder.Configuration.GetValue<String>("ClientOrigin");
//var client_origin_2 = builder.Configuration.GetValue<String>("ClientOrigin2");
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: client_app,
        policy => policy.WithOrigins(
            client_origin
            //client_origin_2
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

var app = builder.Build();

// CORSを一部許可 - 2
app.UseCors(client_app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

