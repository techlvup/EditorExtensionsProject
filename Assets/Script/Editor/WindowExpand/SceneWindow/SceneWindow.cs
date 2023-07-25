using UnityEngine;
using UnityEditor;
using System.Collections.Generic;



public class SceneWindow : SceneView
{
    private Dictionary<string, Texture> m_textures;
    private string m_searchName;
    private GUISkin m_skin;

    private Camera m_camera;
    private CameraClearFlags m_clearFlags;
    private Color m_backgroundColor;

    private Vector2 scrollPosition = Vector2.zero;

    private bool m_isShowLeft;



    [MenuItem("MyExtensions/MyWindows/场景窗口")]
    public static void DisplaySceneWindow()
    {
        EditorWindow window = GetWindow<SceneWindow>("场景窗口");
        window.minSize = new Vector2(1250, 800);
        window.Show();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        m_camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        m_clearFlags = m_camera.clearFlags;
        m_backgroundColor = m_camera.backgroundColor;

        m_camera.clearFlags = CameraClearFlags.SolidColor;
        m_camera.backgroundColor = Color.magenta;

        m_textures = new Dictionary<string, Texture>();

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

        GUILayout.BeginHorizontal();

        GUILayout.Space(10);

        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();

        GUILayout.Space(5);

        m_isShowLeft = GUILayout.Toggle(m_isShowLeft, m_isShowLeft ? "隐藏场景Cube图片" : "展示场景Cube图片", m_skin.GetStyle("button"), GUILayout.Width(150), GUILayout.Height(50));

        if (m_isShowLeft)
        {
            DrawLeftArea();
        }

        GUILayout.EndVertical();
    }

    public override void OnDisable()
    {
        base.OnDisable();

        m_textures.Clear();

        m_camera.clearFlags = m_clearFlags;
        m_camera.backgroundColor = m_backgroundColor;

        scrollPosition = Vector2.zero;

        m_isShowLeft = false;
    }

    new public void OnDestroy()
    {
        base.OnDestroy();
    }



    private void DrawLeftArea()
    {
        Rect leftArea = new Rect(10, 80, 400, position.height - 100);
        GUILayout.BeginArea(leftArea, m_skin.GetStyle("NotificationBackground"));

        GUILayout.BeginHorizontal();

        GUILayout.Space(leftArea.width / 2 - 60);

        GUILayout.Label("搜索", new GUIStyle(GUI.skin.label) { fontSize = 20 }, GUILayout.Width(100), GUILayout.Height(30));

        GUILayout.EndHorizontal();

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
                if (string.IsNullOrEmpty(m_searchName) || item.Key.Contains(m_searchName))
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(leftArea.width / 2 - 150);

                    if (GUILayout.Button("", m_skin.GetStyle("button"), GUILayout.Width(200), GUILayout.Height(200)))
                    {
                        Selection.activeTransform = GameObject.Find("cube" + index).transform;
                        FrameSelected();
                    }

                    Rect rect = GUILayoutUtility.GetLastRect();

                    GUILayout.EndHorizontal();

                    if (rect.x != 0 && rect.y != 0)
                    {
                        GUI.DrawTexture(new Rect(rect.x + 25, rect.y + 10, 150, 150), item.Value);

                        GUI.Label(new Rect(rect.x + 80, rect.y + 170, 50, 20), item.Key);
                    }

                    if (index < m_textures.Count)
                    {
                        GUILayout.Space(10);
                    }

                    index++;
                }
            }

            GUILayout.EndScrollView();
        }

        GUILayout.EndArea();
    }
}