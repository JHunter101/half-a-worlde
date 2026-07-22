using Bunit;
using Core.Models;

namespace Specifications;

public sealed class BlazorTestContext : IDisposable
{
    private readonly IGameStateService _gameStateService;

    public BunitContext BunitContext => field ??= new BunitContext();
    public GameSettings? Settings => _gameStateService.Session?.Settings;

    public IRenderedComponent<Home> RenderedPage
        => field ??= BunitContext.Render<Home>();

    public bool InvalidTriggered { get; private set; }

    public bool SuccessTriggered { get; private set; }

    public BlazorTestContext(IGameStateService gameStateService)
    {
        _gameStateService = gameStateService;

        _gameStateService.OnSuccess += SetSuccessTriggered;
        _gameStateService.OnInvalid += SetInvalidTriggered;
    }

    public void ResetEvents()
    {
        InvalidTriggered = false;
        SuccessTriggered = false;
    }

    private void SetInvalidTriggered() => InvalidTriggered = true;

    private void SetSuccessTriggered()
    {
        SuccessTriggered = true;
        _gameStateService.NotifyAnimationComplete();
    }

    public void Dispose()
    {
        _gameStateService.OnSuccess -= SetSuccessTriggered;
        _gameStateService.OnInvalid -= SetInvalidTriggered;

        BunitContext.Dispose();
    }
}