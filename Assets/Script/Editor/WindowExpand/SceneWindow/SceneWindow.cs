using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public class SceneWindow : SceneView
{
    private Rect m_leftLineArea;
    private Rect m_middleArea;
    private Rect m_rightLineArea;
    private bool m_isMoveLeft;
    private bool m_isMovRight;
    private Camera m_camera;
    private Dictionary<string, Texture> m_textures;
    private string m_searchName;
    private GUISkin m_skin;
    private CameraClearFlags m_clearFlags;
    private Color m_backgroundColor;
    private Vector2 scrollPosition = Vector2.zero;



    [MenuItem("MyWindow/MyWindows/场景窗口")]
    public static void DisplaySceneWindow()
    {
        EditorWindow window = GetWindow<SceneWindow>("场景窗口");
        window.minSize = new Vector2(1200, 800);
        window.Show();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        m_middleArea = new Rect(400, 10, 400, position.height - 10);
        m_isMoveLeft = false;
        m_isMovRight = false;

        m_camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        m_clearFlags = m_camera.clearFlags;
        m_backgroundColor = m_camera.backgroundColor;

        m_camera.clearFlags = CameraClearFlags.SolidColor;
        m_camera.backgroundColor = Color.magenta;

        m_textures = new Dictionary<string, Texture>();
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    protected override void OnGUI()
    {
        base.OnGUI();

        if (m_skin == null)
        {
            m_skin = GUI.skin;
        }

        if (m_middleArea.height != position.height - 10)
        {
            m_middleArea.height = position.height - 10;
        }

        if (m_textures.Count <= 0)
        {
            for (int i = 1; i <= 6; i++)
            {
                if (!m_textures.ContainsKey("cube" + i))
                {
                    Vector3 pos = GameObject.Find("cube" + i).transform.position;

                    m_camera.transform.position = new Vector3(pos.x, pos.y + 2, pos.z + 3);

                    m_camera.transform.LookAt(pos);

                    RenderTexture renderTexture = new RenderTexture(150, 150, 24);

                    m_camera.targetTexture = renderTexture;

                    m_camera.Render();

                    m_textures.Add(GameObject.Find("cube" + i).GetComponent<Renderer>().sharedMaterial.name.Replace(" (Instance)", ""), renderTexture);
                }
            }
        }

        GUILayout.BeginHorizontal();

        DrawLeftArea();

        DrawMiddleArea();

        DrawRightArea();

        GUILayout.EndHorizontal();

        ReceiveViewEvent();
    }

    public override void OnDisable()
    {
        base.OnDisable();

        m_textures.Clear();

        m_camera.clearFlags = m_clearFlags;
        m_camera.backgroundColor = m_backgroundColor;

        scrollPosition = Vector2.zero;
    }

    new public void OnDestroy()
    {
        base.OnDestroy();
    }



    private void DrawLeftArea()
    {
        Rect leftArea = new Rect(0, 15, m_middleArea.x - 10, m_middleArea.height);
        GUILayout.BeginArea(leftArea, m_skin.GetStyle("button"));

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        m_searchName = GUILayout.TextField(m_searchName, m_skin.GetStyle("ToolbarSeachTextField"));

        if (GUILayout.Button("", m_skin.GetStyle("ToolbarSeachCancelButton")))
        {
            m_searchName = string.Empty;
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (m_textures.Count > 0)
        {
            int index = 1;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (var item in m_textures)
            {
                if(string.IsNullOrEmpty(m_searchName) || item.Key.Contains(m_searchName))
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(leftArea.width / 2 - 110);

                    GUILayout.Box("", m_skin.GetStyle("NotificationBackground"), GUILayout.Width(200), GUILayout.Height(200));

                    Rect rect = GUILayoutUtility.GetLastRect();

                    GUILayout.EndHorizontal();

                    if(rect.x != 0 && rect.y != 0)
                    {
                        GUI.DrawTexture(new Rect(rect.x + 25, rect.y + 10, 150, 150), item.Value);

                        GUI.Label(new Rect(rect.x + 80, rect.y + 170, 50, 20), item.Key);
                    }

                    index++;
                }
            }

            GUILayout.EndScrollView();
        }

        GUILayout.EndArea();

        GUILayout.Box("", GUIStyle.none, GUILayout.Width(1), GUILayout.ExpandHeight(true));

        Rect leftLineArea = new Rect(m_middleArea.x - 10, 15, 10, m_middleArea.height);

        if (m_leftLineArea != leftLineArea)
        {
            m_leftLineArea = leftLineArea;
        }
    }

    private void DrawMiddleArea()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginArea(m_middleArea);



        GUILayout.EndArea();

        GUILayout.EndVertical();
    }

    private void DrawRightArea()
    {
        GUILayout.Box("", GUIStyle.none, GUILayout.Width(1), GUILayout.ExpandHeight(true));

        Rect rightLineArea = new Rect(m_middleArea.x + m_middleArea.width, 15, 10, m_middleArea.height);

        if (m_rightLineArea != rightLineArea)
        {
            m_rightLineArea = rightLineArea;
        }

        Rect rightArea = new Rect(m_middleArea.x + m_middleArea.width + 10, 15, position.width - m_middleArea.x - m_middleArea.width - 10, m_middleArea.height);

        GUILayout.BeginArea(rightArea, m_skin.GetStyle("button"));

        GUILayout.BeginVertical();

        GUILayout.EndVertical();

        GUILayout.EndArea();
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
                        else if (m_rightLineArea.Contains(currEvent.mousePosition))
                        {
                            m_isMovRight = true;
                        }
                    }
                }
                break;

            case EventType.MouseDrag:
                {
                    if (btnType == 0)
                    {
                        if (currEvent.delta.x != 0)
                        {
                            if(m_isMoveLeft)
                            {
                                float result = m_middleArea.x + currEvent.delta.x;

                                if (result <= m_rightLineArea.x - 300 && result >= 300)
                                {
                                    m_middleArea.x += currEvent.delta.x;
                                    m_middleArea.width -= currEvent.delta.x;
                                }
                            }
                            else if(m_isMovRight)
                            {
                                float result1 = position.width - m_middleArea.x - m_middleArea.width - currEvent.delta.x - 10;
                                float result2 = m_middleArea.width + currEvent.delta.x;

                                if (result1 >= 300 && result2 >= 300)
                                {
                                    m_middleArea.width += currEvent.delta.x;
                                }
                            }
                        }
                    }
                }
                break;

            case EventType.MouseUp:
                {
                    if (btnType == 0)
                    {
                        m_isMoveLeft = false;
                        m_isMovRight = false;
                    }
                }
                break;
        }

        if (Event.current.type == EventType.Repaint)
        {
            if (m_leftLineArea.Contains(currEvent.mousePosition))
            {
                EditorGUIUtility.AddCursorRect(m_leftLineArea, MouseCursor.ResizeHorizontal);
            }
            else if(m_rightLineArea.Contains(currEvent.mousePosition))
            {
                EditorGUIUtility.AddCursorRect(m_rightLineArea, MouseCursor.ResizeHorizontal);
            }
        }
    }

    private void InvokeMethod(string name, object[] param)
    {
        MethodInfo method = typeof(SceneView).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);

        if (method != null)
        {
            method.Invoke(this, param);
        }
    }
}