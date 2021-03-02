using System;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.Components.Contents
{
    public class PatchOptionsContent : PatchContent
    {
        public const string NoobModeKey = "NoobModeEnabled";
        public const string TipsEnabledKey = "TipsEnabled";

        private Vector2 _scrollPosition;

        private float _margin = 4f;

        private Rect _noobModeDescriptionArea;
        private Rect _noobModeArea;
        private bool _isNoobMode;
        private bool _previousIsNoobMode;

        private Rect _disableTipsDescriptionArea;
        private Rect _disableTipsArea;
        private bool _isTipEnabled;
        private bool _previousIsTipEnabled;

        private Rect _tutorialDescriptionArea;
        private Rect _tutorialArea;

        public override void Initialize()
        {
            base.Initialize();

            if (PlayerPrefs.HasKey(NoobModeKey))
            {
                _isNoobMode = PlayerPrefs.GetInt(NoobModeKey) == 1;
                _previousIsNoobMode = _isNoobMode;
            }
            else
            {
                _isNoobMode = false;
                _previousIsNoobMode = false;
            }

            if (PlayerPrefs.HasKey(TipsEnabledKey))
            {
                _isTipEnabled = PlayerPrefs.GetInt(TipsEnabledKey) == 1;
                _previousIsTipEnabled = _isNoobMode;
            }
            else
            {
                _isTipEnabled = true;
                _previousIsTipEnabled = true;
            }
        }

        public override void UpdateUISize()
        {
            base.UpdateUISize();
            var height = ContentArea.position.y;

            _noobModeDescriptionArea = new Rect(ContentArea.position.x, height, ContentArea.width * 11 / 12, 40);
            _noobModeArea = new Rect(ContentArea.position.x + ContentArea.width * 11 / 12, height, ContentArea.width * 1 / 12, 30);

            height += 40 + _margin;

            _disableTipsDescriptionArea = new Rect(ContentArea.position.x, height, ContentArea.width * 11 / 12, 40);
            _disableTipsArea = new Rect(ContentArea.position.x + ContentArea.width * 11 / 12, height, ContentArea.width * 1 / 12, 30);

            height += 40 + _margin;

            _tutorialDescriptionArea = new Rect(ContentArea.position.x, height, ContentArea.width * 10 / 12, 40);
            _tutorialArea = new Rect(ContentArea.position.x + ContentArea.width * 10 / 12, height, ContentArea.width * 2 / 12, 30);
        }

        public override void Render()
        {
            base.Render();
            _scrollPosition = GUI.BeginScrollView(ContentArea, _scrollPosition, ContentArea);

            GUI.Label(_noobModeDescriptionArea, "Beginner mode (coming soon)<i><size=9><color=#808080ff> - Allows you to simplify the process of building patches, etc by automatizing some tasks.</color></size></i>");
            _isNoobMode = GUI.Toggle(_noobModeArea, _isNoobMode, "");
            if (_isNoobMode != _previousIsNoobMode)
            {
                PlayerPrefs.SetInt(NoobModeKey, (_isNoobMode) ? 1 : 0);
                _previousIsNoobMode = _isNoobMode;
            }

            GUI.Label(_disableTipsDescriptionArea, "Enable tips<i><size=9><color=#808080ff> - Allows you to see some tips on Admin Tool startup.</color></size></i>");
            _isTipEnabled = GUI.Toggle(_disableTipsArea, _isTipEnabled, "");
            if (_isTipEnabled != _previousIsTipEnabled)
            {
                PlayerPrefs.SetInt(TipsEnabledKey, (_isTipEnabled) ? 1 : 0);
                _previousIsTipEnabled = _isTipEnabled;
            }

            GUI.Label(_tutorialDescriptionArea, "Tutorial<i><size=9><color=#808080ff> - Press this button to play again the tutorial! I know you love it! :-)</color></size></i>");
            if (GUI.Button(_tutorialArea, "Play it!"))
            {
                ThemeHelper.ToggleHasBeenOpened(false);
                AdminWindow.SetContainerComponents(Host);
            }

            GUI.EndScrollView();
        }
    }
}
