using System;
using System.IO;
using MHLab.Patch.Admin.Editor.Components;
using MHLab.Patch.Admin.Editor.EditorHelpers;
using MHLab.Patch.Admin.Editor.Localization;
using MHLab.Patch.Core.Admin;
using MHLab.Patch.Core.Admin.Localization;
using MHLab.Patch.Core.IO;
using UnityEditor;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor
{
    public sealed class AdminWindow : EditorWindow
    {
        public static class AdminWindowMenu
        {
            [MenuItem("Window/PATCH/Admin Tool #&p")]
            public static void ShowAdminWindow()
            {
                ShowWindow();
            }
            
            [MenuItem("Window/PATCH/Go to workspace folder #&o")]
            public static void OpenWorkspaceFolder()
            {
                System.Diagnostics.Process.Start(Path.Combine(Path.GetDirectoryName(Application.dataPath), WorkspaceFolderName));
            }
            
            [MenuItem("Window/PATCH/Read the manual")]
            public static void OpenDocumentation()
            {
                System.Diagnostics.Process.Start("https://github.com/manhunterita/PATCH/wiki");
            }
        }
        
        private static EditorWindow _currentWindow;
        private static bool _isInitialized = false;
        private const string WorkspaceFolderName = "PATCHWorkspace";
        
        public IAdminSettings AdminSettings;
        public IAdminLocalizedMessages Localization;
        
        private WidgetContainer _widgets;

        private static void ShowWindow()
        {
            const int minWidth = 800;
            const int minHeight = 600;
            const string windowTitle = "PATCH - Admin Tool";

            var window = GetWindow<AdminWindow>(false, windowTitle);
            window.minSize = GetWindowSize(minWidth, minHeight);
            _currentWindow = window;
            
            window.Initialize();
            
            window.Show();
        }
        
        private static Vector2 GetWindowSize(int minWidth, int minHeight)
        {
            if (Screen.currentResolution.width < minWidth)
                minWidth = Screen.currentResolution.width;
            if (Screen.currentResolution.height < minHeight)
                minHeight = Screen.currentResolution.height;
            
            return new Vector2(minWidth, minHeight);
        }

        private void Initialize()
        {
            if (!_isInitialized)
            {
                InitializeSettings();
                
                InitializeInterface();
                
                _isInitialized = true;
            }
        }

        private void InitializeSettings()
        {
            AdminSettings = new AdminSettings();

            AdminSettings.RootPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), WorkspaceFolderName);
            AdminSettings.AppDataPath = PathsManager.GetSpecialPath(Environment.SpecialFolder.ApplicationData);
            
            Localization = new EnglishAdminLocalizedMessages();
        }
        
        private void InitializeInterface()
        {
            _widgets = WidgetContainer.Create(this);
            _widgets.MinSize = _currentWindow.minSize;

            _widgets.AddSkin(ThemeHelper.MainColorName, Resources.Load<GUISkin>("PatchMainGUISkin"));
            _widgets.AddSkin(ThemeHelper.SecondaryColorName, Resources.Load<GUISkin>("PatchSecondaryGUISkin"));
            _widgets.AddSkin(ThemeHelper.DarkColorName, Resources.Load<GUISkin>("PatchDarkGUISkin"));
            _widgets.AddSkin(ThemeHelper.PopupColorName, Resources.Load<GUISkin>("PatchPopupGUISkin"));

            ThemeHelper.InitializeContent(_widgets);

            SetContainerComponents(_widgets);
        }

        public static void SetContainerComponents(WidgetContainer widgets)
        {
            widgets.ClearComponents();
            
            if (ThemeHelper.HasToShowErrorPopup(out var type))
            {
                widgets.Push<PatchErrorPopup>();
			}
			/*else if (ThemeHelper.HasToSetProjectName())
            {
	            widgets.Push<PatchProjectSettings>();
            }*/
			else if (ThemeHelper.HasToShowTutorial())
            {
                widgets.Push<PatchTutorial>();
            }
            else
            {
                widgets.Push<PatchTopbar>();
                widgets.Push<PatchWindow>();
                widgets.Push<PatchSidebar>();
                widgets.Push<PatchTipPopup>();
                widgets.Push<PatchPopup>();
                widgets.Push<PatchErrorPopup>();
            }
        }

        private void OnInspectorUpdate()
        {
            if (_isInitialized)
            {
                _widgets.Update();
                Repaint();
            }

            if (EditorApplication.isCompiling)
            {
                Close();
            }
        }

        private void OnGUI()
        {
            if (_isInitialized)
            {
                _widgets.Render();
            }
        }
        
        private void OnDestroy()
        {
            _isInitialized = false;
            _currentWindow = null;
        }
    }
}