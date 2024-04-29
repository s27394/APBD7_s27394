using APBD7.Endpoints;
using APBD7.Interfaces;
using APBD7.Validators;
using APBD7.Interfaces;
using APBD7.Services;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbService, DbService>();
builder.Services.AddValidatorsFromAssemblyContaining<ProductWarehouseDTOValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.RegisterProductWarehouseEndpoints();

app.Run();