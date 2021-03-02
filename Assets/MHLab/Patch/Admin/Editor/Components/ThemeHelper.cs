using System.Collections.Generic;
using MHLab.PATCH.Admin.Editor;
using MHLab.Patch.Admin.Editor.Components.Contents;
using MHLab.Patch.Admin.Editor.EditorHelpers;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.Components
{
    public static class ThemeHelper
    {
        public static string MainColorName = "MainColor";
        public static string SecondaryColorName = "SecondaryColor";
        public static string DarkColorName = "DarkColor";
        public static string PopupColorName = "PopupColor";

        public static readonly string[] SidebarButtons = new string[]
        {
            "Builds", 
            "Patches", 
            "Launcher", 
            "Options",
            "Info"
        };
        public static Dictionary<string, Widget> WindowContents;

        public static Color MainColor = new Color32(82, 82, 82, 255);
        public static Color SecondaryColor = new Color32(62, 62, 62, 255);

        public static Color TextColor = new Color32(240, 240, 240, 255);
        public static Color TipTitleColor = new Color32(200, 200, 200, 255);

        public static Color PopupColor = new Color32(0, 0, 0, 230);

        public static string ConvertToStringFormat(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a));
        }

        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static void InitializeContent(WidgetContainer container)
        {
            WindowContents = new Dictionary<string, Widget>()
            {
                { "Builds", WidgetFactory.CreateWidget<PatchBuildsContent>(container) },
                { "Patches", WidgetFactory.CreateWidget<PatchPatchesContent>(container) },
                { "Launcher", WidgetFactory.CreateWidget<PatchLauncherContent>(container) },
                { "Options", WidgetFactory.CreateWidget<PatchOptionsContent>(container) },
                { "Info", WidgetFactory.CreateWidget<PatchInfoContent>(container) },
            };
        }

        public static bool HasToShowErrorPopup(out PopupErrorType type)
        {
#if NET_2_0_SUBSET
            type = PopupErrorType.DotNetSubset;
            return true;
#else
            type = PopupErrorType.None;
            return false;
#endif
        }

        private const string HasBeenOpenedKey = "PatchHasBeenOpened";

        public static bool HasToShowTutorial()
        {
            if (PlayerPrefs.HasKey(HasBeenOpenedKey))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void ToggleHasBeenOpened(bool opened)
        {
            if(opened)
                PlayerPrefs.SetInt(HasBeenOpenedKey, 1);
            else
                PlayerPrefs.DeleteKey(HasBeenOpenedKey);
            PlayerPrefs.Save();
        }
    }
}
