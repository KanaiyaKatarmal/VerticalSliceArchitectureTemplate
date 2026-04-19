using FluentValidation;
using Scalar.AspNetCore;
using System.Reflection;
using VerticalSliceArchitectureTemplate.Exceptions;
using VerticalSliceArchitectureTemplate.Extensions;
using VerticalSliceArchitectureTemplate.Features.Book.CreateBook;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSQLDatabaseConfiguration(builder.Configuration);
builder.Services.RegisterApiEndpointsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddHealthChecksConfiguration();
builder.Services.AddValidatorsFromAssembly(typeof(CreateBookValidator).Assembly);
builder.Services.AddHandlersFromAssembly(typeof(Program).Assembly);
builder.Services.AddExceptionHandler<CustomExceptionHandler>().AddProblemDetails();

var app = builder.Build();

app.MapApiEndpoints();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHealthChecks();

app.MapScalarApiReference(options =>
{
    options.WithTheme(ScalarTheme.DeepSpace);
    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.Run();
