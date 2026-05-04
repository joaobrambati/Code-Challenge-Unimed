using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Csv;

public class CsvInputReader
{
    public List<Reembolso> Ler(string input)
    {
        var linhas = input.Split('\n').Skip(1);

        return linhas
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var colunas = l.Split(',');

                return new Reembolso
                {
                    Id = int.Parse(colunas[0]),
                    TipoProcedimento = MapearTipo(colunas[1]),
                    DataProcedimento = DateTime.Parse(colunas[2]),
                    ValorPago = decimal.Parse(colunas[3]),
                    ClienteId = int.Parse(colunas[4])
                };
            }).ToList();
    }

    private TipoProcedimento MapearTipo(string tipo)
    {
        var valor = tipo.Trim().ToLower();

        if (valor.Contains("consulta"))
            return TipoProcedimento.ConsultaMedica;

        if (valor.Contains("imagem"))
            return TipoProcedimento.ExameImagem;

        if (valor.Contains("laboratorial"))
            return TipoProcedimento.ExameLaboratorial;

        return TipoProcedimento.Outros;
    }
}
