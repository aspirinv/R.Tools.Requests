using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using R.Tools.Extensions;
using R.Tools.Requests.Contracts;
using R.Tools.Requests.Middleware;

namespace R.Tools.Requests
{
    public static class Extensions
    {
        public static IHostApplicationBuilder RegisterRequestsCollectorIntoMongoDb(this IHostApplicationBuilder builder,
            Action<CollectorOptions>? configure = null)
        {
            var section = builder.Configuration.GetSection("StorageOptions");
            var storageOptions = new StorageOptions();
            section.Bind(storageOptions);

            var options = new CollectorOptions();
            configure?.Invoke(options);

            builder.Services.AddSingleton(options);
            if (storageOptions.ConnectionString.IsEmpty())
            {
                throw new StorageOptionsMisconfigurationException();
            }
            
            builder.Services.AddSingleton(storageOptions);
            builder.Services.AddTransient<IRequestCollectMiddleware, RequestCollectMiddleware>();
            builder.Services.AddSingleton<IRequestStorage, MongoRequestStorage>();

            return builder;
        }
        public static IHostApplicationBuilder RegisterRequestsCollector(this IHostApplicationBuilder builder,
            Action<CollectorOptions>? configure = null)
        {
            var options = new CollectorOptions();
            configure?.Invoke(options);
            builder.Services.AddSingleton(options);
            builder.Services.AddScoped<IRequestCollectMiddleware, RequestCollectMiddleware>();

            return builder;
        }


        public static IApplicationBuilder CollectRequests(this IApplicationBuilder builder)
        {
            var storageService = builder.ApplicationServices.GetService<IRequestStorage>();
            if(storageService == null)
            {
                throw new StorageConfigurationMissingException();
            }
            builder.UseMiddleware<IRequestCollectMiddleware>();
            return builder;
        }
    }
}
