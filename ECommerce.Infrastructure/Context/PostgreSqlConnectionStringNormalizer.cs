using Npgsql;

namespace ECommerce.Infrastructure.Context;

public static class PostgreSqlConnectionStringNormalizer
{
    public static string Normalize(string connectionString)
    {
        if (!Uri.TryCreate(connectionString, UriKind.Absolute, out var uri) ||
            (uri.Scheme != "postgresql" && uri.Scheme != "postgres"))
        {
            return connectionString;
        }

        var credentials = uri.UserInfo.Split(':', 2);
        if (credentials.Length != 2 || string.IsNullOrWhiteSpace(uri.AbsolutePath.Trim('/')))
        {
            throw new ArgumentException("PostgreSQL URL kullanıcı adı, parola ve veritabanı adı içermelidir.", nameof(connectionString));
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Database = Uri.UnescapeDataString(uri.AbsolutePath.Trim('/')),
            Username = Uri.UnescapeDataString(credentials[0]),
            Password = Uri.UnescapeDataString(credentials[1]),
            SslMode = SslMode.Require
        };

        foreach (var parameter in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = parameter.Split('=', 2);
            var key = Uri.UnescapeDataString(parts[0].Replace('+', ' '));
            var value = parts.Length > 1
                ? Uri.UnescapeDataString(parts[1].Replace('+', ' '))
                : string.Empty;

            if (key.Equals("sslmode", StringComparison.OrdinalIgnoreCase))
            {
                builder.SslMode = value.ToLowerInvariant() switch
                {
                    "disable" => SslMode.Disable,
                    "allow" => SslMode.Allow,
                    "prefer" => SslMode.Prefer,
                    "require" => SslMode.Require,
                    "verify-ca" => SslMode.VerifyCA,
                    "verify-full" => SslMode.VerifyFull,
                    _ => throw new ArgumentException("PostgreSQL URL içindeki sslmode değeri geçersiz.", nameof(connectionString))
                };
            }
            else if (key.Equals("channel_binding", StringComparison.OrdinalIgnoreCase))
            {
                builder.ChannelBinding = value.ToLowerInvariant() switch
                {
                    "disable" => ChannelBinding.Disable,
                    "prefer" => ChannelBinding.Prefer,
                    "require" => ChannelBinding.Require,
                    _ => throw new ArgumentException("PostgreSQL URL içindeki channel_binding değeri geçersiz.", nameof(connectionString))
                };
            }
        }

        return builder.ConnectionString;
    }
}
