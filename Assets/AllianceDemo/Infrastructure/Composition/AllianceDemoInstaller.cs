using AllianceDemo.Application.UseCases;
using AllianceDemo.Domain.Interfaces;
using AllianceDemo.Infrastructure.Services;
using Zenject;

namespace AllianceDemo.Infrastructure.Composition
{
    /// <summary>
    /// Scene-level DI container configuration.
    /// Attach to SceneContext to enable the architecture.
    /// </summary>
    public class AllianceDemoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindInfrastructureServices();
            BindUseCases();
        }

        private void BindInfrastructureServices()
        {
            Container.Bind<ILogService>().To<UnityLogService>().AsSingle();
            Container.Bind<IAllianceApiClient>().To<MockAllianceApiClient>().AsSingle();
        }

        private void BindUseCases()
        {
            Container.Bind<StartBattleUseCase>().AsTransient();
            Container.Bind<UseAbilityUseCase>().AsTransient();
            Container.Bind<CompleteBattleUseCase>().AsTransient();
        }
    }
}
