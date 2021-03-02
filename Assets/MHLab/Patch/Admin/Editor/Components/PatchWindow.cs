using MHLab.Patch.Admin.Editor.EditorHelpers;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.Components
{
    public class PatchWindow : Widget
    {
        private Rect _contentArea;
        private Vector2 _previousHostSize;

        public override void Initialize()
        {
            base.Initialize();

            Size = new Vector2(Host.Width - (Host.MinSize.x * 20) / 100, Host.Height - (Host.MinSize.y * 15) / 100);

            _previousHostSize = Host.Size;
            _contentArea = new Rect((Host.MinSize.x * 20) / 100, (Host.MinSize.y * 15) / 100, Width, Height);
        }

        public override void Update()
        {
            base.Update();
            ThemeHelper.WindowContents[ThemeHelper.SidebarButtons[Host.GetData<int>(PatchSidebar.SelectedView)]].Update();
        }

        public override void Render()
        {
            base.Render();

            if (_previousHostSize != Host.Size)
            {
                Size = new Vector2(Host.Width - (Host.MinSize.x * 20) / 100, Host.Height - (Host.MinSize.y * 15) / 100);
                _contentArea = new Rect((Host.MinSize.x * 20) / 100, (Host.MinSize.y * 15) / 100, Width, Height);
                _previousHostSize = Host.Size;
            }

            var previous = GUI.skin;
            GUI.skin = Host.GetSkin(ThemeHelper.SecondaryColorName);

            GUI.Box(_contentArea, "");

            RenderContent(Host.GetData<int>(PatchSidebar.SelectedView));

            GUI.skin = previous;
        }

        private void RenderContent(int view)
        {
            ThemeHelper.WindowContents[ThemeHelper.SidebarButtons[view]].Render();
        }
    }
}
