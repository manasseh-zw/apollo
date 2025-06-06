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
    }

    public static DatabaseOptions DatabaseOptions { get; } =
        new()
        {
            ConnectionString =
                Environment.GetEnvironmentVariable("DATABASE_URL")
                ?? throw new Exception("DB URL not set"),
            VectorConnectionString =
                Environment.GetEnvironmentVariable("VECTOR_DB_URL")
                ?? throw new Exception("Vector URL not set"),
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

    public static Google Google { get; } =
        new()
        {
            ApiKey =
                Environment.GetEnvironmentVariable("GOOGLE_AI_API_KEY")
                ?? throw new Exception("Google AI apikey is not set"),
        };

    public static OpenAI OpenAI { get; } =
        new()
        {
            ApiKey =
                Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new Exception("OPEN AI apikey is not set"),
        };

    public static ExaAI ExaAI { get; } =
        new()
        {
            ApiKey =
                Environment.GetEnvironmentVariable("EXA_API_KEY")
                ?? throw new Exception("Exa API key is not set"),
        };

    public static Models Models { get; } =
        new()
        {
            GptO4mini = "o4-mini",
            Gpt41 = "gpt-4.1",
            TextEmbeddingSmall = "text-embedding-3-small",
            GeminiProFlash25 = "gemini-2.5-flash-preview-04-17",
        };

    public static Client Client { get; } =
        new()
        {
            Url =
                Environment.GetEnvironmentVariable("CLIENT_URL")
                ?? throw new Exception("Client URL is not set"),
        };

    public static Resend Resend { get; } =
        new()
        {
            ApiKey =
                Environment.GetEnvironmentVariable("RESEND_API_KEY")
                ?? throw new Exception("Resend Apikey is not set"),
        };

    // public static Qdrant Qdrant { get; } =
    //     new()
    //     {
    //         ApiKey =
    //             Environment.GetEnvironmentVariable("QDRANT_API_KEY")
    //             ?? throw new Exception("QDRANT_API_KEY is not set"),
    //         Endpoint =
    //             Environment.GetEnvironmentVariable("QDRANT_ENDPOINT")
    //             ?? throw new Exception("QDRANT_ENDPOINT is not set"),
    //     };
}

public class DatabaseOptions
{
    public string ConnectionString { get; init; } = string.Empty;
    public string VectorConnectionString { get; init; } = string.Empty;
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

public class Google
{
    public string ApiKey { get; init; } = string.Empty;
}

public class OpenAI
{
    public string ApiKey { get; init; } = string.Empty;
}

public class Models
{
    public required string GptO4mini { get; set; }
    public required string Gpt41 { get; set; }
    public required string GeminiProFlash25 { get; set; }
    public required string TextEmbeddingSmall { get; set; }
}

public class ExaAI
{
    public required string ApiKey { get; init; } = string.Empty;
}

public class Client
{
    public required string Url { get; set; }
}

public class Resend
{
    public required string ApiKey { get; set; }
}

// public class Qdrant()
// {
//     public required string ApiKey { get; set; }
//     public required string Endpoint { get; set; }
// }
