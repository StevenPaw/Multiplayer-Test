using MHLab.Patch.Admin.Editor.Components.Contents;
using MHLab.Patch.Admin.Editor.EditorHelpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MHLab.Patch.Admin.Editor.Components
{
    public class PatchTipPopup : Widget
    {
        private bool _shouldBeRendered = false;

        private GUIStyle _style;
        private Rect _backdropArea;
        private Vector2 _backdropSize;

        private Rect _titleArea;
        private Rect _textArea;

        private Rect _closeButton;
        private Vector2 _closeButtonSize;
        private Rect _moreButton;
        private Vector2 _moreButtonSize;

        private Vector2 _previousHostSize;

        private string _phrase;
        private int _currentPhraseIndex;
        
        private static readonly string[] Phrases = new string[]
        {
            string.Format("<color={0}>You should write a review on Asset Store to help other customers!</color>", ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor)),
            string.Format("<color={0}>Doc contains a lot of useful information! Read it, make me proud of you!</color>", ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor)),
            string.Format("<color={0}>Did I already say you should read the doc one more time? :)</color>", ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor)),
            string.Format("<color={0}>You can join our Discord server to chat with us!</color>", ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor)),
            string.Format("<color={0}>You can send me a mail to ask for help! But read the doc first :)</color>", ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor)),
            string.Format("<color={0}>You can use PATCH not only with Unity! UE, CE, MonoGame, etc: everything works with PATCH!</color>", ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor)),
            string.Format("<color={0}>You can sell your own PATCH skins and plugins on Asset Store!</color>", ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor)),
            string.Format("<color={0}>You must set .NET Standard 2.0 as your API Compatibility Level!</color>", ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor)),
            string.Format("<color={0}>It is a good practice to run PATCH Launcher as admin!</color>", ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor)),
            string.Format("<color={0}>You must not run your Launcher in the Editor!</color>", ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor)),
        };

        public override void Initialize()
        {
            base.Initialize();
            Size = new Vector2(Host.Width, Host.Height);
            _previousHostSize = Host.Size;

            _shouldBeRendered = true;
            if (PlayerPrefs.HasKey(PatchOptionsContent.TipsEnabledKey))
                _shouldBeRendered = PlayerPrefs.GetInt(PatchOptionsContent.TipsEnabledKey) == 1;
            _backdropSize = new Vector2(250, 130);
            _closeButtonSize = new Vector2(20, 20);
            _moreButtonSize = new Vector2(20, 20);

            //_backdropArea = new Rect(Width - _backdropSize.x - 10, Height - _backdropSize.y - 10, _backdropSize.x, _backdropSize.y);
            _backdropArea = new Rect(10, Height - _backdropSize.y - 10, _backdropSize.x, _backdropSize.y);

            _titleArea = new Rect(_backdropArea.x + 10, _backdropArea.y + 10, _backdropArea.width - 20, 15);
            _textArea = new Rect(_backdropArea.x + 10, _backdropArea.y + 15, _backdropArea.width - 20, _backdropArea.height - 25);

            _closeButton = new Rect(_backdropArea.x + _backdropArea.width - 10 - _closeButtonSize.x, _backdropArea.y + 10, _closeButtonSize.x, _closeButtonSize.y);
            _moreButton = new Rect(_backdropArea.x + _backdropArea.width - 10 - _moreButtonSize.x, _backdropArea.y + _backdropArea.height - _moreButtonSize.y - 10, _moreButtonSize.x, _moreButtonSize.y);

            _style = new GUIStyle();
            _style.wordWrap = true;
            _style.fontSize = 12;
            _style.richText = true;
            _style.alignment = TextAnchor.MiddleCenter;

            _phrase = GetRandomPhrase();
        }

        public override void Render()
        {
            if (_shouldBeRendered)
            {
                base.Render();

                if (_previousHostSize != Host.Size)
                {
                    Size = new Vector2(Host.Width, Host.Height);
                    //_backdropArea = new Rect(Width - _backdropSize.x - 10, Height - _backdropSize.y - 10, _backdropSize.x, _backdropSize.y);
                    _backdropArea = new Rect(10, Height - _backdropSize.y - 10, _backdropSize.x, _backdropSize.y);

                    _titleArea = new Rect(_backdropArea.x + 10, _backdropArea.y + 10, _backdropArea.width - 20, 15);
                    _textArea = new Rect(_backdropArea.x + 10, _backdropArea.y + 15, _backdropArea.width - 20, _backdropArea.height - 25);

                    _closeButton = new Rect(_backdropArea.x + _backdropArea.width - 10 - _closeButtonSize.x, _backdropArea.y + 10, _closeButtonSize.x, _closeButtonSize.y);
                    _moreButton = new Rect(_backdropArea.x + _backdropArea.width - 10 - _moreButtonSize.x, _backdropArea.y + _backdropArea.height - _moreButtonSize.y - 10, _moreButtonSize.x, _moreButtonSize.y);
                }

                var previous = GUI.skin;
                GUI.skin = Host.GetSkin(ThemeHelper.DarkColorName);

                GUI.Box(_backdropArea, "");

                GUI.Label(_titleArea, "<color=" + ThemeHelper.ConvertToStringFormat(ThemeHelper.TipTitleColor) + ">Did you know?</color>", _style);
                GUI.Label(_textArea, _phrase, _style);

                if (GUI.Button(_closeButton, "X"))
                {
                    _shouldBeRendered = false;
                }

                if (GUI.Button(_moreButton, ">"))
                {
                    _phrase = GetRandomPhrase();
                }

                GUI.skin = previous;
            }
        }

        private string GetRandomPhrase()
        {
            if (Phrases.Length == 0)
            {
                _shouldBeRendered = false;
                return "";
            }

            if (Phrases.Length == 1)
            {
                return Phrases[0];
            }
            else
            {
                int index;
                do
                {
                    index = (int)Random.Range(0, Phrases.Length);
                    if (index > Phrases.Length - 1) index = Phrases.Length - 1;
                } while (_currentPhraseIndex == index);

                _currentPhraseIndex = index;
                return Phrases[index];
            }
        }
    }
}
