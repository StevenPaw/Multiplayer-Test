using System.Collections.Generic;
using MHLab.Patch.Admin.Editor.EditorHelpers;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.Components
{
    public class PatchTutorial : Widget
    {
        private bool _shouldBeRendered = false;

        private GUIStyle _style;
        private Rect _backdropArea;

        private Texture2D _logo;
        private Vector2 _logoSize;
        private Rect _logoArea;
        private Rect _logoTopArea;

        private Rect _textArea;
        private Rect _nextButtonArea;
        private Rect _previousButtonArea;
        private Rect _skipTutorialButtonArea;

        private Vector2 _previousHostSize;

        private int _tutorialCurrentPage = 1;

        private int TutorialCurrentPage
        {
            get { return _tutorialCurrentPage; }
            set
            {
                if (value > _tutorialPages.Count)
                    _tutorialCurrentPage = _tutorialPages.Count;
                else if (value < 1)
                    _tutorialCurrentPage = 1;
                else
                    _tutorialCurrentPage = value;
            }
        }

        private Rect _firstBoxArea;
        private Rect _secondBoxArea;

        private List<TutorialPage> _tutorialPages = new List<TutorialPage>();

        private Texture2D _branchesImage;
        private Texture2D _branchesImage2;
        private Texture2D _buildsImage;
        private Texture2D _buildsImage2;
        private Texture2D _patchesImage;
        private Texture2D _patchesImage2;
        private Texture2D _launcherImage;
        private Texture2D _launcherImage2;
        private Texture2D _configurationImage;
        private Texture2D _serverImage;
        private Texture2D _optionsImage;
        private Texture2D _logsImage;
        private Texture2D _logsImage2;

        public override void Initialize()
        {
            base.Initialize();
            Size = new Vector2(Host.Width, Host.Height);
            _previousHostSize = Host.Size;

            CheckForPopupOpening();

            _tutorialPages.Add(new WelcomeTutorialPage());
            
            _tutorialPages.Add(new BuildsTutorialPage());
            _tutorialPages.Add(new Builds2TutorialPage());
            _tutorialPages.Add(new PatchesTutorialPage());
            _tutorialPages.Add(new Patches2TutorialPage());
            _tutorialPages.Add(new LauncherTutorialPage());
            _tutorialPages.Add(new Launcher2TutorialPage());
            _tutorialPages.Add(new OptionsTutorialPage());

            _tutorialPages.Add(new ReadDocumentationTutorialPage());
            _tutorialPages.Add(new EndTutorialPage());

            if (_shouldBeRendered)
            {
                _style = new GUIStyle();
                _style.wordWrap = true;
                _style.fontSize = 15;
                _style.richText = true;
                _style.alignment = TextAnchor.MiddleCenter;

                _backdropArea = new Rect(0, 0, Width, Height);

                _logo = Resources.Load<Texture2D>("Images/logo_editor");
                _logoSize = new Vector2(250, 220);
                _logoArea = new Rect((Width / 2) - (_logoSize.x / 2), (Height / 2) - 300, _logoSize.x, _logoSize.y);
                _logoTopArea = new Rect((Width / 2) - (_logoSize.x / 2), 20, _logoSize.x, _logoSize.y);

                _textArea = new Rect((Width / 2) - (200), (Height / 2) - (30), 400, 60);

                _nextButtonArea = new Rect(Width - 20 - 100, Height - 20 - 30, 100, 30);
                _previousButtonArea = new Rect(20, Height - 20 - 30, 100, 30);
                _skipTutorialButtonArea = new Rect(Width / 2 - 50, Height - 20 - 30, 100, 30);

                _firstBoxArea = new Rect(20, _logoTopArea.y + _logoSize.y + 10, Width / 2 - 40, Height / 2);
                _secondBoxArea = new Rect(Width / 2 + 20, _logoTopArea.y + _logoSize.y + 10, Width / 2 - 40, Height / 2);
                
                _buildsImage = Resources.Load<Texture2D>("Images/Tutorial/Builds");
                _buildsImage2 = Resources.Load<Texture2D>("Images/Tutorial/Builds2");
                _patchesImage = Resources.Load<Texture2D>("Images/Tutorial/Patches");
                _patchesImage2 = Resources.Load<Texture2D>("Images/Tutorial/Patches2");
                _launcherImage = Resources.Load<Texture2D>("Images/Tutorial/Launcher");
                _launcherImage2 = Resources.Load<Texture2D>("Images/Tutorial/Launcher2");
                _optionsImage = Resources.Load<Texture2D>("Images/Tutorial/Options");
            }
        }

        public override void Render()
        {
            if (_shouldBeRendered)
            {
                base.Render();

                if (_previousHostSize != Host.Size)
                {
                    Size = new Vector2(Host.Width, Host.Height);
                    _backdropArea = new Rect(0, 0, Width, Height);
                    _logoArea = new Rect((Width / 2) - (_logoSize.x / 2), (Height / 2) - 300, _logoSize.x, _logoSize.y);
                    _logoTopArea = new Rect((Width / 2) - (_logoSize.x / 2), 20, _logoSize.x, _logoSize.y);

                    _textArea = new Rect((Width / 2) - (200), (Height / 2) - (30), 400, 60);

                    _nextButtonArea = new Rect(Width - 20 - 100, Height - 20 - 30, 100, 30);
                    _previousButtonArea = new Rect(20, Height - 20 - 30, 100, 30);
                    _skipTutorialButtonArea = new Rect(Width / 2 - 50, Height - 20 - 30, 100, 30);

                    _firstBoxArea = new Rect(20, _logoTopArea.y + _logoSize.y + 10, Width / 2 - 40, Height / 2);
                    _secondBoxArea = new Rect(Width / 2 + 20, _logoTopArea.y + _logoSize.y + 10, Width / 2 - 40, Height / 2);
                }

                var previous = GUI.skin;
                GUI.skin = Host.GetSkin(ThemeHelper.PopupColorName);

                GUI.Box(_backdropArea, "");

                RenderTutorial();

                if (TutorialCurrentPage < _tutorialPages.Count)
                {
                    if (GUI.Button(_nextButtonArea, "Next"))
                    {
                        TutorialCurrentPage++;
                    }
                }
                else
                {
                    if (GUI.Button(_nextButtonArea, "Complete!"))
                    {
                        ThemeHelper.ToggleHasBeenOpened(true);
                        AdminWindow.SetContainerComponents(Host);
                    }
                }

                if (TutorialCurrentPage > 1)
                {
                    if (GUI.Button(_previousButtonArea, "Previous"))
                    {
                        TutorialCurrentPage--;
                    }
                }

                if (GUI.Button(_skipTutorialButtonArea, "Skip tutorial"))
                {
                    ThemeHelper.ToggleHasBeenOpened(true);
                    AdminWindow.SetContainerComponents(Host);
                }

                GUI.skin = previous;
            }
        }

        private void CheckForPopupOpening()
        {
            _shouldBeRendered = ThemeHelper.HasToShowTutorial();
        }

        private void RenderTutorial()
        {
            _tutorialPages[TutorialCurrentPage - 1].Render(this);
            
        }

        protected abstract class TutorialPage
        {
            public abstract void Render(PatchTutorial host);
        }

        protected class WelcomeTutorialPage : TutorialPage
        {
            public override void Render(PatchTutorial host)
            {
                GUI.DrawTexture(host._logoArea, host._logo, ScaleMode.ScaleToFit);

                GUI.Label(host._textArea, "<color=" + ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor) + ">Hey! It seems like this is your first time here, so: welcome in PATCH!\n\n" +
                                          "I prepared a little tutorial to help you getting started with it. I don't want to be boring, so let's go: I hope you enjoy this tour!\n\n" +
                                          "Click on the \"Next\" button or on \"Skip tutorial\" if you don't need it!</color>", host._style);
            }
        }

        protected class BuildsTutorialPage : TutorialPage
        {
            public override void Render(PatchTutorial host)
            {
                GUI.DrawTexture(host._logoTopArea, host._logo, ScaleMode.ScaleToFit);

                GUI.Label(host._firstBoxArea, "<color=" + ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor) + "><b>Builds</b>\n\nA build is one of the main units in PATCH's workflow. It represents your" +
                                              " game's current state.\nPATCH's engine performs some computations on your game to generate all needed metadata, packages and version files. " +
                                              "To start using this tool, you need to build your game and place it in <i>\"" + ((AdminWindow)host.Host.CurrentWindow).AdminSettings.ApplicationFolderName + "\"</i> folder, under the PATCH's workspace.\n\n" +
                                              "You're ready to start now!</color>", host._style);
                GUI.DrawTexture(host._secondBoxArea, host._buildsImage, ScaleMode.ScaleToFit);
            }
        }

        protected class Builds2TutorialPage : TutorialPage
        {
            public override void Render(PatchTutorial host)
            {
                GUI.DrawTexture(host._logoTopArea, host._logo, ScaleMode.ScaleToFit);

                GUI.DrawTexture(host._firstBoxArea, host._buildsImage2, ScaleMode.ScaleToFit);

                GUI.Label(host._secondBoxArea, "<color=" + ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor) + ">In the Builds tab - after you placed your built game in the correct folder - you can create your first PATCH " +
                                               "build by pressing the <i>\"Build new version\"</i> button.\n<i>\"Release type\"</i> regulates the versioning number of your next build: you can read more about it in the doc.</color>", host._style);
            }
        }

        protected class PatchesTutorialPage : TutorialPage
        {
            public override void Render(PatchTutorial host)
            {
                GUI.DrawTexture(host._logoTopArea, host._logo, ScaleMode.ScaleToFit);

                GUI.Label(host._firstBoxArea, "<color=" + ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor) + "><b>Patches</b>\n\nA patch is the second main unit in PATCH's workflow. It represents " +
                                              "the binary difference between two builds.\nThis concept is really important: it allows you to publish small updates for your game. It is a lot more convenient than downloading the whole new build everytime!\n" +
                                              "To start using this tool, you must have at least two builds: then you will be able to generate the patch!</color>", host._style);
                GUI.DrawTexture(host._secondBoxArea, host._patchesImage, ScaleMode.ScaleToFit);
            }
        }

        protected class Patches2TutorialPage : TutorialPage
        {
            public override void Render(PatchTutorial host)
            {
                GUI.DrawTexture(host._logoTopArea, host._logo, ScaleMode.ScaleToFit);

                GUI.DrawTexture(host._firstBoxArea, host._patchesImage2, ScaleMode.ScaleToFit);

                GUI.Label(host._secondBoxArea, "<color=" + ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor) + ">Patches generation is really straightforward: just select game builds (PATCH automatically " +
                                               "selects current one and previous one), the compression level and press the button! You did it! :)</color>", host._style);
            }
        }

        protected class LauncherTutorialPage : TutorialPage
        {
            public override void Render(PatchTutorial host)
            {
                GUI.DrawTexture(host._logoTopArea, host._logo, ScaleMode.ScaleToFit);

                GUI.DrawTexture(host._firstBoxArea, host._launcherImage, ScaleMode.ScaleToFit);

                GUI.Label(host._secondBoxArea, "<color=" + ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor) + "><b>Launcher</b>\n\nYou may need to publish updates not only for your game, but for your Launcher too." +
                                               "That's really important to fix bugs or to extend Launcher's functionalities over time.</color>", host._style);
            }
        }

        protected class Launcher2TutorialPage : TutorialPage
        {
            public override void Render(PatchTutorial host)
            {
                GUI.DrawTexture(host._logoTopArea, host._logo, ScaleMode.ScaleToFit);

                GUI.Label(host._firstBoxArea, "<color=" + ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor) + ">Just place your built Launcher in the \"Updater\" folder, set a name for the archive, " +
                                              "the compression level and hit the <i>\"Build Launcher update\"</i> button. Your update for your Launcher is ready!</color>", host._style);
                GUI.DrawTexture(host._secondBoxArea, host._launcherImage2, ScaleMode.ScaleToFit);
            }
        }
        
        protected class OptionsTutorialPage : TutorialPage
        {
            public override void Render(PatchTutorial host)
            {
                GUI.DrawTexture(host._logoTopArea, host._logo, ScaleMode.ScaleToFit);

                GUI.DrawTexture(host._firstBoxArea, host._optionsImage, ScaleMode.ScaleToFit);

                GUI.Label(host._secondBoxArea, "<color=" + ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor) + "><b>Options</b>\n\nOptions tab contains all options and settings for this Admin Tool: " +
                                               "they only influence its behavior. Also, here you can find the button to relaunch this tutorial! I know you're happy now! :D</color>", host._style);
            }
        }

        protected class ReadDocumentationTutorialPage : TutorialPage
        {
            public override void Render(PatchTutorial host)
            {
                GUI.DrawTexture(host._logoArea, host._logo, ScaleMode.ScaleToFit);

                GUI.Label(host._textArea, "<color=" + ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor) + ">The last advice for you is: you really, really, REALLY should read the documentation " +
                                          "to get a full guide and a deep look into this software.\n\nDocumentation contains a lot of useful information and explanations about this tools and best practices!\n" +
                                          "Also, I wrote it with love: don't disappoint me! :)</color>", host._style);
            }
        }

        protected class EndTutorialPage : TutorialPage
        {
            public override void Render(PatchTutorial host)
            {
                GUI.DrawTexture(host._logoArea, host._logo, ScaleMode.ScaleToFit);

                GUI.Label(host._textArea, "<color=" + ThemeHelper.ConvertToStringFormat(ThemeHelper.TextColor) + ">This journey through PATCH's features is finally at its end!\n" +
                                          "I want to thank you for your patience and I hope you are not too much confused.\n" +
                                          "Also, remember that you can contact me to report problems or to ask for help!\n" +
                                          "Now it's time for me to leave and let you alone with your new, shining PATCH! Have fun and enjoy it! :)</color>", host._style);
            }
        }
    }
}
