using System.Diagnostics;
using MHLab.Patch.Admin.Editor.EditorHelpers;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.Components
{
    public class PatchPopup : Widget
    {
        private const string PatchPopupKeyName = "PatchPopupOpeningsAmount";
        private const string PatchPopupFlag = "PatchPopupDontAskAgain";
        private const int PatchPopupOpeningsAmount = 10;

        private const string PatchReviewUrl = "https://assetstore.unity.com/packages/tools/user-tools/utilities/p-a-t-c-h-ultimate-patching-system-41417";//"https://www.assetstore.unity3d.com/en/#!/account/downloads/search=#PACKAGES%20PATCH%20ultimate";

        private bool _shouldBeRendered = false;

        private GUIStyle _style;
        private Rect _backdropArea;

        private Texture2D _logo;
        private Vector2 _logoSize;
        private Rect _logoArea;

        private Rect _textArea;

        private Rect _letsgoButtonArea;
        private Rect _remindMeButtonArea;
        private Rect _dontAskAgainButtonArea;

        private Vector2 _previousHostSize;

        public override void Initialize()
        {
            base.Initialize();
            Size = new Vector2(Host.Width, Host.Height);
            _previousHostSize = Host.Size;

            CheckForPopupOpenings();

            if (_shouldBeRendered)
            {
                _style = new GUIStyle();
                _style.wordWrap = true;
                _style.fontSize = 15;
                _style.richText = true;
                _style.alignment = TextAnchor.MiddleCenter;

                _backdropArea = new Rect(0, 0, Width, Height);

                _logo = Resources.Load<Texture2D>("Images/logo_editor");
                _logoSize = new Vector2(250, 220);
                _logoArea = new Rect((Width / 2) - (_logoSize.x / 2), (Height / 2) - 230, _logoSize.x, _logoSize.y);

                _textArea = new Rect((Width / 2) - (200), (Height / 2) - (30), 400, 60);

                _letsgoButtonArea = new Rect((Width / 2) - 250, (Height / 2) + 85, 150, 30);
                _remindMeButtonArea = new Rect((Width / 2) - 75, (Height / 2) + 85, 150, 30);
                _dontAskAgainButtonArea = new Rect((Width / 2) + 100, (Height / 2) + 85, 150, 30);
            }
        }

        public override void Render()
        {
            if (_shouldBeRendered)
            {
                base.Render();

                if (_previousHostSize != Host.Size)
                {
                    Size = new Vector2(Host.Width, Host.Height);
                    _backdropArea = new Rect(0, 0, Width, Height);
                    _logoArea = new Rect((Width / 2) - (_logoSize.x / 2), (Height / 2) - 230, _logoSize.x, _logoSize.y);

                    _textArea = new Rect((Width / 2) - (200), (Height / 2) - (30), 400, 60);

                    _letsgoButtonArea = new Rect((Width / 2) - 250, (Height / 2) + 85, 150, 30);
                    _remindMeButtonArea = new Rect((Width / 2) - 75, (Height / 2) + 85, 150, 30);
                    _dontAskAgainButtonArea = new Rect((Width / 2) + 100, (Height / 2) + 85, 150, 30);
                }

                var previous = GUI.skin;
                GUI.skin = Host.GetSkin(ThemeHelper.PopupColorName);

                GUI.Box(_backdropArea, "");

                GUI.DrawTexture(_logoArea, _logo, ScaleMode.ScaleToFit);

                GUI.Label(_textArea, "<color=" + ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor) + ">Hey! If you love P.A.T.C.H. help us! Leave a review on Asset Store!</color>", _style);

                if (GUI.Button(_letsgoButtonArea, "Let's go!"))
                {
                    Process.Start(PatchReviewUrl);
                    _shouldBeRendered = false;
                }

                if (GUI.Button(_remindMeButtonArea, "Remind me later!"))
                {
                    _shouldBeRendered = false;
                }

                if (GUI.Button(_dontAskAgainButtonArea, "I did it!"))
                {
                    PlayerPrefs.SetInt(PatchPopupFlag, 1);
                    _shouldBeRendered = false;
                }

                GUI.skin = previous;
            }
        }

        private void CheckForPopupOpenings()
        {
            if (PlayerPrefs.HasKey(PatchPopupFlag))
            {
                _shouldBeRendered = false;
                return;
            }

            int amount = 1;
            if (PlayerPrefs.HasKey(PatchPopupKeyName))
            {
                amount = PlayerPrefs.GetInt(PatchPopupKeyName);
                if (amount >= PatchPopupOpeningsAmount)
                    amount = 1;
            }
            else
            {
                PlayerPrefs.SetInt(PatchPopupKeyName, 1);
            }

            if (amount == 1)
            {
                _shouldBeRendered = true;
            }

            amount = amount + 1;
            PlayerPrefs.SetInt(PatchPopupKeyName, amount);
        }
    }
}
