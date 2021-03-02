using MHLab.Patch.Admin.Editor.EditorHelpers;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.Components.Contents
{
    public class PatchContent : Widget
    {
        private float _padding = 30;

        private Rect _contentArea;
        protected Vector2 _previousHostSize;

        protected Rect ContentArea
        {
            get { return _contentArea; }
        }

        protected AdminWindow CurrentWindow => (AdminWindow)Host.CurrentWindow;

        public override void Initialize()
        {
            base.Initialize();

            UpdateUISize();
        }

        public virtual void UpdateUISize()
        {
            Size = new Vector2(Host.Width - (Host.MinSize.x * 20) / 100 - _padding * 2, Host.Height - (Host.MinSize.y * 15) / 100 - _padding * 2);
            _contentArea = new Rect((Host.MinSize.x * 20) / 100 + _padding, (Host.MinSize.y * 15) / 100 + _padding, Width, Height);
            _previousHostSize = Host.Size;
        }

        public override void Render()
        {
            base.Render();

            if (_previousHostSize != Host.Size)
            {
                UpdateUISize();
            }
        }
    }
}
