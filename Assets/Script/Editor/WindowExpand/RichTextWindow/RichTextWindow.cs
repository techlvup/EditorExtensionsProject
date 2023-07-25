using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

public class RichTextWindow : EditorWindow
{
    private Text m_textMain = null;
    private StringBuilder m_textMainDes = null;
    private RichTextData m_richTextDataMain = null;
    private List<RichTextData> m_allTextDataList = null;
    private Vector2 m_scrollPos = Vector2.zero;

    [MenuItem("MyExtensions/MyWindows/富文本工具")]
    public static void ShowRichTextWindow()
    {
        EditorWindow window = GetWindow<RichTextWindow>("富文本工具");
        window.minSize = new Vector2(837, 520);
        window.maxSize = new Vector2(857, 550);
        window.Show();
    }

    private void Awake()
    {
        if (Selection.gameObjects.Length == 1)
        {
            GameObject go = Selection.gameObjects[0];
            m_textMain = go.GetComponent<Text>();
        }
       
        if(m_textMain == null)
        {
            GameObject goCanvas = new GameObject("MyCanvas");
            Canvas canvas = goCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            GameObject goText = new GameObject("MyTestText", typeof(RectTransform));
            goText.transform.SetParent(goCanvas.transform);
            
            RectTransform rectTransform = goText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(500, 500);
            rectTransform.anchoredPosition = new Vector2(0, 0);

            m_textMain = goText.AddComponent<Text>();
        }

        Selection.activeTransform = m_textMain.transform;

        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.FrameSelected();
        }

        m_textMainDes = new StringBuilder();
        m_allTextDataList = new List<RichTextData>();
        m_richTextDataMain = new RichTextData();

        m_allTextDataList.Add(m_richTextDataMain);
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        SetMainText();

        ShowScrollView();

        ShowDownUI();

