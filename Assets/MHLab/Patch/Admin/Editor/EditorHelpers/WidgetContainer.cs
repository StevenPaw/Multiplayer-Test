using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.EditorHelpers
{
    public sealed class WidgetContainer
    {
        private EditorWindow _host;

        public EditorWindow CurrentWindow
        {
            get { return _host; }
        }

        private Vector2 _minSize = new Vector2(-1, -1);
        public Vector2 MinSize
        {
            get { return _minSize; }
            set { _minSize = value; }
        }

        private Vector2 _maxSize = new Vector2(-1, -1);
        public Vector2 MaxSize
        {
            get { return _maxSize; }
            set { _maxSize = value; }
        }

        public Vector2 Size
        {
            get { return _host.position.size; }
        }

        public float Width { get { return Size.x; } }
        public float Height { get { return Size.y; } }

        private readonly List<Widget> _uiComponents;

        private readonly Dictionary<string, object> _data;

        private readonly Dictionary<string, GUISkin> _skins;

        public static WidgetContainer Create(EditorWindow host)
        {
            return new WidgetContainer(host);
        }

        private WidgetContainer(EditorWindow host)
        {
            _host = host;
            _uiComponents = new List<Widget>();
            _data = new Dictionary<string, object>();
            _skins = new Dictionary<string, GUISkin>();
        }

        public void Push<T>() where T : Widget
        {
            _uiComponents.Add(WidgetFactory.CreateWidget<T>(this));
        }

        public void ClearComponents()
        {
            _uiComponents.Clear();
        }

        public void Render()
        {
            for (int i = 0; i < _uiComponents.Count; i++)
            {
                _uiComponents[i].Render();
            }
        }

        public void Update()
        {
            for (int i = 0; i < _uiComponents.Count; i++)
            {
                _uiComponents[i].Update();
            }
        }

        public void AddData(string key, object val)
        {
            _data[key] = val;
        }

        public T GetData<T>(string key)
        {
            return (T) _data[key];
        }

        public bool HasData(string key)
        {
            return _data.ContainsKey(key);
        }

        public void AddSkin(string key, GUISkin val)
        {
            _skins[key] = val;
        }

        public GUISkin GetSkin(string key)
        {
            return _skins[key];
        }
    }
}
