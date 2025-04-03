using dotenv.net;

namespace Apollo.Config;

public static class AppConfig
{
    public static void Initialize()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (string.Equals(environment, "Development"))
        {
            DotEnv.Load();
        }
        Console.WriteLine(AzureAI.Endpoint);
        Console.WriteLine(AzureAI.ApiKey);
    }

    public static DatabaseOptions DatabaseOptions { get; } =
        new()
        {
            ConnectionString =
                $"Host={Environment.GetEnvironmentVariable("DB_HOST")};"
                + $"Port={Environment.GetEnvironmentVariable("DB_PORT")};"
                + $"Database={Environment.GetEnvironmentVariable("DB_NAME")};"
                + $"Username={Environment.GetEnvironmentVariable("DB_USER")};"
                + $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}",
        };

    public static JwtOptions JwtOptions { get; } =
        new()
        {
            Secret =
                Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? throw new Exception("JWT secret key is not set"),

            Issuer =
                Environment.GetEnvironmentVariable("JWT_ISSUER")
                ?? throw new Exception("JWT issuer is not set"),

            Audience =
                Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                ?? throw new Exception("JWT audience is not set"),
        };

    public static AzureAI AzureAI { get; } =
        new()
        {
            ApiKey =
                Environment.GetEnvironmentVariable("AZURE_AI_API_KEY")
                ?? throw new Exception("Azure AI API key is not set"),

            Endpoint =
                Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")
                ?? throw new Exception("Azure AI Endpoint is not set"),
        };

    public static ExaAI ExaAI { get; } =
        new()
        {
            ApiKey =
                Environment.GetEnvironmentVariable("EXA_API_KEY")
                ?? throw new Exception("Exa API key is not set"),
        };

    public static FirecrawlAI FirecrawlAI { get; } =
        new()
        {
            ApiKey =
                Environment.GetEnvironmentVariable("FIRECRAWL_API_KEY")
                ?? throw new Exception("Firecrawl API key is not set"),
        };
}

public class DatabaseOptions
{
    public string ConnectionString { get; init; } = string.Empty;
}

public class JwtOptions
{
    public string Secret { get; init; } = string.Empty;

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;
}

public class AzureAI
{
    public string ApiKey { get; init; } = string.Empty;
    public string Endpoint { get; init; } = string.Empty;
}

public class ExaAI
{
    public string ApiKey { get; init; } = string.Empty;
}

public class FirecrawlAI
{
    public string ApiKey { get; init; } = string.Empty;
}
