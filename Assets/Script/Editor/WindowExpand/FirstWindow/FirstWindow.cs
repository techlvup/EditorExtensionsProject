using UnityEngine;
using UnityEditor;

public class FirstWindow : EditorWindow
{
    private string str1;
    private string str2 = "aaaaaaaaa";
    private bool isOn_1 = true;
    private bool isOn_2 = true;
    private float slider_1 = 5;
    private Object obj = null;
    private TextAnchor m_alignment = TextAnchor.MiddleCenter;
    private Vector2 scrollPos = Vector2.zero;


    [MenuItem("MyExtensions/MyWindows/第一个窗口")]  //添加菜单选项
    public static void ShowFirstWindow()
    {
        EditorWindow window = GetWindow<FirstWindow>("第一个窗口");
        window.minSize = new Vector2(300, 300);
        window.maxSize = new Vector2(500, 500);
        window.Show();
    }

    /// <summary>
    /// 窗口的生命周期事件
    /// </summary>
    private void Awake()
    {
        Debug.Log("当打开窗口的时候调用一次");
    }

    private void OnEnable()
    {
        Debug.Log("当显示窗口的时候调用一次");
    }

    private void OnFocus()
    {
        Debug.Log("当窗口变为选中状态时调用");
    }

    private void OnInspectorUpdate()
    {
        Debug.Log("当属性界面更新时每0.1秒调用一次，几乎一直在更新");

        //开启重绘实时渲染(避免OnGUI函数内渲染的UI刷新不及时)
        Repaint();
    }

    private void OnGUI()
    {
        Debug.Log("在渲染UI(即操作窗体或改变窗体选中状态)的时候每0.05秒调用一次，每次渲染都会先清空内容再渲染");

        //创建并设置输入框内容,会返回输入框内容
        str1 = EditorGUILayout.TextField("str_1", str1);

        //创建按钮，如果存在该按钮并点击则返回true
        if (GUILayout.Button("Button_1"))
        {
            Debug.Log("点击Button_1");
        }

        //创建选项框并设置其是否选中，会返回其选中状态
        isOn_1 = EditorGUILayout.BeginToggleGroup("toggle_1", isOn_1);

        str2 = EditorGUILayout.TextField("str_2", str2);

        //在BeginToggleGroup与EndToggleGroup中间的字段可被toggle_1控制其是否启用
        EditorGUILayout.EndToggleGroup();

        //创建并设置滑块的值,会返回滑块的值
        slider_1 = EditorGUILayout.Slider(slider_1, 0, 10);

        if (Selection.gameObjects.Length > 0)
        {
            obj = Selection.gameObjects[0];
        }
        else
        {
            obj = null;
        }

        //创建并设置GameObject引用框，会返回对应GameObject的引用
        obj = EditorGUILayout.ObjectField(obj, typeof(GameObject), true);

        //创建滑动列表区域，返回其位置
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        //创建普通toggle，会返回其选中状态
        isOn_2 = EditorGUILayout.Toggle(isOn_2, GUILayout.Height(30));

        EditorGUILayout.Toggle(false, GUILayout.Height(30));

        EditorGUILayout.Toggle(false, GUILayout.Height(30));

        EditorGUILayout.Toggle(false, GUILayout.Height(30));

        EditorGUILayout.Toggle(false, GUILayout.Height(30));

        EditorGUILayout.Toggle(false, GUILayout.Height(30));

        EditorGUILayout.Toggle(true, GUILayout.Height(30));

        //BeginScrollView与EndScrollView中间的字段可显示在滑动列表中
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("聚焦", GUILayout.Width(100)))
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.FrameSelected();
            }
        }

        //创建可弹出的枚举菜单栏
        m_alignment = (TextAnchor)EditorGUILayout.EnumPopup(m_alignment, GUILayout.Width(100));

        EditorGUILayout.EndHorizontal();
    }

    private void OnSelectionChange()
    {
        Debug.Log("当选择发生更改时调用一次，可选项在Project和Hierarchy界面中");
    }

    private void OnLostFocus()
    {
        Debug.Log("当窗口变为未选中状态时调用一次");
    }

    private void OnHierarchyChange()
    {
        Debug.Log("在Hierarchy界面增加、减少、显示、隐藏物体时调用一次");
    }

    private void OnProjectChange()
    {
        Debug.Log("在Project界面删除、增加文件时调用一次");
    }

    private void OnDisable()
    {
        Debug.Log("窗口隐藏时调用一次");
    }

    private void OnDestroy()
    {
        Debug.Log("窗口关闭时调用一次");
    }
}