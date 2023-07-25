using UnityEngine;
using UnityEditor;

public class EditorStyleViewer : EditorWindow
{
    private Vector2 scrollPosition = Vector2.zero;
    private string search = string.Empty;

    [MenuItem("MyExtensions/MyWindows/GUI样式查看器")]
    public static void DisplayEditorStyleViewer()
    {
        EditorWindow window = GetWindow<EditorStyleViewer>("GUI样式查看器");
        window.minSize = new Vector2(800, 800);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal("HelpBox");

        GUILayout.Label("单击示例将复制其名到剪贴板", "label");

        GUILayout.FlexibleSpace();//填充GUILayout容器中未被其他控件占据的剩余空间

        GUILayout.Label("查找:");

        search = EditorGUILayout.TextField(search);

        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach (GUIStyle style in GUI.skin)
        {
            if (style.name.ToLower().Contains(search.ToLower()))
            {
                GUILayout.BeginHorizontal("PopupCurveSwatchBackground");

                GUILayout.Space(10);

                if (GUILayout.Button(style.name, style))
                {
                    EditorGUIUtility.systemCopyBuffer = style.name;
                }

                GUILayout.FlexibleSpace();//填充GUILayout容器中未被其他控件占据的剩余空间

                EditorGUILayout.SelectableLabel("\"" + style.name + "\"");//创建显示用户可以进行选择和复制内容的文本

                GUILayout.EndHorizontal();

                GUILayout.Space(20);
            }
        }

        GUILayout.EndScrollView();
    }
}