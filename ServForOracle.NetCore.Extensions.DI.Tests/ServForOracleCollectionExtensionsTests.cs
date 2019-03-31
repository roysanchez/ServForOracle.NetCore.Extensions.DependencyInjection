using AutoFixture.Xunit2;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ServForOracle.NetCore.Extensions.DI.Tests
{
    public class ServForOracleCollectionExtensionsTests
    {
        public enum TestEnum
        {
            A,
            B
        }

        [Fact]
        public void Throws_ArgumentNull()
        {
            var serviceCollection = new ServiceCollection();

            Assert.Throws<ArgumentNullException>("connectionString", () => serviceCollection.AddServForOracle((string)null));
            Assert.Throws<ArgumentNullException>("services", () => ServForOracleCollectionExtensions.AddServForOracle(null, (string)null));
        }

        [Theory, AutoData]
        public void Dictionary_Throws_ArgumentNull(Dictionary<string, string> connectionStrings, string otherKey)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddServForOracle(connectionStrings);

            Assert.Throws<ArgumentNullException>("connectionStringsKeyNamed", () => serviceCollection.AddServForOracle((Dictionary<string, string>)null));
            Assert.Throws<ArgumentOutOfRangeException>("connectionStringsKeyNamed", () => serviceCollection.AddServForOracle(new Dictionary<string, string>()));
            Assert.Throws<ArgumentNullException>("services", () => ServForOracleCollectionExtensions.AddServForOracle(null, (string)null));
            Assert.Throws<ArgumentNullException>("services", () => ServForOracleCollectionExtensions.AddServForOracle(null, (Dictionary<string, string>)null));

            var provider = serviceCollection.BuildServiceProvider();
            var serviceLocator = provider.GetService<Func<string, IServiceForOracle>>();

            Assert.Throws<ArgumentNullException>("key", () => serviceLocator(null));
            Assert.Throws<KeyNotFoundException>(() => serviceLocator(otherKey));
        }

        [Theory, AutoData]
        public void ConnectionString(string connectionString)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddServForOracle(connectionString);

            var provider = serviceCollection.BuildServiceProvider();

            var serv = provider.GetService<IServiceForOracle>();

            Assert.NotNull(serv);
        }

        [Theory, AutoData]
        public void DictionaryString(Dictionary<string, string> connectionStrings)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddServForOracle(connectionStrings);

            var provider = serviceCollection.BuildServiceProvider();

            var servLocator = provider.GetService<Func<string, IServiceForOracle>>();
            Assert.NotNull(servLocator);

            foreach (var conStr in connectionStrings)
            {
                var service = servLocator(conStr.Key);
                Assert.NotNull(service);
            }
        }

        [Theory, AutoData]
        public void Enum_ListConnectionString(string conStringA, string conStringB)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddServForOracle<TestEnum>(new string[] { conStringA, conStringB });

            var provider = serviceCollection.BuildServiceProvider();

            var servLocator = provider.GetService<Func<TestEnum, IServiceForOracle>>();
            Assert.NotNull(servLocator);

            var serviceA = servLocator(TestEnum.A);
            var serviceB = servLocator(TestEnum.B);

            Assert.NotNull(serviceA);
            Assert.NotNull(serviceB);
        }

        [Theory, AutoData]
        public void Enum_DictionaryConnectionString(string conStringA, string conStringB)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddServForOracle<TestEnum>(new Dictionary<TestEnum, string>
                {
                    { TestEnum.A, conStringA },
                    { TestEnum.B, conStringB }
                });

            var provider = serviceCollection.BuildServiceProvider();

            var servLocator = provider.GetService<Func<TestEnum, IServiceForOracle>>();
            Assert.NotNull(servLocator);

            var serviceA = servLocator(TestEnum.A);
            var serviceB = servLocator(TestEnum.B);

            Assert.NotNull(serviceA);
            Assert.NotNull(serviceB);
        }
    }
}
