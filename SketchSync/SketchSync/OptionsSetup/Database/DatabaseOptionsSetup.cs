using Microsoft.Extensions.Options;
using SketchSync.Database;

namespace SketchSync.OptionsSetup.Database;

public class DatabaseOptionsSetup(IConfiguration configuration) : IConfigureOptions<DatabaseOptions>
{
    private const string SectionName = "Database";
    
    public void Configure(DatabaseOptions options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}