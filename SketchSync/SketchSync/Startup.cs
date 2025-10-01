using System.Text.Json.Serialization;
using Carter;
using FluentValidation;
using SketchSync.Database;
using SketchSync.Extensions;
using SketchSync.Middlewares;
using SketchSync.OptionsSetup;
using SketchSync.Services;

namespace SketchSync;

internal class Startup(IConfiguration configuration, IWebHostEnvironment environment)
{
    public IConfiguration Configuration { get; } = configuration;

    public IWebHostEnvironment Environment { get; set; } = environment;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptionsSetup(configuration);
        services.AddPersistence(configuration);
        services.AddServices(configuration);
        services.AddValidatorsFromAssemblyContaining<AssemblyReference>();

        services.AddJwtBearerAuthentication();
            
        services
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(c => c.AddSwaggerConfiguration())
            .AddCarter()
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddSignalR();
        
        services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            })
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
    }
    
    public static void Configure(IApplicationBuilder app)
    {   
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ExceptionMiddleware>();
    }
}