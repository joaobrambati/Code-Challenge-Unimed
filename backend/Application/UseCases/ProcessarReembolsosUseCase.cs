using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class ProcessarReembolsosUseCase
{
    private readonly ReembolsoCalculator _calculator;
    private readonly ReembolsoFraudChecker _fraudChecker;
    private readonly ILogger<ProcessarReembolsosUseCase> _logger;

    public ProcessarReembolsosUseCase(ReembolsoCalculator calculator, ReembolsoFraudChecker fraudChecker, ILogger<ProcessarReembolsosUseCase> logger)
    {
        _calculator = calculator;
        _fraudChecker = fraudChecker;
        _logger = logger;
    }

    public List<Reembolso> Executar(List<Reembolso> pedidos)
    {
        foreach (var pedido in pedidos)
        {
            if (pedido.DataProcedimento < DateTime.Now.AddDays(-90))
            {
                pedido.DefinirComoRejeitado();

                _logger.LogWarning(
                    "REJEITADO_90_DIAS | Id: {Id} | Cliente: {ClienteId}",
                    pedido.Id,
                    pedido.ClienteId
                );

                continue;
            }

            _calculator.Calcular(pedido);

            if (pedido.Status == StatusReembolso.Aprovado)
                pedido.DefinirComoAprovado();

        }

        _fraudChecker.AplicarRegras(pedidos);

        return pedidos;
    }
}
