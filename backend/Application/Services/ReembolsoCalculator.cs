using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ReembolsoCalculator
{
    private readonly ILogger<ReembolsoCalculator> _logger;

    public ReembolsoCalculator(ILogger<ReembolsoCalculator> logger) => _logger = logger;

    public void Calcular(Reembolso reembolso)
    {
        if (reembolso.DataProcedimento < DateTime.Today.AddDays(-90))
        {
            reembolso.DefinirComoRejeitado();

            _logger.LogWarning(
                "Reembolso REJEITADO | Id: {Id} | Cliente: {ClienteId} | Data: {Data}",
                reembolso.Id,
                reembolso.ClienteId,
                reembolso.DataProcedimento
            );

            return;
        }

        decimal percentual = reembolso.TipoProcedimento switch
        {
            TipoProcedimento.ConsultaMedica => 0.8m,
            TipoProcedimento.ExameImagem => 0.9m,
            TipoProcedimento.ExameLaboratorial => 0.7m,
            _ => 0.5m
        };

        var valor = reembolso.ValorPago * percentual;

        if (valor > 500)
            valor = 500;

        reembolso.DefinirValorReembolso(valor);
    }
}
