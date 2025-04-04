using Xunit;

namespace Apollo.Tests;

public class SkipIfNoApiKeyFactAttribute : FactAttribute
{
    public SkipIfNoApiKeyFactAttribute()
    {
        var exaKey = Environment.GetEnvironmentVariable("EXA_API_KEY");
        var firecrawlKey = Environment.GetEnvironmentVariable("FIRECRAWL_API_KEY");

        if (string.IsNullOrEmpty(exaKey) && string.IsNullOrEmpty(firecrawlKey))
        {
            Skip = "Skipping because no API keys are set in environment variables";
        }
    }
}