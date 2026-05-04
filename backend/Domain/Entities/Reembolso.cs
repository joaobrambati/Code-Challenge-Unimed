using Domain.Enums;

namespace Domain.Entities;

public class Reembolso
{
    public int Id { get; set; }
    public TipoProcedimento TipoProcedimento { get; set; }
    public DateTime DataProcedimento { get; set; }
    public decimal ValorPago { get; set; }
    public int ClienteId { get; set; }

    public decimal ValorReembolsado { get; set; }
    public StatusReembolso Status { get; set; }

    public void DefinirComoRejeitado()
    {
        Status = StatusReembolso.Rejeitado;
    }

    public void DefinirComoAprovado()
    {
        Status = StatusReembolso.Aprovado;
    }

    public void DefinirComoFraude()
    {
        Status = StatusReembolso.SuspeitoDeFraude;
    }

    public void DefinirValorReembolso(decimal valor)
    {
        ValorReembolsado = valor;
    }
}
