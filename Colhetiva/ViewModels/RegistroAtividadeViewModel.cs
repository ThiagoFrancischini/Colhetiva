using Colhetiva.Core.Entities;
using Colhetiva.DTOs;

namespace Colhetiva.ViewModels;

public class CalendarioMesViewModel
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public List<List<DateTime?>> Semanas { get; set; } = new();
    public Dictionary<DateTime, List<ItemCalendarioDto>> AtividadesPorDia { get; set; } = new();

    public DateTime MesAtual => new(Ano, Mes, 1);
    public DateTime MesAnterior => MesAtual.AddMonths(-1);
    public DateTime MesProximo => MesAtual.AddMonths(1);
}

public class RegistrarAtividadeViewModel
{
    public Horta Horta { get; set; } = null!;
    public Guid? CanteiroIdPreSelecionado { get; set; }
    public List<string> AtividadesSugeridas { get; set; } = new()
    {
        "Rega", "Poda", "Adubação", "Colheita", "Limpeza", "Manutenção de Cerca"
    };
}
