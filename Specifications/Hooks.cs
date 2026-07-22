using Bunit;
using Reqnroll.BoDi;

namespace Specifications;

[Binding]
public sealed class Hooks(IObjectContainer container)
{
    [BeforeScenario]
    public void BeforeScenario()
    {
        var wordRepository = new WordRepositoryStub();

        container.RegisterInstanceAs<IWordRepository>(wordRepository);
        container.RegisterInstanceAs<WordRepositoryStub>(wordRepository);

        container.RegisterTypeAs<GuessValidator, IGuessValidator>();
        container.RegisterTypeAs<WordGenerator, IWordGenerator>();
        container.RegisterTypeAs<GameStateService, IGameStateService>();

        IWordRepository wordRepositoryService = container.Resolve<IWordRepository>();
        IGuessValidator guessValidator = container.Resolve<IGuessValidator>();
        IWordGenerator wordGenerator = container.Resolve<IWordGenerator>();
        IGameStateService gameStateService = container.Resolve<IGameStateService>();

        var blazorTestContext = new BlazorTestContext(gameStateService);

        BunitServiceProvider services = blazorTestContext.BunitContext.Services;

        services.AddSingleton(wordRepositoryService);
        services.AddSingleton(guessValidator);
        services.AddSingleton(wordGenerator);
        services.AddSingleton(gameStateService);

        container.RegisterInstanceAs(blazorTestContext);
    }

    [AfterScenario]
    public void AfterScenario(BlazorTestContext context)
        => context.Dispose();
}