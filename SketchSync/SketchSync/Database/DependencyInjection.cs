using System.Text.Json;
using JasperFx;
using Marten;
using Microsoft.Extensions.Options;
using SketchSync.Database.Repositories.JwtToken;
using SketchSync.Database.Repositories.Users;
using SketchSync.Database.Repositories.Boards;
using SketchSync.Entities;
using Weasel.Core;

namespace SketchSync.Database;

public static class DependencyInjection
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        AddMarten(services, configuration);
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBoardRepository, BoardRepository>();
        services.AddScoped<IJwtTokenRepository, JwtTokenRepository>();
    }
    
    private static void AddMarten(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMarten(db =>
        {
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>();
            var connectionString = options.Value.Postgres;
        
            db.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
            db.UseSystemTextJsonForSerialization(JsonSerializerOptions.Default, EnumStorage.AsString);
            db.Connection(connectionString);
        
            db.Schema.For<User>()
                .DocumentAlias("users");
        
            db.Schema.For<JwtToken>()
                .DocumentAlias("jwt_tokens");
        
            db.Schema.For<UserJwtToken>()
                .DocumentAlias("user_jwt_tokens")
                .ForeignKey<User>(x => x.UserId)
                .ForeignKey<JwtToken>(x => x.JwtTokenId);

            db.Schema.For<Board>()
                .DocumentAlias("boards")
                .ForeignKey<User>(x => x.OwnerId);
        });
    }
}