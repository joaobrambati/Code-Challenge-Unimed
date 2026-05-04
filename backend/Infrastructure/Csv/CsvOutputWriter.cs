using Domain.Entities;
using Domain.Enums;
using System.Globalization;

namespace Infrastructure.Csv;

public class CsvOutputWriter
{
    public string Gerar(List<Reembolso> pedidos)
    {
        var header =
            $"{"Id",-3}| " +
            $"{"TipoProcedimento",-21}| " +
            $"{"DataProcedimento",-15} | " +
            $"{"ValorPago",-10} | " +
            $"{"ValorReembolsado",-18} | " +
            $"{"ClienteId",-10} | " +
            $"{"Status",-20}";

        var underscore = new string('-', header.Length);

        var linhas = pedidos.Select(p =>
            $"{p.Id,-3}| " +
            $"{MapearTipo(p.TipoProcedimento),-21}|  " +
            $"{p.DataProcedimento.ToString("yyyy-MM-dd"),-15} | " +
            $"{p.ValorPago.ToString("0.##", CultureInfo.InvariantCulture),-10} | " +
            $"{p.ValorReembolsado.ToString("0.##", CultureInfo.InvariantCulture),-18} | " +
            $"{p.ClienteId,-10} | " +
            $"{MapearStatus(p.Status),-20}"
        );

        return header + "\n" + underscore + "\n" + string.Join("\n", linhas);
    }

    private string MapearTipo(TipoProcedimento tipo)
    {
        return tipo switch
        {
            TipoProcedimento.ConsultaMedica => "Consulta Médica",
            TipoProcedimento.ExameImagem => "Exame de Imagem",
            TipoProcedimento.ExameLaboratorial => "Exame Laboratorial",
            _ => "Outros"
        };
    }

    private string MapearStatus(StatusReembolso status)
    {
        return status switch
        {
            StatusReembolso.Aprovado => "Aprovado",
            StatusReembolso.Rejeitado => "Rejeitado",
            StatusReembolso.SuspeitoDeFraude => "Suspeito de Fraude",
            _ => status.ToString()
        };
    }
}
