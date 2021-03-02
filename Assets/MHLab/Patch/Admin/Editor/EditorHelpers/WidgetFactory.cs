using System;

namespace MHLab.Patch.Admin.Editor.EditorHelpers
{
    public class WidgetFactory
    {
        public static T CreateWidget<T>(WidgetContainer host) where T : Widget
        {
            var widget = (T)Activator.CreateInstance<T>();
            widget.Host = host;
            widget.Initialize();

            return widget;
        }
    }
}
