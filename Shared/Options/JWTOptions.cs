namespace Shared.Authentication;
public class JWTOptions
{
    public double DurationInDays { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
}
