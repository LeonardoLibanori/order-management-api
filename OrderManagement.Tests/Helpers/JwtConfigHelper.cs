using Microsoft.Extensions.Configuration;

namespace OrderManagement.Tests.Helpers;

public static class JwtConfigHelper
{
    public static IConfiguration GetConfiguration()
    {
        var config = new Dictionary<string, string?>
        {
            { "JwtSettings:Secret", "chave-secreta-para-testes-unitarios-32chars" },
            { "JwtSettings:Issuer", "OrderManagement.API" },
            { "JwtSettings:Audience", "OrderManagement.Client" },
            { "JwtSettings:ExpiresInHours", "8" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(config)
            .Build();
    }
}