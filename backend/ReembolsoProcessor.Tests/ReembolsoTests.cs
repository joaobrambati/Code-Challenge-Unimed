using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace ReembolsoProcessor.Tests;

public class ReembolsoTests
{
    private readonly ReembolsoCalculator _calculator;
    private readonly ReembolsoFraudChecker _fraudChecker;

    public ReembolsoTests()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        services.AddTransient<ReembolsoCalculator>();
        services.AddTransient<ReembolsoFraudChecker>();

        var provider = services.BuildServiceProvider();

        _calculator = provider.GetRequiredService<ReembolsoCalculator>();
        _fraudChecker = provider.GetRequiredService<ReembolsoFraudChecker>();
    }

    // =========================
    // 💰 CÁLCULO
    // =========================
    [Fact]
    public void Deve_Calcular_Consulta_Medica_80_Porcento()
    {
        var r = Criar(TipoProcedimento.ConsultaMedica, 400);

        _calculator.Calcular(r);

        Assert.Equal(320, r.ValorReembolsado);
    }

    [Fact]
    public void Deve_Calcular_Exame_Imagem_90_Porcento()
    {
        var r = Criar(TipoProcedimento.ExameImagem, 400);

        _calculator.Calcular(r);

        Assert.Equal(360, r.ValorReembolsado);
    }

    [Fact]
    public void Deve_Calcular_Laboratorial_70_Porcento()
    {
        var r = Criar(TipoProcedimento.ExameLaboratorial, 400);

        _calculator.Calcular(r);

        Assert.Equal(280, r.ValorReembolsado);
    }

    [Fact]
    public void Deve_Calcular_Outros_50_Porcento()
    {
        var r = Criar(TipoProcedimento.Outros, 400);

        _calculator.Calcular(r);

        Assert.Equal(200, r.ValorReembolsado);
    }

    [Fact]
    public void Deve_Respeitar_Limite_De_500()
    {
        var r = Criar(TipoProcedimento.ConsultaMedica, 2000);

        _calculator.Calcular(r);

        Assert.Equal(500, r.ValorReembolsado);
    }

    // =========================
    // ⛔ REJEIÇÃO (90 DIAS)
    // =========================
    [Fact]
    public void Deve_Rejeitar_Se_For_Maior_Que_90_Dias()
    {
        var r = Criar(TipoProcedimento.ConsultaMedica, 1000, DateTime.Now.AddDays(-100));

        _calculator.Calcular(r);

        Assert.Equal(StatusReembolso.Rejeitado, r.Status);
        Assert.Equal(0, r.ValorReembolsado);
    }

    // =========================
    // 🚨 FRAUDE - QUANTIDADE
    // =========================
    [Fact]
    public void Deve_Marcar_Fraude_Quando_Mais_De_5_Em_30_Dias()
    {
        var baseDate = DateTime.Today;

        var lista = Enumerable.Range(0, 6)
            .Select(i => Criar(
                TipoProcedimento.ConsultaMedica,
                200,
                baseDate.AddDays(i),
                clienteId: 1))
            .ToList();

        lista.ForEach(r => _calculator.Calcular(r));

        _fraudChecker.AplicarRegras(lista);

        Assert.Equal(StatusReembolso.SuspeitoDeFraude, lista.Last().Status);
    }

    // =========================
    // 🚨 FRAUDE - VALOR
    // =========================
    [Fact]
    public void Deve_Marcar_Fraude_Quando_Valor_Ultrapassar_1500()
    {
        var lista = new List<Reembolso>
        {
            Criar(TipoProcedimento.ExameImagem, 800, DateTime.Today.AddDays(-3), 1),
            Criar(TipoProcedimento.ExameImagem, 800, DateTime.Today.AddDays(-2), 1),
            Criar(TipoProcedimento.ExameImagem, 800, DateTime.Today.AddDays(-1), 1),
            Criar(TipoProcedimento.ExameImagem, 800, DateTime.Today, 1)
        };

        lista.ForEach(r => _calculator.Calcular(r));

        _fraudChecker.AplicarRegras(lista);

        Assert.Equal(StatusReembolso.SuspeitoDeFraude, lista.Last().Status);
    }

    // =========================
    // 🚨 EDGE CASE - IGUAL 1500
    // =========================
    [Fact]
    public void Nao_Deve_Marcar_Fraude_Se_Valor_For_Exatamente_1500()
    {
        var lista = new List<Reembolso>
        {
            Criar(TipoProcedimento.ExameImagem, 800, DateTime.Today.AddDays(-2), 1),
            Criar(TipoProcedimento.ExameImagem, 800, DateTime.Today.AddDays(-1), 1),
            Criar(TipoProcedimento.ExameImagem, 800, DateTime.Today, 1)
        };

        lista.ForEach(r => _calculator.Calcular(r));

        _fraudChecker.AplicarRegras(lista);

        Assert.Equal(StatusReembolso.Aprovado, lista.Last().Status);
    }

    // =========================
    // 🧱 HELPER
    // =========================
    private Reembolso Criar(
        TipoProcedimento tipo,
        decimal valor,
        DateTime? data = null,
        int clienteId = 1)
    {
        return new Reembolso
        {
            TipoProcedimento = tipo,
            ValorPago = valor,
            DataProcedimento = data ?? DateTime.Today,
            ClienteId = clienteId,
            Status = StatusReembolso.Aprovado
        };
    }

}
