using System.Collections;
using AllianceDemo.Domain.Enums;
using AllianceDemo.Presentation.Gameplay;
using AllianceDemo.Presentation.UI;
using UnityEngine;

namespace AllianceDemo.Presentation.FTUE
{
    /// <summary>
    /// First-Time User Experience flow controller.
    /// Handles:
    /// - Intro dialog
    /// - Teaching ability button usage
    /// - Highlight guidance
    /// - Waiting for battle result
    ///
    /// UI-only. No gameplay logic inside.
    /// </summary>
    [DisallowMultipleComponent]
    public class FtueController : MonoBehaviour
    {
        #region Inspector

        [Header("Dialog Text")]
        [TextArea]
        [SerializeField] private string _introLine = "Welcome to the Alliance, hero!";

        [TextArea]
        [SerializeField] private string _useAbilityLine = "Tap your ability to strike the enemy!";

        [TextArea]
        [SerializeField] private string _victoryLine = "Victory! You're ready.";

        [Header("References")]
        [SerializeField] private AbilityButtonView _abilityButton;
        [SerializeField] private BattleController _battle;
        [SerializeField] private DialogView _dialog;
        [SerializeField] private FtueHighlightView _highlight;
        [SerializeField] private LevelUpPopupView _levelUpPopup;

        #endregion

        #region State

        private bool _abilityUsed;
        private bool _battleCompleted;
        private BattleResult _result;

        #endregion

        #region Public API

        /// <summary>
        /// Safe FTUE initialization. Must be called before <see cref="StartFtue"/>.
        /// Subscribes to battle events.
        /// </summary>
        public void Initialize()
        {
            if (_battle == null || _dialog == null || _highlight == null || _levelUpPopup == null)
            {
                Debug.LogError("[FTUE] Missing references. FTUE disabled.");
                enabled = false;
                return;
            }

            _battle.AbilityUsed += OnAbilityUsed;
            _battle.BattleCompleted += OnBattleCompleted;
        }

        /// <summary>
        /// Starts the guided onboarding flow.
        /// </summary>
        public void StartFtue()
        {
            if (!enabled)
                return;

            StartCoroutine(FtueFlow());
        }

        #endregion

        #region Unity Lifecycle

        private void OnDestroy()
        {
            if (_battle != null)
            {
                _battle.AbilityUsed -= OnAbilityUsed;
                _battle.BattleCompleted -= OnBattleCompleted;
            }
        }

        #endregion

        #region Event Handlers

        private void OnAbilityUsed()
        {
            _abilityUsed = true;
        }

        private void OnBattleCompleted(BattleResult result)
        {
            _battleCompleted = true;
            _result = result;
        }

        #endregion

        #region FTUE Flow

        /// <summary>
        /// Main FTUE steps.
        /// Can be later turned into a scriptable sequence if needed.
        /// </summary>
        private IEnumerator FtueFlow()
        {
            // 1) Intro line
            yield return ShowStep(_introLine);

            // 2) Explain ability usage
            yield return ShowStep(_useAbilityLine);

            // 3) Highlight ability button and wait for tap
            yield return HighlightAbilityAndWait();

            // 4) Wait until the battle is completed (Win/Lose)
            yield return WaitForBattle();

            // 5) OPTIONAL: final dialog + level-up popup
            // If you want it back, just uncomment:
            // yield return ShowVictoryFlow();
        }

        /// <summary>
        /// Shows a dialog step and waits for user to press continue.
        /// </summary>
        private IEnumerator ShowStep(string text)
        {
            bool done = false;
            _dialog.Show(text, () => done = true);
            while (!done)
                yield return null;
        }

        /// <summary>
        /// Highlights the ability button, enables it and waits for the player
        /// to press it at least once.
        /// </summary>
        private IEnumerator HighlightAbilityAndWait()
        {
            if (_abilityButton != null && _highlight != null)
            {
                _highlight.ShowOver(_abilityButton.RectTransform);
                _battle.EnableAbilityButton(true);
            }
            else
            {
                Debug.LogWarning("[FTUE] AbilityButton or Highlight is missing – cannot highlight.");
                _battle.EnableAbilityButton(true);
            }

            _abilityUsed = false;
            while (!_abilityUsed)
                yield return null;

            _highlight?.Hide();
        }

        /// <summary>
        /// Waits until the battle is completed (result provided by BattleController).
        /// </summary>
        private IEnumerator WaitForBattle()
        {
            _battleCompleted = false;
            while (!_battleCompleted)
                yield return null;
        }

        /// <summary>
        /// Optional victory step: final text + level-up popup.
        /// Currently not used to avoid overlapping with VictoryPopup.
        /// </summary>
        private IEnumerator ShowVictoryFlow()
        {
            if (_result == BattleResult.Win)
            {
                yield return ShowStep(_victoryLine);
                _levelUpPopup.Show("Level Up!");
            }
        }

        #endregion
    }
}
