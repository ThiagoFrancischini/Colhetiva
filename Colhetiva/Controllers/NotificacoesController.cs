using Colhetiva.Core.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace Colhetiva.Controllers;

public class NotificacoesController : Controller
{
    private readonly INotificacaoService _notificacaoService;

    public NotificacoesController(INotificacaoService notificacaoService)
    {
        _notificacaoService = notificacaoService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Faça login para ver suas notificações.";
            return RedirectToAction("Login", "Account");
        }
        var usuarioId = Guid.Parse(usuarioIdStr);

        var notificacoes = await _notificacaoService.GetNotificacoesAsync(usuarioId);
        return View(notificacoes);
    }

    [HttpGet]
    public async Task<IActionResult> MarcarELida(Guid id)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Faça login para ver suas notificações.";
            return RedirectToAction("Login", "Account");
        }
        var usuarioId = Guid.Parse(usuarioIdStr);

        var notificacao = await _notificacaoService.MarcarComoLidaAsync(id, usuarioId);
        if (notificacao == null)
            return RedirectToAction("Index");

        return RedirectToAction("Details", "Horta", new { id = notificacao.Aviso.HortaId });
    }
}
