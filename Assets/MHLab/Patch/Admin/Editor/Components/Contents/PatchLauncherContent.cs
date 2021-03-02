using System;
using System.Threading;
using System.Threading.Tasks;
using MHLab.Patch.Core.Admin;
using MHLab.Patch.Core.Admin.Progresses;
using MHLab.Patch.Utilities.Serializing;
using UnityEditor;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.Components.Contents
{
    public class PatchLauncherContent : PatchContent
    {
        public const string LauncherNameKey = "DeployLauncherName";

        private Vector2 _scrollPosition;

        private Rect _helpBoxArea;

        private string _archiveNameText = "";
        private Rect _deployDescriptionArea;
        private Rect _archiveTextArea;
        private Rect _deployButtonArea;

        private Rect _compressionTextArea;
        private Rect _compressionSelectorArea;
        private int _compressionValue = 9;

        private Rect _filesCountArea;
        private Rect _filesSizeArea;
        
        private int _filesCount;
        private string _filesSize;

        private bool _isTaskStarted = false;
        private float _progress = 0f;
        private string _buildingLog = string.Empty;
        private bool _isCompleted = false;
        private bool _isFailed = false;
        private bool _isStopped = false;

        private Exception _currentError;
        
        private int _skippedTicks = 10;
        
        private AdminPatcherUpdateContext _context;
        private UpdaterBuilder _builder;

        public override void Initialize()
        {
            base.Initialize();

            var progress = new Progress<BuilderProgress>();
            progress.ProgressChanged += BuilderOnProgress;
            
            _context = new AdminPatcherUpdateContext(CurrentWindow.AdminSettings, progress);
            _context.Logger = new MHLab.Patch.Utilities.Logging.Logger(CurrentWindow.AdminSettings.GetLogsFilePath(), CurrentWindow.AdminSettings.DebugMode);
            _context.Serializer = new NewtonsoftSerializer();
            _context.LocalizedMessages = CurrentWindow.Localization;
            _context.Initialize();
            
            _builder = new UpdaterBuilder(_context);

            if (PlayerPrefs.HasKey(LauncherNameKey))
                _archiveNameText = PlayerPrefs.GetString(LauncherNameKey);
        }

        private void BuilderOnFailed(Exception e)
        {
            _isFailed = true;
            _currentError = e;
        }

        private void BuilderOnCompleted()
        {
            _isCompleted = true;
        }

        private void BuilderOnProgress(object sender, BuilderProgress e)
        {
            _progress = (float)e.CurrentSteps / e.TotalSteps;
            _buildingLog = e.StepMessage;
        }

        private void BuilderOnStarted()
        {
            _isTaskStarted = true;
        }

        private void UpdateProgress()
        {
            if (_isStopped)
            {
                _isTaskStarted = false;
                _isCompleted = false;
                _isFailed = false;
                _progress = 0;
                _isStopped = false;
            }

            if (_isTaskStarted)
            {
                if (_isCompleted)
                {
                    if (!_isStopped)
                    {
                        Host.CurrentWindow.ShowNotification(new GUIContent("Nice! Launcher " + _archiveNameText + " deployment has been successfully processed!"));
                        EditorUtility.ClearProgressBar();
                        _isStopped = true;
                        _isTaskStarted = false;
                    }
                }
                else if (_isFailed)
                {
                    if (!_isStopped)
                    {
                        EditorUtility.ClearProgressBar();
                        Host.CurrentWindow.ShowNotification(new GUIContent("Ooops! Launcher " + _archiveNameText + " deploying triggered an error! Check the console!"));
                        Debug.Log(_currentError);

                        _currentError = null;
                        _isStopped = true;
                        _isTaskStarted = false;
                    }
                }
                else
                {
                    EditorUtility.DisplayProgressBar("PATCH - Launcher creator", _buildingLog, _progress);
                }
            }
        }
        
        public override void UpdateUISize()
        {
            base.UpdateUISize();

            var height = ContentArea.position.y;

            _helpBoxArea = new Rect(ContentArea.position.x, height, ContentArea.width, 30);

            _deployDescriptionArea = new Rect(ContentArea.position.x, height, ContentArea.width * 9 / 12, 40);
            _archiveTextArea = new Rect(ContentArea.position.x + ContentArea.width * 9 / 12, height, ContentArea.width * 3 / 12, 25);

            height += 40;
            
            _compressionTextArea = new Rect(ContentArea.position.x, height, ContentArea.width * 9 / 12, 40);
            _compressionSelectorArea = new Rect(ContentArea.position.x + ContentArea.width * 9 / 12, height, ContentArea.width * 3 / 12, 25);

            height += 40;
            
            _filesCountArea = new Rect(ContentArea.position.x, height, ContentArea.width * 6 / 12, 40);
            _filesSizeArea = new Rect(ContentArea.position.x + ContentArea.width * 6 / 12, height, ContentArea.width * 6 / 12, 40);

            height += 40;
            
            _deployButtonArea = new Rect(ContentArea.position.x, height, ContentArea.width, 40);
        }
        
        private void UpdateAfterTicks()
        {
            if (_skippedTicks >= 10)
            {
                _skippedTicks = 0;

                UpdateFilesData();
            }

            _skippedTicks++;
        }

        private void UpdateFilesData()
        {
            _filesCount = _builder.GetCurrentFilesToProcessAmount();
            _filesSize = _builder.GetCurrentFilesToProcessSize();
        }

        public override void Update()
        {
            base.Update();

            UpdateProgress();
            UpdateAfterTicks();
        }

        public override void Render()
        {
            base.Render();

            UpdateUISize();

            if (_filesCount > 0)
            {
                GUI.Label(_deployDescriptionArea, "Launcher archive name<i><size=9><color=#808080ff> - Building a new launcher update generates an archive you can distribute. Set its name!</color></size></i>");
                _archiveNameText = EditorGUI.TextField(_archiveTextArea, _archiveNameText, Host.GetSkin(ThemeHelper.SecondaryColorName).textField);
                
                GUI.Label(_compressionTextArea, "Compression Level<i><size=9><color=#808080ff> - Set the compression level for the deployed archive.</color></size></i>");
                _compressionValue = EditorGUI.IntSlider(_compressionSelectorArea, _compressionValue, 1, 9);
                
                GUI.Label(_filesCountArea, "Files count: <i><size=11><color=#808080ff>" + _filesCount + "</color></size></i>");
                GUI.Label(_filesSizeArea, "Files size: <i><size=11><color=#808080ff>" + _filesSize + "</color></size></i>");

                if (!_isTaskStarted)
                {
                    if (GUI.Button(_deployButtonArea, "Build Launcher update"))
                    {
                        if (string.IsNullOrWhiteSpace(_archiveNameText))
                        {
                            Host.CurrentWindow.ShowNotification(new GUIContent("You must set Launcher archive name!"));
                        }
                        else
                        {
                            PlayerPrefs.SetString(LauncherNameKey, _archiveNameText);
                            Task.Run(() =>
                            {
                                BuilderOnStarted();

                                try
                                {
                                    TriggerBuilder();

                                    BuilderOnCompleted();
                                }
                                catch (Exception e)
                                {
                                    BuilderOnFailed(e);
                                }
                            });
                        }
                    }
                }
            }
            else
            {
                EditorGUI.HelpBox(_helpBoxArea,
                    "Hey! There are no files ready in \"" + _context.Settings.UpdaterFolderName + "\" folder. You should build your launcher first and then put it in \"" + _context.Settings.UpdaterFolderName + "\" folder! Read the doc if you are not sure how to do it!",
                    MessageType.Warning);
            }
        }

        private void TriggerBuilder()
        {
            _context.CompressionLevel = _compressionValue;
            _context.LauncherArchiveName = _archiveNameText;
            _context.Initialize();
            _builder.Build();
        }
    }
}
