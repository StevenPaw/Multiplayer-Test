using System;
using System.Linq;
using System.Threading.Tasks;
using MHLab.Patch.Core.Admin;
using MHLab.Patch.Core.Admin.Progresses;
using MHLab.Patch.Utilities.Serializing;
using UnityEditor;
using UnityEngine;

namespace MHLab.Patch.Admin.Editor.Components.Contents
{
    public class PatchPatchesContent : PatchContent
    {
        private Vector2 _scrollPosition;

        private float _margin = 4f;

        private Rect _helpBoxArea;
        private Rect _patchesDescriptionArea;
        private Rect _patchesArea1;
        private Rect _patchesArea2;
        private int _patchesIndex1;
        private int _patchesIndex2;

        private string[] _versions;

        private Rect _compressionTextArea;
        private Rect _compressionSelectorArea;
        private int _compressionValue = 9;

        private int _skippedTicks = 10;
        
        private bool _isTaskStarted = false;
        private float _progress = 0f;
        private string _buildingLog = string.Empty;
        private bool _isCompleted = false;
        private bool _isFailed = false;
        private bool _isStopped = false;

        private Exception _currentError;

        private Rect _patchButtonArea;
        
        private AdminPatchContext _context;
        private PatchBuilder _builder;

        public override void Initialize()
        {
            base.Initialize();
            
            var progress = new Progress<BuilderProgress>();
            progress.ProgressChanged += ProgressChanged;
            
            _context = new AdminPatchContext(CurrentWindow.AdminSettings, progress);
            _context.Logger = new MHLab.Patch.Utilities.Logging.Logger(CurrentWindow.AdminSettings.GetLogsFilePath(), CurrentWindow.AdminSettings.DebugMode);
            _context.Serializer = new NewtonsoftSerializer();
            _context.LocalizedMessages = CurrentWindow.Localization;
            
            _builder = new PatchBuilder(_context);
            
            InitializePatches();
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

        private void ProgressChanged(object sender, BuilderProgress e)
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
                        Host.CurrentWindow.ShowNotification(new GUIContent("Nice! Patch from " + _versions[_patchesIndex1] + " to " + _versions[_patchesIndex2] + " has been successfully processed!"));
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
                        Host.CurrentWindow.ShowNotification(new GUIContent("Ooops! Patch from " + _versions[_patchesIndex1] + " to " + _versions[_patchesIndex2] + " triggered an error! Check the console!"));
                        Debug.Log(_currentError);

                        _currentError = null;
                        _isStopped = true;
                        _isTaskStarted = false;
                    }
                }
                else
                {
                    EditorUtility.DisplayProgressBar("PATCH - Patch creator", _buildingLog, _progress);
                }
            }
        }

        private void InitializePatches()
        {
            RefreshPatches();
            
            if (_versions.Length > 1)
            {
                _patchesIndex1 = _versions.Length - 2;
                _patchesIndex2 = _versions.Length - 1;
            }
            else
            {
                _patchesIndex1 = 0;
                _patchesIndex2 = 0;
            }
        }
        
        private void RefreshPatches()
        {
            _versions = _context.GetVersions().Select(v => v.ToString()).ToArray();
        }

        public override void OnShow()
        {
            base.OnShow();
            InitializePatches();
        }
        
        private void UpdateAfterTicks()
        {
            if (_skippedTicks >= 10)
            {
                _skippedTicks = 0;

                RefreshPatches();
            }

            _skippedTicks++;
        }

        public override void Update()
        {
            base.Update();

            UpdateProgress();
            UpdateAfterTicks();
        }

        public override void UpdateUISize()
        {
            base.UpdateUISize();

            var height = ContentArea.position.y;

            _helpBoxArea = new Rect(ContentArea.position.x, ContentArea.position.y, ContentArea.width, 30);
            _patchesDescriptionArea = new Rect(ContentArea.position.x, height, ContentArea.width * 8 / 12, 40);
            _patchesArea1 = new Rect(ContentArea.position.x + ContentArea.width * 8 / 12, height, ContentArea.width * 2 / 12 - 10, 30);
            _patchesArea2 = new Rect(ContentArea.position.x + ContentArea.width * 10 / 12 + 10, height, ContentArea.width * 2 / 12 - 10, 30);
            
            height += 40;
            
            _compressionTextArea = new Rect(ContentArea.position.x, height, ContentArea.width * 9 / 12, 40);
            _compressionSelectorArea = new Rect(ContentArea.position.x + ContentArea.width * 9 / 12, height, ContentArea.width * 3 / 12, 25);

            height += 40 + _margin;

            _patchButtonArea = new Rect(ContentArea.position.x, height, ContentArea.width, 40);
        }

        public override void Render()
        {
            base.Render();
            _scrollPosition = GUI.BeginScrollView(ContentArea, _scrollPosition, ContentArea);


            if (_versions.Length > 1)
            {
                GUI.Label(_patchesDescriptionArea,
                    "Patches<i><size=9><color=#808080ff> - This allows you to generate a patch between selected builds.</color></size></i>");

                _patchesIndex1 = EditorGUI.Popup(_patchesArea1, _patchesIndex1, _versions);
                _patchesIndex2 = EditorGUI.Popup(_patchesArea2, _patchesIndex2, _versions);
                
                GUI.Label(_compressionTextArea, "Compression Level<i><size=9><color=#808080ff> - Sets the compression level for the patch archive.</color></size></i>");
                _compressionValue = EditorGUI.IntSlider(_compressionSelectorArea, _compressionValue, 1, 9);

                if (!_isTaskStarted)
                {
                    if (GUI.Button(_patchButtonArea, "Build new patch"))
                    {
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
            else
            {
                EditorGUI.HelpBox(_helpBoxArea, "Hey! You need at least two builds to create a patch. Read the doc if you are not sure how to do it!", MessageType.Warning);
            }

            GUI.EndScrollView();
        }

        private void TriggerBuilder()
        {
            _context.VersionFrom = _context.VersionFactory.Parse(_versions[_patchesIndex1]);
            _context.VersionTo = _context.VersionFactory.Parse(_versions[_patchesIndex2]);
            _context.CompressionLevel = _compressionValue;
            
            _context.Initialize();
            _builder.Build();
        }
    }
}
