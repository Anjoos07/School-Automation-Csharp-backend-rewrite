using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Forms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}



app.UseHttpsRedirection();
app.MapFormOperationsEndpoints();


app.Run();