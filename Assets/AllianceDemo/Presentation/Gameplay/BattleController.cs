using System;
using AllianceDemo.Application.UseCases;
using AllianceDemo.Domain.Entities;
using AllianceDemo.Domain.Enums;
using AllianceDemo.Presentation.FTUE;
using AllianceDemo.Presentation.UI;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace AllianceDemo.Presentation.Gameplay
{
    /// <summary>
    /// Coordinates a small battle scenario:
    /// - Hero vs Enemy
    /// - Ability-based attack
    /// - FTUE-guided first battle
    /// - Simple victory flow with restart
    ///
    /// Responsibilities (presentation layer only):
    /// - Holds references to domain objects (Hero, Enemy)
    /// - Wires domain use cases to Unity views
    /// - Exposes high-level events for FTUE and other listeners
    /// </summary>
    public class BattleController : MonoBehaviour
    {
        #region Inspector

        [Header("Views")]
        [SerializeField] private HeroView _heroView;
        [SerializeField] private EnemyView _enemyView;
        [SerializeField] private HealthBarView _heroHealthBar;
        [SerializeField] private HealthBarView _enemyHealthBar;
        [SerializeField] private AbilityButtonView _abilityButton;
        [SerializeField] private LevelUpPopupView _levelUpPopup;
        [SerializeField] private VictoryPopupView _victoryPopup;
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

        #endregion

        #region Events

        /// <summary>
        /// Raised when the player presses the ability button.
        /// Used by FTUE to know that the player followed instructions.
        /// </summary>
        public event Action AbilityUsed;

        /// <summary>
        /// Raised when the battle has a final result (Win/Lose).
        /// FTUE can subscribe to wait for battle completion.
        /// </summary>
        public event Action<BattleResult> BattleCompleted;

        #endregion

        #region Properties

        public Hero Hero { get; private set; }
        public Enemy Enemy { get; private set; }

        #endregion

        #region Dependencies

        private StartBattleUseCase _startBattleUseCase;
        private UseAbilityUseCase _useAbilityUseCase;
        private CompleteBattleUseCase _completeBattleUseCase;

        #endregion

        private bool _battleFinished;

        #region DI

        [Inject]
        public void Construct(
            StartBattleUseCase startBattleUseCase,
            UseAbilityUseCase useAbilityUseCase,
            CompleteBattleUseCase completeBattleUseCase)
        {
            _startBattleUseCase = startBattleUseCase;
            _useAbilityUseCase = useAbilityUseCase;
            _completeBattleUseCase = completeBattleUseCase;
        }

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            CreateDomainModels();
            BindViewsAndUi();
            InitializeBattle();
            SetupFtue();
        }

        private void OnDestroy()
        {
            if (_abilityButton != null)
                _abilityButton.Clicked -= OnAbilityButtonClicked;

            if (_victoryPopup != null)
                _victoryPopup.Continued -= OnVictoryContinuePressed;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Creates new domain instances for hero and enemy.
        /// Called on first start and when resetting the battle.
        /// </summary>
        private void CreateDomainModels()
        {
            Hero = new Hero(
                id: _heroId,
                name: _heroName,
                level: 1,
                maxHealth: _heroMaxHealth,
                abilityDamage: _heroAbilityDamage);

            Enemy = new Enemy(
                id: _enemyId,
                name: _enemyName,
                maxHealth: _enemyMaxHealth);
        }

        /// <summary>
        /// One-time binding between domain models, UI views and input.
        /// Event subscriptions live here and are cleaned up in OnDestroy.
        /// </summary>
        private void BindViewsAndUi()
        {
            // Bind hero & enemy views to domain entities
            _heroView.Bind(Hero);
            _enemyView.Bind(Enemy);

            // Bind health bars to current hero/enemy
            _heroHealthBar.Bind(() => Hero.Health, () => Hero.MaxHealth);
            _enemyHealthBar.Bind(() => Enemy.Health, () => Enemy.MaxHealth);

            _heroHealthBar.RefreshImmediate();
            _enemyHealthBar.RefreshImmediate();

            // Ability button is initially hidden; FTUE (or free play) will reveal it
            _abilityButton.HideImmediate();
            _abilityButton.Clicked += OnAbilityButtonClicked;

            // Victory popup will restart the battle
            if (_victoryPopup != null)
                _victoryPopup.Continued += OnVictoryContinuePressed;
        }

        /// <summary>
        /// Initializes the battle state (HP, animations) using the use case.
        /// </summary>
        private void InitializeBattle()
        {
            _battleFinished = false;

            _startBattleUseCase.Initialize(Hero, Enemy);

            _heroHealthBar.RefreshImmediate();
            _enemyHealthBar.RefreshImmediate();

            _heroView.PlayIdle();
            _enemyView.PlayIdle();
        }

        /// <summary>
        /// Starts the FTUE flow if a controller is assigned.
        /// </summary>
        private void SetupFtue()
        {
            if (_ftueController == null)
                return;

            // Текущая версия FtueController ожидает внутренние ссылки через инспектор,
            // поэтому вызываем параметрless Initialize().
            _ftueController.Initialize();
            _ftueController.StartFtue();
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Called by <see cref="AbilityButtonView"/> when the player presses the ability button.
        /// Orchestrates attack animation, damage application and battle completion.
        /// </summary>
        private void OnAbilityButtonClicked()
        {
            if (_battleFinished)
                return;

            if (!Enemy.IsAlive)
                return;

            PlayHeroAttackFeedback();

            // Apply damage on the domain side
            bool enemyDied = _useAbilityUseCase.Execute(Hero, Enemy);

            // Enemy visual reaction
            PlayEnemyReaction(enemyDied);

            // Update UI
            _enemyHealthBar.AnimateToValue();

            // Notify FTUE that the player used the ability
            AbilityUsed?.Invoke();

            if (enemyDied)
            {
                HandleBattleFinished();
            }
        }

        #endregion

        #region Battle Flow

        /// <summary>
        /// Plays hero attack animation + small squash & stretch.
        /// Purely visual, does not modify domain state.
        /// </summary>
        private void PlayHeroAttackFeedback()
        {
            if (_heroView == null)
                return;

            _heroView.PlayAttack();

            Transform t = _heroView.transform;
            t.DOKill();
            t.localScale = Vector3.one;

            t.DOPunchScale(
                punch: new Vector3(0.1f, 0.1f, 0f),
                duration: 0.2f,
                vibrato: 10,
                elasticity: 1f);
        }

        /// <summary>
        /// Plays hit or death animation on the enemy.
        /// </summary>
        private void PlayEnemyReaction(bool enemyDied)
        {
            if (_enemyView == null)
                return;

            if (enemyDied)
                _enemyView.PlayDie();
            else
                _enemyView.PlayHit();
        }

        /// <summary>
        /// Finalizes the battle: completes use case, logs result,
        /// hides ability button, shows victory popup and notifies listeners.
        /// </summary>
        private void HandleBattleFinished()
        {
            BattleResult result = _completeBattleUseCase.Execute(Hero, Enemy);
            _battleFinished = true;

            _heroHealthBar.AnimateToValue();
            _enemyHealthBar.AnimateToValue();

            // Hide ability button while victory popup is on screen
            EnableAbilityButton(false);

            BattleCompleted?.Invoke(result);

            if (_victoryPopup != null)
                _victoryPopup.Show(result);
        }

        /// <summary>
        /// Public API used by FTUE to toggle ability availability
        /// and apply visual attention (pulse) when enabled.
        /// </summary>
        public void EnableAbilityButton(bool enable)
        {
            if (_abilityButton == null)
                return;

            if (!enable)
            {
                _abilityButton.HideImmediate();

                RectTransform rtOff = _abilityButton.RectTransform;
                if (rtOff != null)
                {
                    rtOff.DOKill();
                    rtOff.localScale = Vector3.one;
                }

                return;
            }

            // Smooth fade-in reveal
            _abilityButton.RevealWithFade();

            // Pulse animation to draw attention
            RectTransform rt = _abilityButton.RectTransform;
            if (rt != null)
            {
                rt.DOKill();
                rt.localScale = Vector3.one;

                rt.DOPunchScale(
                        punch: new Vector3(0.1f, 0.1f, 0f),
                        duration: 0.5f,
                        vibrato: 8,
                        elasticity: 0.7f)
                  .SetLoops(-1, LoopType.Yoyo);
            }
        }

        /// <summary>
        /// Fully resets the battle to a fresh state:
        /// - creates new Hero/Enemy domain objects
        /// - re-binds UI delegates
        /// - initializes HP via use case
        /// - hides ability button until FTUE / free play decides otherwise
        /// </summary>
        public void ResetBattle()
        {
            _battleFinished = false;

            CreateDomainModels();

            // Re-bind domain to views and health bars
            _heroView.Bind(Hero);
            _enemyView.Bind(Enemy);

            _heroHealthBar.Bind(() => Hero.Health, () => Hero.MaxHealth);
            _enemyHealthBar.Bind(() => Enemy.Health, () => Enemy.MaxHealth);

            _heroHealthBar.RefreshImmediate();
            _enemyHealthBar.RefreshImmediate();

            InitializeBattle();

            // After reset we keep ability hidden; caller decides when to show.
            EnableAbilityButton(false);

            Debug.Log("[Battle] Reset complete — ready for new session.");
        }

        #endregion

        #region Victory Flow

        /// <summary>
        /// Called by <see cref="VictoryPopupView"/> when the player presses "Continue".
        /// Hides the popup, resets the battle and enables the ability button again.
        /// </summary>
        private void OnVictoryContinuePressed()
        {
            if (_victoryPopup != null)
                _victoryPopup.Hide();

            ResetBattle();

            // For now we go into "free play" mode after first battle.
            // If you want FTUE again – call EnableAbilityButton(false) and restart FTUE here.
            EnableAbilityButton(true);
        }

        #endregion
    }
}
