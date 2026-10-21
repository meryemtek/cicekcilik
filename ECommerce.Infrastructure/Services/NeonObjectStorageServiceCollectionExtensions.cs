using Amazon.Runtime;
using Amazon.S3;
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure.Services;

public static class NeonObjectStorageServiceCollectionExtensions
{
    public static IServiceCollection AddNeonObjectStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IAmazonS3>(_ =>
        {
            var endpoint = configuration["AWS_ENDPOINT_URL_S3"]
                ?? throw new InvalidOperationException("AWS_ENDPOINT_URL_S3 ayarı eksik.");
            var region = configuration["AWS_REGION"]
                ?? throw new InvalidOperationException("AWS_REGION ayarı eksik.");
            var accessKey = configuration["AWS_ACCESS_KEY_ID"]
                ?? throw new InvalidOperationException("AWS_ACCESS_KEY_ID ayarı eksik.");
            var secretKey = configuration["AWS_SECRET_ACCESS_KEY"]
                ?? throw new InvalidOperationException("AWS_SECRET_ACCESS_KEY ayarı eksik.");

            return new AmazonS3Client(
                new BasicAWSCredentials(accessKey, secretKey),
                new AmazonS3Config
                {
                    ServiceURL = endpoint,
                    AuthenticationRegion = region,
                    ForcePathStyle = true
                });
        });
        services.AddScoped<IDosyaServisi, NeonObjectStorageDosyaServisi>();

        return services;
    }
}
