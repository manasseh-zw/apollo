using dotenv.net;

namespace Apollo.Config;

public static class AppConfig
{
    public static void Initialize()
    {
        DotEnv.Load();
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

    public static OpenAIOptions OpenAIOptions { get; } =
        new()
        {
            ApiKey =
                Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new Exception("OpenAI API key is not set"),

            ModelId = Environment.GetEnvironmentVariable("OPENAI_MODEL_ID") ?? "o3-mini",

            Temperature = double.Parse(
                Environment.GetEnvironmentVariable("OPENAI_TEMPERATURE") ?? "0.7"
            ),
        };

    public static DeepSeekOptions DeepSeekOptions { get; } =
        new()
        {
            ApiKey =
                Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY")
                ?? throw new Exception("DeepSeek API key is not set"),

            ModelId = Environment.GetEnvironmentVariable("DEEPSEEK_MODEL_ID") ?? "deepseek-chat",
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

public class OpenAIOptions
{
    public string ApiKey { get; init; } = string.Empty;

    public string ModelId { get; init; } = string.Empty;

    public double Temperature { get; init; } = 0.7;
}

public class DeepSeekOptions
{
    public string ApiKey { get; init; } = string.Empty;

    public string ModelId { get; init; } = string.Empty;
}
