using MHLab.Patch.Admin.Editor.EditorHelpers;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.Components
{
    public class PatchSidebar : Widget
    {
        public const string SelectedView = "PatchSelectedView";

        private Rect _sidebarArea;
        private Vector2 _buttonSize;
        private Rect _buttonsArea;

        private Vector2 _previousHostSize;

        private Vector2 _arrowSize;
        private Rect _arrowArea;
        private Texture2D _arrow;

        private int _previousIndex;

        public override void Initialize()
        {
            base.Initialize();
            Size = new Vector2((Host.MinSize.x * 20) / 100, Host.Height);
            _previousHostSize = Host.Size;

            _sidebarArea = new Rect(0, (Host.MinSize.y * 15) / 100, Width, Height);
            _buttonSize = new Vector2(Width, 40);

            if (PlayerPrefs.HasKey(SelectedView))
            {
                Host.AddData(SelectedView, PlayerPrefs.GetInt(SelectedView));
            }
            else
            {
                Host.AddData(SelectedView, 0);
            }

            float buttonsHeight = 0;
            for (int i = 0; i < ThemeHelper.SidebarButtons.Length; i++)
            {
                buttonsHeight += _buttonSize.y;
            }
            _buttonsArea = new Rect(_sidebarArea.position.x, _sidebarArea.position.y, Width, buttonsHeight);

            _arrowSize = new Vector2(20, _buttonSize.y);

            _arrow = Resources.Load<Texture2D>("Images/editor_arrow");

            _previousIndex = Host.GetData<int>(SelectedView);
            _arrowArea = new Rect(Width, _sidebarArea.y + (Host.GetData<int>(SelectedView) * _buttonSize.y), _arrowSize.x, _arrowSize.y);
        }

        public override void Render()
        {
            base.Render();

            if (_previousHostSize != Host.Size)
            {
                _previousHostSize = Host.Size;
                Size = new Vector2((Host.MinSize.x * 20) / 100, Host.Height);
                _sidebarArea = new Rect(0, (Host.MinSize.y * 15) / 100, Width, Height);
                _buttonSize = new Vector2(Width, 40);
            }

            var previous = GUI.skin;
            GUI.skin = Host.GetSkin(ThemeHelper.MainColorName);
            GUI.Box(_sidebarArea, "");

            var index = GUI.SelectionGrid(_buttonsArea, Host.GetData<int>(SelectedView), ThemeHelper.SidebarButtons, 1);
            if (index != Host.GetData<int>(SelectedView))
            {
                PlayerPrefs.SetInt(SelectedView, index);
                ThemeHelper.WindowContents[ThemeHelper.SidebarButtons[Host.GetData<int>(PatchSidebar.SelectedView)]].OnShow();
            }
            Host.AddData(SelectedView, index);

            if (_previousIndex != Host.GetData<int>(SelectedView))
            {
                _arrowArea = new Rect(Width, _sidebarArea.y + (Host.GetData<int>(SelectedView) * _buttonSize.y),
                    _arrowSize.x, _arrowSize.y);
                _previousIndex = Host.GetData<int>(SelectedView);
            }

            GUI.DrawTexture(_arrowArea, _arrow, ScaleMode.ScaleToFit);

            GUI.skin = previous;
        }
    }
}
