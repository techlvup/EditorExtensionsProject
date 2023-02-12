using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ComplexWindow : EditorWindow
{
    private Rect m_leftArea;
    private Rect m_leftLineArea;
    private bool m_isMoveLeft;

    [MenuItem("MyTest/MyWindows/ComplexWindow")]
    public static void DisplayComplexWindow()
    {
        EditorWindow window = GetWindow<ComplexWindow>("复杂的窗口");
        window.minSize = new Vector2(1200, 800);
        window.Show();
    }

    private void Awake()
    {
        m_leftArea = new Rect(0, 0, 300, position.height);
        m_isMoveLeft = false;
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        DrawLeftArea();

        GUILayout.EndHorizontal();

        ReceiveViewEvent();
    }

    private void OnDestroy()
    {

    }

    private void DrawLeftArea()
    {
        GUILayout.BeginVertical();

        GUILayout.Box("", GUI.skin.GetStyle("WindowBackground"), GUILayout.Width(m_leftArea.width), GUILayout.ExpandHeight(true));

        GUILayout.EndVertical();

        GUILayout.Box("", GUIStyle.none, GUILayout.Width(2), GUILayout.ExpandHeight(true));

        Rect leftLineArea = new Rect(m_leftArea.width - 2, 0, 6, position.height);
        if (m_leftLineArea != leftLineArea)
        {
            m_leftLineArea = leftLineArea;
        }
    }

    private void ReceiveViewEvent()
    {
        Event currEvent = Event.current;
        int btnType = currEvent.button;

        switch (currEvent.type)
        {
            case EventType.MouseDown:
                {
                    if (btnType == 0)
                    {
                        if (m_leftLineArea.Contains(currEvent.mousePosition))
                        {
                            m_isMoveLeft = true;
                        }
                    }
                }
                break;

            case EventType.MouseDrag:
                {
                    if (btnType == 0)
                    {
                        if (m_isMoveLeft && currEvent.delta.x != 0)
                        {
                            m_leftArea.width += currEvent.delta.x;
                        }
                    }
                }
                break;

            case EventType.MouseUp:
                {
                    if (btnType == 0)
                    {
                        m_isMoveLeft = false;
                    }
                }
                break;
        }
    }
}