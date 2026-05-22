using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Recommenda.Application.Repositories;
using Recommenda.Application.Services;
using Recommenda.Infrastructure;
using Recommenda.Infrastructure.Persistence;
using Recommenda.Infrastructure.Persistence.Repositories;

namespace Recommenda.API.Extensions;

/// <summary>
/// Extensoes para registrar servicos, repositorios e documentacao da solucao Recommenda na DI.
/// </summary>
public static class RecommendaServiceCollectionExtensions
{
    /// <summary>
    /// Registra o <see cref="RecommendaContext"/> com MySQL.
    /// A connection string e lida de <c>ConnectionStrings:RecommendaMySQL</c>.
    /// </summary>
    public static IServiceCollection AddRecommendaDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RecommendaMySQL")
            ?? throw new InvalidOperationException(
                "Connection string 'RecommendaMySQL' nao encontrada. Configure em appsettings.json.");

        services.AddDbContext<RecommendaContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        return services;
    }

    /// <summary>
    /// Registra o repositorio generico <see cref="IRepository{T}"/> e todos os repositorios
    /// especificos por agregado como <c>Scoped</c> (um por requisicao HTTP).
    /// </summary>
    public static IServiceCollection AddRecommendaRepositories(this IServiceCollection services)
    {
        // Repositorio generico — cobre CRUD basico de qualquer entidade de dominio
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Repositorios especificos com consultas alem do CRUD minimo
        services.AddScoped<IArtistRepository,      ArtistRepository>();
        services.AddScoped<IAlbumRepository,       AlbumRepository>();
        services.AddScoped<ITrackRepository,       TrackRepository>();
        services.AddScoped<IGenreRepository,       GenreRepository>();
        services.AddScoped<IUserRepository,        UserRepository>();
        services.AddScoped<IAlbumRatingRepository, AlbumRatingRepository>();
        services.AddScoped<ITrackRatingRepository, TrackRatingRepository>();
        services.AddScoped<IPlaylistRepository,    PlaylistRepository>();

        return services;
    }

    /// <summary>
    /// Configura o Swagger/OpenAPI com metadados do dominio e comentarios XML.
    /// </summary>
    public static IServiceCollection AddRecommendaSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title       = "Recommenda API",
                Version     = "v1",
                Description =
                    "API REST para descoberta musical: gerenciamento de artistas, albuns, " +
                    "faixas, generos e avaliacoes de usuarios.",
                Contact = new OpenApiContact
                {
                    Name  = "Equipe Recommenda",
                    Email = "contato@recommenda.example.com"
                }
            });

            // Comentarios XML dos controllers refletidos na UI do Swagger
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });

        return services;
    }
}
