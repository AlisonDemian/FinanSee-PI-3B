namespace finansee_api.Services;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "finansee-api";
    public string Audience { get; set; } = "finansee-client";
    public string Secret { get; set; } =
        "finansee-secret-change-this-in-production-2026";
    public int ExpirationMinutes { get; set; } = 120;
}
