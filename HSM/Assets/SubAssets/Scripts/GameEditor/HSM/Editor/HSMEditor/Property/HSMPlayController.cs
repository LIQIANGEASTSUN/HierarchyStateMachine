using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HSMTree
{
    public enum HSMPlayType
    {
        INVALID = -1,
        PLAY    = 0,
        PAUSE   = 1,
        STOP    = 2,
    }

    public class HSMPlayController
    {
        private HSMPlayModel _playModel;
        private HSMPlayView _playView;

        public HSMPlayController()
        {
            Init();
        }

        private void Init()
        {
            _playModel = new HSMPlayModel();
            _playView = new HSMPlayView();
        }

        public void OnDestroy()
        {

        }

        public void OnGUI()
        {
            _playView.Draw();
        }
        
    }

    public class HSMPlayModel
    {

        public HSMPlayModel()
        {

        }

    }

    public class HSMPlayView
    {
        private int option = 2;
        private readonly string[] optionArr = new string[] { "Play", "Pause", "Stop"};
        public HSMPlayView()
        {

        }

        public void Draw()
        {
            EditorGUILayout.BeginHorizontal("box");
            {
                int index = option;
                option = GUILayout.Toolbar(option, optionArr, EditorStyles.toolbarButton);
                if (index != option)
                {
                    if (null != HSMManager.hSMRuntimePlay)
                    {
                        HSMManager.hSMRuntimePlay((HSMPlayType)option);
                    }
                }

            }
            EditorGUILayout.EndHorizontal();
        }
    }

}


