using System;
using System.Threading.Tasks;
using MHLab.Patch.Core.Admin;
using MHLab.Patch.Core.Admin.Progresses;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Utilities.Serializing;
using UnityEditor;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.Components.Contents
{
    public class PatchBuildsContent : PatchContent
    {
        private Vector2 _scrollPosition;

        private float _margin = 4f;

        private Rect _helpBoxArea;
        private Rect _lastVersionArea;

        private readonly string[] _releaseTypes = new string[] { "Patch release", "Minor release", "Major release" };
        private readonly string[] _releaseTypesText = new string[] { string.Empty, string.Empty, string.Empty };
        private int _releaseTypeIndex = 0;
        private Rect _releaseTypeArea;
        private Rect _releaseTypeTextArea;

        private Rect _cleaningTextArea;
        private Rect _cleaningToggleArea;
        private bool _cleaningFlag = true;

        private Rect _versionDescriptionArea;
        private Rect _versionArea;

        private Rect _filesCountArea;
        private Rect _filesSizeArea;

        private Rect _buildButtonArea;

        private bool _isTaskStarted = false;
        private float _progress = 0f;
        private bool _isCompleted = false;
        private bool _isFailed = false;
        private bool _isStopped = false;

        private Exception _currentError;

        private int _skippedTicks = 10;
        private int _filesCount;
        private string _filesSize;
        
        private string _lastVersion;
        
        private string _buildingLog = string.Empty;

        private AdminBuildContext _context;
        private BuildBuilder _builder;

        public override void Initialize()
        {
            base.Initialize();
            
            var progress = new Progress<BuilderProgress>();
            progress.ProgressChanged += ProgressChanged;
            
            _context = new AdminBuildContext(CurrentWindow.AdminSettings, progress);
            _context.Logger = new MHLab.Patch.Utilities.Logging.Logger(CurrentWindow.AdminSettings.GetLogsFilePath(), CurrentWindow.AdminSettings.DebugMode);
            _context.Serializer = new NewtonsoftSerializer();
            _context.LocalizedMessages = CurrentWindow.Localization;
            _context.Initialize();
            
            _builder = new BuildBuilder(_context);
        }

        private void ProgressChanged(object sender, BuilderProgress e)
        {
            _progress = (float)e.CurrentSteps / e.TotalSteps;
            _buildingLog = e.StepMessage;
        }

        private void BuilderOnFailed(Exception e)
        {
            _isFailed = true;
            _currentError = e;
        }

        private void BuilderOnCompleted()
        {
            _isCompleted = true;

            if (_cleaningFlag)
            {
                DirectoriesManager.Clean(CurrentWindow.AdminSettings.GetApplicationFolderPath());
            }
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
                        Host.CurrentWindow.ShowNotification(new GUIContent("Nice! Build " + _context.BuildVersion + " has been successfully processed!"));
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
                        Host.CurrentWindow.ShowNotification(new GUIContent("Ooops! Build " + _context.BuildVersion + " triggered an error! Check the console!"));
                        Debug.Log(_currentError);

                        _currentError = null;
                        _isStopped = true;
                        _isTaskStarted = false;
                    }
                }
                else
                {
                    EditorUtility.DisplayProgressBar("PATCH - Build creator", _buildingLog, _progress);
                }
            }
        }

        public override void UpdateUISize()
        {
            base.UpdateUISize();

            var height = ContentArea.position.y;
            
            _helpBoxArea = new Rect(ContentArea.position.x, ContentArea.position.y, ContentArea.width, 30);

            if (_lastVersion != "none")
            {
                _lastVersionArea = new Rect(ContentArea.position.x, height, ContentArea.width, 30);

                height += 40 + _margin;

                _filesCountArea = new Rect(ContentArea.position.x, height, ContentArea.width, 40);

                height += 40 + _margin;

                _filesSizeArea = new Rect(ContentArea.position.x, height, ContentArea.width, 40);
                
                height += 40 + _margin;

                _cleaningTextArea = new Rect(ContentArea.position.x, height, ContentArea.width * 9 / 12, 40);
                _cleaningToggleArea = new Rect(ContentArea.position.x + ContentArea.width * 9 / 12, height, ContentArea.width * 3 / 12, 40);

                height += 40 + _margin;

                _releaseTypeTextArea = new Rect(ContentArea.position.x, height, ContentArea.width * 9 / 12, 40);
                _releaseTypeArea = new Rect(ContentArea.position.x + ContentArea.width * 9 / 12, height,
                    ContentArea.width * 3 / 12, 40);

                height += 40 + _margin;
            }

            _buildButtonArea = new Rect(ContentArea.position.x, height, ContentArea.width, 40);
        }

        private void UpdateAfterTicks()
        {
            if (_skippedTicks >= 10)
            {
                _skippedTicks = 0;

                UpdateRecognizedFiles();
                UpdateLastVersion();
                UpdateReleaseTypes();
            }

            _skippedTicks++;
        }
        
        private void UpdateRecognizedFiles()
        {
            try
            {
                _filesSize = _builder.GetCurrentFilesToProcessSize();
                _filesCount = _builder.GetCurrentFilesToProcessAmount();
            }
            catch
            {
                _filesSize = "0B";
                _filesCount = 0;
            }
        }

        private void UpdateLastVersion()
        {
            string lastVersionString = "none";
            try
            {
                var lastVersion = _context.GetLastVersion();
                if (lastVersion != null)
                {
                    lastVersionString = lastVersion.ToString();
                }
            }
            catch
            {
            }

            _lastVersion = lastVersionString;
        }

        private void UpdateReleaseTypes()
        {
            var currentVersion = _context.GetLastVersion();

            if (currentVersion == null) return;
            
            var version = _context.VersionFactory.Create(currentVersion);
            version.UpdatePatch();
            _releaseTypesText[0] = $"{_releaseTypes[0]}: {version}";
            
            version = _context.VersionFactory.Create(currentVersion);
            version.UpdateMinor();
            _releaseTypesText[1] = $"{_releaseTypes[1]}: {version}";
            
            version = _context.VersionFactory.Create(currentVersion);
            version.UpdateMajor();
            _releaseTypesText[2] = $"{_releaseTypes[2]}: {version}";
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

            _scrollPosition = GUI.BeginScrollView(ContentArea, _scrollPosition, ContentArea);

            if (_filesCount > 0)
            {
                var buttonText = "Initial build";
                
                if (_lastVersion != "none")
                {
                    EditorGUI.HelpBox(_lastVersionArea, "Last created version: " + _lastVersion, MessageType.Info);

                    GUI.Label(_filesCountArea,
                        "Recognized files count:<i><size=11><color=#808080ff> " + _filesCount + "</color></size></i>");

                    GUI.Label(_filesSizeArea,
                        "Recognized files size:<i><size=11><color=#808080ff> " + _filesSize + "</color></size></i>");

                    GUI.Label(_cleaningTextArea,
                        "Clean after build<i><size=9><color=#808080ff> - This will clean the \"" + CurrentWindow.AdminSettings.ApplicationFolderName + "\" folder after build</color></size></i>");
                    _cleaningFlag = EditorGUI.Toggle(_cleaningToggleArea, _cleaningFlag);
                    
                    GUI.Label(_releaseTypeTextArea,
                        "Release type<i><size=9><color=#808080ff> - This will affect how the version number is increased</color></size></i>");
                    _releaseTypeIndex = EditorGUI.Popup(_releaseTypeArea, _releaseTypeIndex, _releaseTypesText);

                    buttonText = "Build new version";
                }

                if (!_isTaskStarted)
                {
                    if (GUI.Button(_buildButtonArea, buttonText))
                    {
                        Task.Run(() =>
                        {
                            BuilderOnStarted();

                            try
                            {
                                TriggerBuild();

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
            else
            {
                EditorGUI.HelpBox(_helpBoxArea, "Hey! There are no files ready to be built. You should build your game first and put it in \"" + _context.Settings.ApplicationFolderName +"\" folder! Read the doc if you are not sure how to do it!", MessageType.Warning);
            }

            GUI.EndScrollView();
        }

        private void TriggerBuild()
        {
            var currentVersion = _context.VersionFactory.Create(_context.GetLastVersion());

            if (currentVersion != null)
            {
                switch (_releaseTypeIndex)
                {
                    case 0:
                        currentVersion.UpdatePatch();
                        break;
                    case 1:
                        currentVersion.UpdateMinor();
                        break;
                    case 2:
                        currentVersion.UpdateMajor();
                        break;
                }
            }
            else
            {
                currentVersion = _context.VersionFactory.Create();
            }

            _context.BuildVersion = currentVersion;
            _context.Initialize();
            _builder.Build();
        }
    }
}
