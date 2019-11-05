using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HSMTree
{
    public class HSMDrawProperty
    {
        private HSMFileHandleController _fileHandleController;
        private HSMPlayController _playController;
        private HSMPropertyOption _propertyOption;
        private HSMDescriptController _descriptController;
        private HSMNodeInspector _nodeInspector;
        private HSMParameterController _parameterController;
        private HSMRuntimeParameter _runtimeParameter;

        public void Init()
        {
            _fileHandleController = new HSMFileHandleController();
            _playController = new HSMPlayController();
            _propertyOption = new HSMPropertyOption();
            _descriptController = new HSMDescriptController();
            _nodeInspector = new HSMNodeInspector();
            _parameterController = new HSMParameterController();
            _runtimeParameter = new HSMRuntimeParameter();
        }

        public void OnDestroy()
        {
            _fileHandleController.OnDestroy();
            _playController.OnDestroy();
            _nodeInspector.OnDestroy();
            _parameterController.OnDestroy();
            _runtimeParameter.OnDestroy();
        }

        public void OnGUI()
        {
            _fileHandleController.OnGUI();
            GUILayout.Space(8);

            _playController.OnGUI();
            GUILayout.Space(8);

            int option = _propertyOption.OnGUI();
            if (option == 0)
            {
                _descriptController.OnGUI();
            }
            else if (option == 1)
            {
                _nodeInspector.OnGUI();
            }
            else if (option == 2)
            {
                if (HSMManager.Instance.PlayType == HSMPlayType.PLAY
                    || HSMManager.Instance.PlayType == HSMPlayType.PAUSE)
                {
                    _runtimeParameter.OnGUI();
                }
                else
                {
                    _parameterController.OnGUI();
                }
            }
        }
    }

    public class HSMPropertyOption
    {
        private int option = 1;
        private readonly string[] optionArr = new string[] { "Descript", "Inspect", "Parameter" };

        public int OnGUI()
        {
            int index = option;
            option = GUILayout.Toolbar(option, optionArr, EditorStyles.toolbarButton);
            return option;
        }
    }
}


