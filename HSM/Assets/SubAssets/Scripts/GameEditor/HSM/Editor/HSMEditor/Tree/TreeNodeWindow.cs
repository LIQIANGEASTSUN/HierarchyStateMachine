using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using HSMTree;

public class TreeNodeWindow : EditorWindow {
    private HSMDrawProperty _HSMDrawPropertyController;
    private HSMDrawController _HSMDrawController;

    public static TreeNodeWindow window;
    private static Rect windowsPosition = new Rect(10, 30, 1236, 864);
    
    [MenuItem("Window/CreateTree")]
    public static void ShowWindow()
    {
        window = EditorWindow.GetWindow<TreeNodeWindow>();
        window.position = windowsPosition;
        window.autoRepaintOnSceneChange = true;
        window.Show();
    }

    private void OnEnable()
    {
        HSMManager.Instance.Init();

        _HSMDrawPropertyController = new HSMDrawProperty();
        _HSMDrawPropertyController.Init();

        _HSMDrawController = new HSMDrawController();
        _HSMDrawController.Init();

        HSMRunTime.Instance.Init();

        EditorApplication.update += OnFrame;
    }

    private void OnDisable()
    {
        HSMManager.Instance.OnDestroy();
        EditorApplication.update -= OnFrame;

        _HSMDrawController.OnDestroy();
        _HSMDrawPropertyController.OnDestroy();

        HSMRunTime.Instance.OnDestroy();
    }

    private void OnFrame()
    {
        HSMManager.Instance.Update();
        HSMRunTime.Instance.Update();
    }

    private void OnGUI()
    {
        if (null == window)
        {
            return;
        }

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(300), GUILayout.ExpandHeight(true));
            {
                _HSMDrawPropertyController.OnGUI();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                _HSMDrawController.OnGUI(window);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();


        Repaint();
    }

    public void DrawWindow(Action callBack)
    {
        // 开始绘制节点 
        // 注意：必须在  BeginWindows(); 和 EndWindows(); 之间 调用 GUI.Window 才能显示
        BeginWindows();
        {
            if (null != callBack)
            {
                callBack();
            }
        }
        EndWindows();
    }

    public void ShowNotification(string meg)
    {
        ShowNotification(new GUIContent(meg));
        //RemoveNotification();
    }
}