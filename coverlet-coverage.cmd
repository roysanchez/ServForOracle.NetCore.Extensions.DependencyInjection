@echo off

dotnet test .\ServForOracle.NetCore.Extensions.DI.Tests
coverlet .\ServForOracle.NetCore.Extensions.DI.Tests\bin\Debug\netcoreapp2.2\ServForOracle.NetCore.Extensions.DI.Tests.dll --target "dotnet" --targetargs "test .\ServForOracle.NetCore.Extensions.DI.Tests --no-build" --include "[ServForOracle.NetCore.Extensions.DependencyInjection]Microsoft.Extensions.DependencyInjection*"