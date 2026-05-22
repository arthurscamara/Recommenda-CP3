using Microsoft.EntityFrameworkCore;
using Recommenda.API.Exceptions;
using Recommenda.API.Extensions;
using Recommenda.Infrastructure.Persistence;

namespace Recommenda.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // -----------------------------------------------------------------------
        // Banco de dados
        // -----------------------------------------------------------------------
        builder.Services.AddRecommendaDbContext(builder.Configuration);

        // -----------------------------------------------------------------------
        // Repositorios (generico + especificos por agregado)
        // -----------------------------------------------------------------------
        builder.Services.AddRecommendaRepositories();

        // -----------------------------------------------------------------------
        // Controllers e documentacao
        // -----------------------------------------------------------------------
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddRecommendaSwagger();

        // -----------------------------------------------------------------------
        // Tratamento global de excecoes (RFC 7807 — ProblemDetails)
        // -----------------------------------------------------------------------
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        var app = builder.Build();

        // UseExceptionHandler deve vir antes de qualquer middleware que possa
        // lancar excecoes, incluindo o Swagger e o MapControllers.
        app.UseExceptionHandler();

        // Aplica migrations pendentes automaticamente ao iniciar
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<RecommendaContext>();
            db.Database.Migrate();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Recommenda API v1");
                options.RoutePrefix = "swagger";
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
