using System.Reflection;
using System.Runtime.Serialization;
using Carter.OpenApi;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SketchSync.Exceptions;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace SketchSync.Extensions;

public static class SwaggerGenOptionsExtensions
{
    public static void AddSwaggerConfiguration(this SwaggerGenOptions options)
    {
        const string bearer = "Bearer";
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter JWT Bearer token",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = bearer,
            BearerFormat = "JWT"
        };

        options.UseAllOfToExtendReferenceSchemas();

        options.SchemaFilter<EnumSchemaFilter>();
        
        options.SchemaFilter<JsonIgnoreSchemaFilter>();

        options.CustomSchemaIds(type => type.ToString());

        options.AddSecurityDefinition(bearer, securityScheme);

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = bearer
                    }
                },
                Array.Empty<string>()
            }
        });

        options.DocInclusionPredicate((_, description) =>
            description.ActionDescriptor.EndpointMetadata.OfType<IIncludeOpenApi>().Any());
    }
}

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum && !(Nullable.GetUnderlyingType(context.Type)?.IsEnum ?? false)) return;
        model.Enum.Clear();
        var enumType = Nullable.GetUnderlyingType(context.Type) ?? context.Type;
        foreach (var enumName in Enum.GetNames(enumType))
        {
            var memberInfo = context.Type.GetMember(enumName).FirstOrDefault(m => m.DeclaringType == context.Type)!;
            var enumMemberAttribute = memberInfo == null
                ? null!
                : memberInfo.GetCustomAttributes(typeof(EnumMemberAttribute), false).OfType<EnumMemberAttribute>()
                    .FirstOrDefault()!;
            var label = enumMemberAttribute == null || string.IsNullOrWhiteSpace(enumMemberAttribute.Value)
                ? enumName
                : enumMemberAttribute.Value;
            model.Enum.Add(new OpenApiString(label));
        }
    }
}

public class JsonIgnoreSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null || context?.Type == null) return;

        foreach (var property in context.Type.GetProperties())
        {
            var jsonIgnoreAttribute = property.GetCustomAttribute<JsonIgnoreAttribute>();
            if (jsonIgnoreAttribute != null && schema.Properties.ContainsKey(property.Name))
            {
                schema.Properties.Remove(property.Name);
            }
        }
    }
}