        SetTextDes();
    }

    private void OnDestroy()
    {
        GameObject.DestroyImmediate(m_textMain.transform.parent.gameObject);
        m_textMain = null;
        m_textMainDes = null;
        m_richTextDataMain = null;
        m_allTextDataList = null;
        m_scrollPos = Vector2.zero;
    }

    private void SetMainText()
    {
        GUILayout.Space(8);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("预览文本", GUILayout.Width(50));
        m_textMain = (Text)EditorGUILayout.ObjectField(m_textMain, typeof(Text), true, GUILayout.Width(150));

        GUILayout.Space(10);

        EditorGUILayout.LabelField("字号", GUILayout.Width(30));
        m_richTextDataMain.fontSize = EditorGUILayout.IntField(m_richTextDataMain.fontSize, GUILayout.Width(30));

        GUILayout.Space(10);

        EditorGUILayout.LabelField("颜色", GUILayout.Width(30));
        m_richTextDataMain.color = EditorGUILayout.ColorField(m_richTextDataMain.color, GUILayout.Width(100));

        GUILayout.Space(10);

        EditorGUILayout.LabelField("字体格式", GUILayout.Width(50));
        m_richTextDataMain.fontStyle = (FontStyle)EditorGUILayout.EnumPopup(m_richTextDataMain.fontStyle, GUILayout.Width(100));

        GUILayout.Space(10);

        EditorGUILayout.LabelField("对齐方式", GUILayout.Width(50));
        m_richTextDataMain.align = (TextAnchor)EditorGUILayout.EnumPopup(m_richTextDataMain.align, GUILayout.Width(100));

        GUILayout.Space(10);

        if (GUILayout.Button("聚焦", GUILayout.Width(50)))
        {
            Selection.activeTransform = m_textMain.transform;

            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.FrameSelected();
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("==================================================================================================================================================================================================================================================================================================");
    }

    private void ShowScrollView()
    {
        GUILayout.Space(5);

        EditorGUILayout.LabelField("添加文本内容如下：");

        GUILayout.Space(5);

        m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.Height(position.height - 125));

        if (m_allTextDataList.Count > 1)
        {
            for (int i = 1; i < m_allTextDataList.Count; i++)
            {
                RichTextData richTextData = m_allTextDataList[i];

                EditorGUILayout.LabelField("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("<color=#FFFFFFFF>第<color=#FFED00FF>" + i + "</color>个添加的文本内容：</color>", richTextData.gUIStyle, GUILayout.Width(120));

                richTextData.text = EditorGUILayout.TextField(richTextData.text);

                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    RichTextData tempData = new RichTextData();
                    m_allTextDataList.Insert(i + 1, tempData);
                }

                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    m_allTextDataList.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();

                richTextData.isSetFontSize = GUILayout.Toggle(richTextData.isSetFontSize, "是否设置字号", GUILayout.Width(200));

                if(richTextData.isSetFontSize)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("字号", GUILayout.Width(30));
                    richTextData.fontSize = EditorGUILayout.IntField(richTextData.fontSize, GUILayout.Width(30));

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();

                richTextData.isSetColor = GUILayout.Toggle(richTextData.isSetColor, "是否设置颜色", GUILayout.Width(200));

                if (richTextData.isSetColor)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("颜色", GUILayout.Width(30));
                    richTextData.color = EditorGUILayout.ColorField(richTextData.color, GUILayout.Width(100));

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();

                richTextData.isSetFontStyle = GUILayout.Toggle(richTextData.isSetFontStyle, "是否设置字体格式", GUILayout.Width(200));

                if (richTextData.isSetFontStyle)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("字体格式", GUILayout.Width(50));
                    richTextData.fontStyle = (FontStyle)EditorGUILayout.EnumPopup(richTextData.fontStyle, GUILayout.Width(100));

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            }
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(5);

        EditorGUILayout.LabelField("=============================================================================================================================================================================================================================================================================================");
    }

    private void ShowDownUI()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("添加", GUILayout.Width(50)))
        {
            RichTextData richTextData = new RichTextData();
            m_allTextDataList.Add(richTextData);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("删除全部", GUILayout.Width(100)))
        {
            m_allTextDataList.Clear();
            m_allTextDataList.Add(m_richTextDataMain);
        }

        GUILayout.Space(10);

        EditorGUILayout.LabelField("显示内容", GUILayout.Width(50));

        m_richTextDataMain.text = GetCurrTextDes();
        EditorGUILayout.TextField(m_richTextDataMain.text, GUILayout.Width(500));

        if (GUILayout.Button("复制", GUILayout.Width(50)))
        {
            GUIUtility.systemCopyBuffer = m_richTextDataMain.text;
        }

        EditorGUILayout.EndHorizontal();
    }

    private string GetCurrTextDes()
    {
        m_textMainDes.Remove(0, m_textMainDes.Length);

        if (m_allTextDataList.Count > 1)
        {
            for (int i = 1; i < m_allTextDataList.Count; i++)
            {
                RichTextData richTextData = m_allTextDataList[i];

                string des = richTextData.text;

                if(richTextData.isSetFontStyle && richTextData.fontStyle != m_richTextDataMain.fontStyle)
                {
                    des = SetFontStyle(des, richTextData.fontStyle);
                }

                if (richTextData.isSetColor && richTextData.color != m_richTextDataMain.color)
                {
                    des = SetTextColor(des, richTextData.color);
                }

                if (richTextData.isSetFontSize && richTextData.fontSize != m_richTextDataMain.fontSize)
                {
                    des = SetFontSize(des, richTextData.fontSize);
                }

                m_textMainDes.Append(des);
            }
        }

        return m_textMainDes.ToString();
    }

    private string SetFontStyle(string text, FontStyle fontStyle)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }

        if (fontStyle == FontStyle.Bold)
        {
            return string.Format("<b>{0}</b>", text);
        }
        else if(fontStyle == FontStyle.Italic)
        {
            return string.Format("<i>{0}</i>", text);
        }
        else if(fontStyle == FontStyle.BoldAndItalic)
        {
            return string.Format("<i><b>{0}</b></i>", text);
        }
        else
        {
            return text;
        }
    }

    private string SetFontSize(string text, int fontSize)
    {
        if (string.IsNullOrEmpty(text) || fontSize <= 0)
        {
            return "";
        }

        return string.Format("<size={0}>{1}</size>", fontSize, text);
    }

    private string SetTextColor(string text, Color color)
    {
        if(string.IsNullOrEmpty(text))
        {
            return "";
        }

        return string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color), text);
    }

    private void SetTextDes()
    {
        m_textMain.fontSize = m_richTextDataMain.fontSize;
        m_textMain.fontStyle = m_richTextDataMain.fontStyle;
        m_textMain.color = m_richTextDataMain.color;
        m_textMain.alignment = m_richTextDataMain.align;

        if(m_textMain.fontSize <= 0)
        {
            m_textMain.text = "";
        }
        else
        {
            m_textMain.text = m_richTextDataMain.text;
        }
    }
}