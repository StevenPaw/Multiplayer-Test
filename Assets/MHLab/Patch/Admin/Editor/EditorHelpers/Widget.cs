using UnityEngine;

namespace MHLab.Patch.Admin.Editor.EditorHelpers
{
    public abstract class Widget
    {
        public WidgetContainer Host { get; set; }

        public Widget Parent { get; set; }

        public Vector2 Size { get; set; }
        public float Width { get { return Size.x; } }
        public float Height { get { return Size.y; } }

        public virtual void Initialize()
        {
            
        }
        
        public virtual void OnShow()
        {

        }

        public virtual void Render()
        {
            
        }

        public virtual void Update()
        {
            
        }
    }
}
