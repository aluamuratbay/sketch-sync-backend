using Carter;

namespace SketchSync.Endpoints;

public abstract class BaseModule : CarterModule
{
    protected BaseModule(params string[] modulePaths) : base(string.Join("/", modulePaths).ToLowerInvariant() + "/")
    {
        IncludeInOpenApi()
            .WithTags(modulePaths[^1])
            .WithDisplayName(modulePaths[^1]);
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
    }
}