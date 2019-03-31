using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ServForOracle.NetCore;
using ServForOracle.NetCore.Cache;
using ServForOracle.NetCore.OracleAbstracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServForOracleCollectionExtensions
    {
        public static IServiceCollection AddServForOracle(this IServiceCollection services, string connectionString)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if(string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            services.AddOptions();
            services.AddLogging();
            services.AddMemoryCache();

            services.TryAddSingleton(provider => ServForOracleCache.Create(provider.GetService<IMemoryCache>()));
            services.TryAddTransient<IDbConnectionFactory>(provider => new OracleDbConnectionFactory(connectionString));
            services.TryAddTransient<IServiceForOracle, ServiceForOracle>();

            return services;
        }

        public static IServiceCollection AddServForOracle(this IServiceCollection services, Dictionary<string, string> connectionStringsKeyNamed)
        {
            return AddServForOracle<string>(services, connectionStringsKeyNamed);
        }

        public static IServiceCollection AddServForOracle<T>(this IServiceCollection services, Dictionary<T, string> connectionStringsKeyNamed)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (connectionStringsKeyNamed is null)
            {
                throw new ArgumentNullException(nameof(connectionStringsKeyNamed));
            }
            if(!connectionStringsKeyNamed.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(connectionStringsKeyNamed));
            }

            services.AddOptions();
            services.AddLogging();
            services.AddMemoryCache();

            services.TryAddSingleton(provider => ServForOracleCache.Create(provider.GetService<IMemoryCache>()));

            services.AddTransient<Func<T, IServiceForOracle>>(provider => key =>
            {
                if (connectionStringsKeyNamed.ContainsKey(key))
                {
                    return new ServiceForOracle(provider.GetService<ILogger<ServiceForOracle>>(), provider.GetService<ServForOracleCache>(),
                        new OracleDbConnectionFactory(connectionStringsKeyNamed[key]));
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            });

            return services;
        }

        public static IServiceCollection AddServForOracle<T>(this IServiceCollection services, IEnumerable<string> connectionStrings)
            where T : Enum
        {
            var dictionary =
                Enum.GetValues(typeof(T)).Cast<T>()
                    .Zip(connectionStrings, (T key, string value) => new KeyValuePair<T, string>(key, value))
                    .ToDictionary(c => c.Key, c => c.Value);

            return AddServForOracle(services, dictionary);
        }
    }
}
