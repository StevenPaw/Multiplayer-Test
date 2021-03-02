using MHLab.PATCH.Admin.Editor;
using MHLab.Patch.Admin.Editor.EditorHelpers;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.Components
{
    public class PatchErrorPopup : Widget
    {
        private bool _shouldBeRendered = false;

        private GUIStyle _style;
        private Rect _backdropArea;

        private Texture2D _logo;
        private Vector2 _logoSize;
        private Rect _logoArea;

        private Rect _textArea;

        private Vector2 _previousHostSize;

        private string _errorText;
        private PopupErrorType _errorType;

        public override void Initialize()
        {
            base.Initialize();
            Size = new Vector2(Host.Width, Host.Height);
            _previousHostSize = Host.Size;

            CheckForPopupOpening();

            if (_shouldBeRendered)
            {
                _style = new GUIStyle();
                _style.wordWrap = true;
                _style.fontSize = 15;
                _style.richText = true;
                _style.alignment = TextAnchor.MiddleCenter;

                _backdropArea = new Rect(0, 0, Width, Height);

                _logo = Resources.Load<Texture2D>("Images/patch_little");
                _logoSize = new Vector2(250, 74);
                _logoArea = new Rect((Width / 2) - (_logoSize.x / 2), (Height / 2) - (_logoSize.y * 2), _logoSize.x, _logoSize.y);

                _textArea = new Rect((Width / 2) - (200), (Height / 2) - (30), 400, 60);

                switch (_errorType)
                {
                    case PopupErrorType.DotNetSubset:
                        _errorText = "Hey! P.A.T.C.H. doesn't work with .NET 2.0 Subset! Let's switch to .NET 2.0 atleast! You can find it in Player Settings!";
                        break;
                }
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
                    _logoArea = new Rect((Width / 2) - (_logoSize.x / 2), (Height / 2) - (_logoSize.y * 2), _logoSize.x, _logoSize.y);

                    _textArea = new Rect((Width / 2) - (200), (Height / 2) - (30), 400, 60);
                }

                var previous = GUI.skin;
                GUI.skin = Host.GetSkin(ThemeHelper.PopupColorName);

                GUI.Box(_backdropArea, "");

                GUI.DrawTexture(_logoArea, _logo, ScaleMode.ScaleToFit);

                GUI.Label(_textArea, "<color=" + ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor) + ">" + _errorText + "</color>", _style);

                GUI.skin = previous;
            }
        }

        private void CheckForPopupOpening()
        {
            _shouldBeRendered = ThemeHelper.HasToShowErrorPopup(out _errorType);
        }
    }
}
