using Application.UseCases;
using Infrastructure.Csv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// 🔧 1. Configurar DI + Logging
var services = new ServiceCollection();

services.AddLogging(config =>
{
    config.AddConsole();
});

services.AddTransient<ProcessarReembolsosUseCase>();
services.AddTransient<Application.Services.ReembolsoCalculator>();
services.AddTransient<Application.Services.ReembolsoFraudChecker>();

var provider = services.BuildServiceProvider();

// INPUT
var input = Console.In.ReadToEnd();

// CSV → objetos
var reader = new CsvInputReader();
var pedidos = reader.Ler(input);

// Processamento
var useCase = provider.GetRequiredService<ProcessarReembolsosUseCase>();
var resultado = useCase.Executar(pedidos);

// objetos → CSV
var writer = new CsvOutputWriter();
var output = writer.Gerar(resultado);

// OUTPUT
Console.WriteLine("\n\nTabela:\n");
Console.WriteLine(output);