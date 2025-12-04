using System;
using AllianceDemo.Domain.Entities;
using AllianceDemo.Domain.Enums;
using AllianceDemo.Application.UseCases;
using AllianceDemo.Presentation.UI;
using AllianceDemo.Presentation.FTUE;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace AllianceDemo.Presentation.Gameplay
{
    /// <summary>
    /// Orchestrates a very small battle scenario:
    /// hero vs enemy + ability usage + FTUE and UI hooks.
    /// </summary>
    public class BattleController : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private HeroView _heroView;
        [SerializeField] private EnemyView _enemyView;
        [SerializeField] private HealthBarView _heroHealthBar;
        [SerializeField] private HealthBarView _enemyHealthBar;
        [SerializeField] private AbilityButtonView _abilityButton;
        [SerializeField] private LevelUpPopupView _levelUpPopup;
        [SerializeField] private DialogView _dialogView;
        [SerializeField] private FtueHighlightView _highlightView;
        [SerializeField] private FtueController _ftueController;

        [Header("Hero Settings")]
        [SerializeField] private string _heroId = "hero-1";
        [SerializeField] private string _heroName = "Alliance Hero";
        [SerializeField] private int _heroMaxHealth = 100;
        [SerializeField] private int _heroAbilityDamage = 40;

        [Header("Enemy Settings")]
        [SerializeField] private string _enemyId = "enemy-1";
        [SerializeField] private string _enemyName = "Dark Minion";
        [SerializeField] private int _enemyMaxHealth = 100;

        public event Action AbilityUsed;
        public event Action<BattleResult> BattleCompleted;

        public Hero Hero { get; private set; }
        public Enemy Enemy { get; private set; }

        private StartBattleUseCase _startBattle;
        private UseAbilityUseCase _useAbility;
        private CompleteBattleUseCase _completeBattle;

        private bool _battleFinished;

        [Inject]
        public void Construct(
            StartBattleUseCase startBattle,
            UseAbilityUseCase useAbility,
            CompleteBattleUseCase completeBattle)
        {
            _startBattle = startBattle;
            _useAbility = useAbility;
            _completeBattle = completeBattle;
        }

        private void Start()
        {
            InitializeDomainModels();
            BindViews();
            SetupBattle();
            SetupFtue();
        }

        private void InitializeDomainModels()
        {
            Hero = new Hero(_heroId, _heroName, level: 1, maxHealth: _heroMaxHealth, abilityDamage: _heroAbilityDamage);
            Enemy = new Enemy(_enemyId, _enemyName, maxHealth: _enemyMaxHealth);
        }

        private void BindViews()
        {
            _heroView.Bind(Hero);
            _enemyView.Bind(Enemy);

            _heroHealthBar.Bind(() => Hero.Health, () => Hero.MaxHealth);
            _enemyHealthBar.Bind(() => Enemy.Health, () => Enemy.MaxHealth);

            _heroHealthBar.RefreshImmediate();
            _enemyHealthBar.RefreshImmediate();

            _abilityButton.SetInteractable(false); // FTUE will enable it
            _abilityButton.OnClicked += OnAbilityButtonClicked;
        }

        private void SetupBattle()
        {
            _startBattle.Initialize(Hero, Enemy);
            _heroHealthBar.AnimateRefresh();
            _enemyHealthBar.AnimateRefresh();
        }

        private void SetupFtue()
        {
            if (_ftueController != null)
            {
                _ftueController.Initialize(this, _dialogView, _highlightView, _levelUpPopup);
                _ftueController.StartFtue();
            }
        }

        private void OnDestroy()
        {
            _abilityButton.OnClicked -= OnAbilityButtonClicked;
        }

        private void OnAbilityButtonClicked()
        {
            if (_battleFinished) return;
            if (!Enemy.IsAlive) return;

            // 1) Play hero attack animation + small squash & stretch
            if (_heroView != null)
            {
                _heroView.PlayAttack();

                var t = _heroView.transform;
                t.DOKill();
                t.localScale = Vector3.one;
                t.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.2f, 10, 1f);
            }

            // 2) Apply damage in domain
            var enemyDied = _useAbility.Execute(Hero, Enemy);

            // 3) Enemy feedback
            if (_enemyView != null)
            {
                if (enemyDied)
                {
                    _enemyView.PlayDie();
                }
                else
                {
                    _enemyView.PlayHit();
                }
            }

            // 4) Update UI
            _enemyHealthBar.AnimateRefresh();

            AbilityUsed?.Invoke();

            // 5) Complete battle if needed
            if (enemyDied)
            {
                var result = _completeBattle.Execute(Hero, Enemy);
                _battleFinished = true;

                _heroHealthBar.AnimateRefresh();
                BattleCompleted?.Invoke(result);
            }
        }


        public void EnableAbilityButton(bool enable)
        {
            _abilityButton.SetInteractable(enable);

            if (enable)
            {
                // Simple DOTween pulse to draw attention
                var rt = _abilityButton.RectTransform;
                if (rt != null)
                {
                    rt.DOKill();
                    rt.localScale = Vector3.one;
                    rt.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.5f, 8, 0.7f)
                      .SetLoops(-1, LoopType.Yoyo);
                }
            }
            else
            {
                var rt = _abilityButton.RectTransform;
                rt?.DOKill();
                if (rt != null)
                    rt.localScale = Vector3.one;
            }
        }
    }
}
