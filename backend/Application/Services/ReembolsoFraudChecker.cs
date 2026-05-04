using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ReembolsoFraudChecker
{
    private readonly ILogger<ReembolsoFraudChecker> _logger;

    public ReembolsoFraudChecker(ILogger<ReembolsoFraudChecker> logger) => _logger = logger;

    public void AplicarRegras(List<Reembolso> reembolsos)
    {
        var grupos = reembolsos.GroupBy(r => r.ClienteId);

        foreach (var grupo in grupos)
        {
            var pedidos = grupo
                .OrderBy(r => r.DataProcedimento)
                .ThenBy(r => r.Id)
                .ToList();

            foreach (var pedido in pedidos)
            {
                var ultimos30Dias = pedidos
                    .Where(p =>
                        p.DataProcedimento >= pedido.DataProcedimento.AddDays(-30)
                        && (
                            p.DataProcedimento < pedido.DataProcedimento
                            || (p.DataProcedimento == pedido.DataProcedimento && p.Id <= pedido.Id)
                        )
                    )
                    .ToList();

                if (ultimos30Dias.Count > 5)
                {
                    pedido.DefinirComoFraude();

                    _logger.LogWarning(
                        "FRAUDE (QUANTIDADE) | Cliente: {ClienteId} | PedidoId: {Id}",
                        pedido.ClienteId,
                        pedido.Id
                    );

                    continue;
                }

                var total = ultimos30Dias
                    .Where(p => p.Status != StatusReembolso.SuspeitoDeFraude)
                    .Sum(p => p.ValorReembolsado);

                if (total > 1500)
                {
                    pedido.DefinirComoFraude();

                    _logger.LogWarning(
                        "FRAUDE (VALOR) | Cliente: {ClienteId} | PedidoId: {Id} | Total: {Total}",
                        pedido.ClienteId,
                        pedido.Id,
                        total
                    );
                }
            }
        }
    }

}
