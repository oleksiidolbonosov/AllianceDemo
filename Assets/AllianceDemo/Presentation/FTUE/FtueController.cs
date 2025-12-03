using System.Collections;
using AllianceDemo.Domain.Enums;
using AllianceDemo.Presentation.Gameplay;
using AllianceDemo.Presentation.UI;
using UnityEngine;

namespace AllianceDemo.Presentation.FTUE
{
    /// <summary>
    /// Very small FTUE flow:
    /// 1) Intro line
    /// 2) Explain ability
    /// 3) Highlight ability button and wait for use
    /// 4) Wait for battle completion
    /// 5) Victory line + level-up popup
    /// </summary>
    public class FtueController : MonoBehaviour
    {
        [TextArea]
        [SerializeField] private string _introLine = "Welcome to the Alliance, hero!";
        [TextArea]
        [SerializeField] private string _useAbilityLine = "Tap your ability to strike the enemy!";
        [TextArea]
        [SerializeField] private string _victoryLine = "Victory! You're ready to join the Alliance.";

        private BattleController _battle;
        private DialogView _dialog;
        private FtueHighlightView _highlight;
        private LevelUpPopupView _levelUpPopup;

        private bool _abilityUsed;
        private bool _battleCompleted;
        private BattleResult _result;

        public void Initialize(
            BattleController battle,
            DialogView dialogView,
            FtueHighlightView highlightView,
            LevelUpPopupView levelUpPopup)
        {
            _battle = battle;
            _dialog = dialogView;
            _highlight = highlightView;
            _levelUpPopup = levelUpPopup;

            _battle.AbilityUsed += OnAbilityUsed;
            _battle.BattleCompleted += OnBattleCompleted;
        }

        private void OnDestroy()
        {
            if (_battle != null)
            {
                _battle.AbilityUsed -= OnAbilityUsed;
                _battle.BattleCompleted -= OnBattleCompleted;
            }
        }

        public void StartFtue()
        {
            StartCoroutine(FtueFlow());
        }

        private void OnAbilityUsed()
        {
            _abilityUsed = true;
        }

        private void OnBattleCompleted(BattleResult result)
        {
            _battleCompleted = true;
            _result = result;
        }

        private IEnumerator FtueFlow()
        {
            // 1) Intro
            bool stepDone = false;
            _dialog.Show(_introLine, () => stepDone = true);
            while (!stepDone) yield return null;

            // 2) Explain ability
            stepDone = false;
            _dialog.Show(_useAbilityLine, () => stepDone = true);
            while (!stepDone) yield return null;

            // 3) Highlight ability & enable it
            var abilityView = _battle.GetComponentInChildren<AbilityButtonView>();
            var abilityRect = abilityView != null ? abilityView.RectTransform : null;
            if (abilityRect != null)
            {
                _highlight.ShowOver(abilityRect);
            }

            _battle.EnableAbilityButton(true);
            _abilityUsed = false;

            while (!_abilityUsed)
                yield return null;

            _highlight.Hide();

            // 4) Wait for battle completion
            _battleCompleted = false;
            while (!_battleCompleted)
                yield return null;

            if (_result == BattleResult.Win)
            {
                stepDone = false;
                _dialog.Show(_victoryLine, () => stepDone = true);
                while (!stepDone) yield return null;

                _levelUpPopup.Show("Level Up!");
            }
        }
    }
}
