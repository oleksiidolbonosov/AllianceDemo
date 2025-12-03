using AllianceDemo.Application.UseCases;
using AllianceDemo.Domain.Interfaces;
using AllianceDemo.Infrastructure.Services;
using Zenject;

namespace AllianceDemo.Infrastructure.Composition
{
    /// <summary>
    /// Zenject installer responsible for binding services and use cases.
    /// Attach this to a SceneContext or ProjectContext in the demo scene.
    /// </summary>
    public class AllianceDemoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Infrastructure singletons
            Container.Bind<ILogService>().To<UnityLogService>().AsSingle();
            Container.Bind<IAllianceApiClient>().To<MockAllianceApiClient>().AsSingle();

            // Use cases
            Container.Bind<StartBattleUseCase>().AsTransient();
            Container.Bind<UseAbilityUseCase>().AsTransient();
            Container.Bind<CompleteBattleUseCase>().AsTransient();
        }
    }
}
