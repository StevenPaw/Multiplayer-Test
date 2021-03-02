using MHLab.Patch.Admin.Editor.EditorHelpers;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.Components
{
    public class PatchTopbar : Widget
    {
        private Rect _topbarArea;
        private Vector2 _previousHostSize;

        private Rect _borderArea;

        private Texture2D _logo;
        private Texture2D _underLogo;
        private Vector2 _logoSize;
        private Rect _logoArea;
        private Rect _underLogoArea;

        private Rect _openWorkspaceButtonArea;
        private Rect _openDocButtonArea;
        private Vector2 _topbarButtonSize;

        public override void Initialize()
        {
            base.Initialize();
            Size = new Vector2(Host.Width, (Host.MinSize.y * 15) / 100);
            _previousHostSize = Host.Size;

            _topbarArea = new Rect(0, 0, Width, Height);
            
            _borderArea = new Rect(0, Height - 10, Width, 10);

            _logo = Resources.Load<Texture2D>("Images/logo_editor");
            _underLogo = Resources.Load<Texture2D>("Images/oblique_filler");
            _logoSize = new Vector2(140, Height * 80 / 100);
            //_logoArea = new Rect(Host.Width / 2 - _logoSize.x / 2, 0, _logoSize.x, _logoSize.y);
            _underLogoArea = new Rect(0, 0, _logoSize.x + 50, Height - 10);
            _logoArea = new Rect(10, 5, _logoSize.x, _logoSize.y);

            _topbarButtonSize = new Vector2(120, 24);
            _openWorkspaceButtonArea = new Rect(Width - 10 - _topbarButtonSize.x, 8, _topbarButtonSize.x, _topbarButtonSize.y);
            _openDocButtonArea = new Rect(Width - 10 - _topbarButtonSize.x, 16 + _topbarButtonSize.y, _topbarButtonSize.x, _topbarButtonSize.y);
        }

        public override void Render()
        {
            base.Render();
            
            if (_previousHostSize != Host.Size)
            {
                _previousHostSize = Host.Size;
                Size = new Vector2(Host.Width, (Host.MinSize.y * 15) / 100);
                _topbarArea = new Rect(0, 0, Width, Height);
                _borderArea = new Rect(0, Height - 10, Width, 10);
                _underLogoArea = new Rect(0, 0, _logoSize.x + 50, Height - 10);
                //_logoArea = new Rect(Host.Width / 2 - _logoSize.x / 2, 0, _logoSize.x, _logoSize.y);
                _logoArea = new Rect(10, 5, _logoSize.x, _logoSize.y);
                _openWorkspaceButtonArea = new Rect(Width - 10 - _topbarButtonSize.x, 10, _topbarButtonSize.x, _topbarButtonSize.y);
                _openDocButtonArea = new Rect(Width - 10 - _topbarButtonSize.x, 16 + _topbarButtonSize.y, _topbarButtonSize.x, _topbarButtonSize.y);
            }

            var previous = GUI.skin;
            GUI.skin = Host.GetSkin(ThemeHelper.SecondaryColorName);
            GUI.Box(_topbarArea, "");

            GUI.skin = Host.GetSkin(ThemeHelper.DarkColorName);
            GUI.Box(_borderArea, "");
            
            GUI.DrawTexture(_underLogoArea, _underLogo, ScaleMode.StretchToFill);
            GUI.DrawTexture(_logoArea, _logo, ScaleMode.ScaleToFit);
            
            GUI.skin = Host.GetSkin(ThemeHelper.SecondaryColorName);
            if (GUI.Button(_openWorkspaceButtonArea, "Go to workspace"))
            {
                AdminWindow.AdminWindowMenu.OpenWorkspaceFolder();
            }
            
            if (GUI.Button(_openDocButtonArea, "Open the doc"))
            {
                AdminWindow.AdminWindowMenu.OpenDocumentation();
            }
            

            GUI.skin = previous;
        }
    }
}
