using System.IO;
using MHLab.Patch.Core.Utilities;
using MHLab.Patch.Utilities;
using MHLab.Patch.Utilities.Serializing;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.Components.Contents
{
    public class PatchInfoContent : PatchContent
    {
        private const string Email = "m4nu.91@gmail.com";
        private const string Skype = "manhunterita";
        private const string Twitter = "@MHLabSoftware";
        private const string Discord = "https://discord.gg/0ndGBjvogdY5SnIw";
        private const string UnityForum = "http://forum.unity3d.com/threads/p-a-t-c-h-ultimate-patching-system.342320";
        private Vector2 _scrollPosition;

        private float _margin = 4f;

        private Rect _emailDescriptionArea;
        private Rect _emailArea;

        private Rect _skypeDescriptionArea;
        private Rect _skypeArea;

        private Rect _twitterDescriptionArea;
        private Rect _twitterArea;
        
        private Rect _discordDescriptionArea;
        private Rect _discordArea;

        private Rect _unityForumDescriptionArea;
        private Rect _unityForumArea;

        private Rect _debugDescriptionArea;
        private Rect _debugButtonArea;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void UpdateUISize()
        {
            base.UpdateUISize();
            var height = ContentArea.position.y;

            _emailDescriptionArea = new Rect(ContentArea.position.x, height, ContentArea.width * 8 / 12, 40);
            _emailArea = new Rect(ContentArea.position.x + ContentArea.width * 8 / 12, height, ContentArea.width * 4 / 12, 30);

            height += 40 + _margin;

            _skypeDescriptionArea = new Rect(ContentArea.position.x, height, ContentArea.width * 8 / 12, 40);
            _skypeArea = new Rect(ContentArea.position.x + ContentArea.width * 8 / 12, height, ContentArea.width * 4 / 12, 30);

            height += 40 + _margin;

            _twitterDescriptionArea = new Rect(ContentArea.position.x, height, ContentArea.width * 9 / 12, 40);
            _twitterArea = new Rect(ContentArea.position.x + ContentArea.width * 8 / 12, height, ContentArea.width * 4 / 12, 30);

            height += 40 + _margin;

            _discordDescriptionArea = new Rect(ContentArea.position.x, height, ContentArea.width * 8 / 12, 40);
            _discordArea = new Rect(ContentArea.position.x + ContentArea.width * 8 / 12, height, ContentArea.width * 4 / 12, 30);

            height += 40 + _margin;

            _unityForumDescriptionArea = new Rect(ContentArea.position.x, height, ContentArea.width * 8 / 12, 40);
            _unityForumArea = new Rect(ContentArea.position.x + ContentArea.width * 8 / 12, height, ContentArea.width * 4 / 12, 30);
            
            height += 40 + _margin;

            _debugDescriptionArea = new Rect(ContentArea.position.x, height, ContentArea.width * 8 / 12, 40);
            _debugButtonArea = new Rect(ContentArea.position.x + ContentArea.width * 8 / 12, height, ContentArea.width * 4 / 12, 30);
        }

        public override void Render()
        {
            base.Render();
            _scrollPosition = GUI.BeginScrollView(ContentArea, _scrollPosition, ContentArea);

            GUI.Label(_emailDescriptionArea, "Email<i><size=9><color=#808080ff> - Contact me here for private support.</color></size></i>");
            GUI.TextField(_emailArea, Email);

            GUI.Label(_skypeDescriptionArea, "Skype<i><size=9><color=#808080ff> - Contact me here for private realtime support.</color></size></i>");
            GUI.TextField(_skypeArea, Skype);

            GUI.Label(_twitterDescriptionArea, "Twitter<i><size=9><color=#808080ff> - Follow me to receive info and news about P.A.T.C.H.!</color></size></i>");
            GUI.TextField(_twitterArea, Twitter);

            GUI.Label(_discordDescriptionArea, "Discord<i><size=9><color=#808080ff> - Join us: here we discuss about bugs, beta versions, fixes, new features, etc.</color></size></i>");
            GUI.TextField(_discordArea, Discord);

            GUI.Label(_unityForumDescriptionArea, "Unity Forum<i><size=9><color=#808080ff> - The official Unity Forum thread!</color></size></i>");
            GUI.TextField(_unityForumArea, UnityForum);
            
            GUI.Label(_debugDescriptionArea, "Debug report<i><size=9><color=#808080ff> - If you contact me for support, remember to include the debug report!</color></size></i>");
            if (GUI.Button(_debugButtonArea, "Generate debug report"))
            {
                TriggerDebugReportCollection();
            }
            
            GUI.EndScrollView();
        }

        private void TriggerDebugReportCollection()
        {
            var system = DebugHelper.GetSystemInfo();
            var report = Debugger.GenerateDebugReport(CurrentWindow.AdminSettings, system, new NewtonsoftSerializer());
            
            File.WriteAllText(CurrentWindow.AdminSettings.GetDebugReportFilePath(), report);
            
#if UNITY_EDITOR_WIN
            System.Diagnostics.Process.Start("explorer.exe", "/select, \"" + CurrentWindow.AdminSettings.GetDebugReportFilePath() + "\"");
#endif
        }
    }
}
