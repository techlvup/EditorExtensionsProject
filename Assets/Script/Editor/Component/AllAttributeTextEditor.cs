/// <summary>
/// 绘制AllAttributeText组件Inspector面板的GUI的实现脚本
/// </summary>
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CanEditMultipleObjects] //如果有绘制的GUI则需要在该脚本加上该特性后，才能在Inspector面板上同时更改多个物体AllAttributeText组件的相同属性
[CustomEditor(typeof(AllAttributeText))] //加上该特性并继承Editor后，则注册为AllAttributeText组件的Inspector面板编辑脚本
public class AllAttributeTextEditor : Editor
{
    private AllAttributeText m_component;

    private ReorderableList m_CanSortList;

    private void Awake()
    {
        m_component = (AllAttributeText)target;

        m_CanSortList = new ReorderableList(serializedObject, serializedObject.FindProperty("CanSortList"));

        InitReorderableList();

        AddButtonClick();
    }

    public override void OnInspectorGUI()
    {
        Debug.Log("在渲染UI的时候每0.05秒调用一次，每次渲染都会先清空内容再渲染");

        DrawDefaultInspector();

        DrawMyGUI();
    }

    private void InitReorderableList()
    {
        //rect参数为传递过来的单行元素矩形信息，rect.x为最左边的值
        //给绘制列表头部的委托事件drawHeaderCallback赋值
        m_CanSortList.drawHeaderCallback = (rect) =>
        {
            EditorGUI.LabelField(new Rect(rect.x + 30, rect.y, rect.width, rect.height), "String");
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 120, rect.y, rect.width, rect.height), "Vector3");
        };

        //给绘制列表中每个元素的委托事件drawElementCallback赋值
        m_CanSortList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            SerializedProperty element = m_CanSortList.serializedProperty.GetArrayElementAtIndex(index);

            Rect nameRect = new Rect(rect.x, rect.y + 2, 80, rect.height - 4);
            Rect positionRect = new Rect(rect.x + rect.width - 200, rect.y + 2, 200, rect.height - 4);

            element.FindPropertyRelative("Name").stringValue = EditorGUI.TextField(nameRect, element.FindPropertyRelative("Name").stringValue);

            element.FindPropertyRelative("Position").vector3Value = EditorGUI.Vector3Field(positionRect, GUIContent.none, element.FindPropertyRelative("Position").vector3Value);
        };
    }

    private void AddButtonClick()
    {
        //onAddDropdownCallback定义点击列表下面的[+]按钮时发生的事件
        m_CanSortList.onAddDropdownCallback = (buttonRect, reorderableList) =>
        {
            if (m_component.NormalList == null || m_component.NormalList.Count == 0)
            {
                EditorApplication.Beep();
                EditorUtility.DisplayDialog("错误", "菜单中没有选项", "返回 true", "返回 false");
                return;
            }

            GenericMenu menu = new GenericMenu();

            foreach (var item in m_component.NormalList)
            {
                menu.AddItem(new GUIContent(item.Name), false, AddItemClick, item);
            }

            menu.ShowAsContext();
        };
    }

    //这个回调函数会在用户选择了[+]下拉菜单中的某一项后调用
    private void AddItemClick(object state)
    {
        MyStateList item = (MyStateList)state;

        int index = m_CanSortList.serializedProperty.arraySize;

        m_CanSortList.serializedProperty.arraySize++;

        m_CanSortList.index = index;

        SerializedProperty element = m_CanSortList.serializedProperty.GetArrayElementAtIndex(index);

        element.FindPropertyRelative("Name").stringValue = item.Name;
        element.FindPropertyRelative("Position").vector3Value = item.Position;

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawMyGUI()
    {
        GUILayout.Space(20);

        GUILayout.Label("NormalList", EditorStyles.boldLabel);

        DrawNormalList();

        GUILayout.Space(10);

        DrawStatesButton();

        DrawCanSortList();
    }

    private void DrawNormalList()
    {
        for (int index = 0; index < m_component.NormalList.Count; index++)
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            m_component.NormalList[index].Position = EditorGUILayout.Vector3Field(m_component.NormalList[index].Name, m_component.NormalList[index].Position);

            if (GUILayout.Button("Remove", GUILayout.Width(80), GUILayout.Height(18)))
            {
                m_component.NormalList.RemoveAt(index);
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawStatesButton()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add"))
        {
            MyStateList myStateList = new MyStateList();

            if (m_component.NormalList.Count > 0)
            {
                string id = "";

                for (int i = 5; i < m_component.NormalList[m_component.NormalList.Count - 1].Name.Length; i++)
                {
                    id = id + m_component.NormalList[m_component.NormalList.Count - 1].Name[i];
                }

                int intId = int.Parse(id) + 1;

                myStateList.Name = "Name_" + intId;
                myStateList.Position = new Vector3(intId, intId, intId);
            }
            else
            {
                myStateList.Name = "Name_1";
                myStateList.Position = new Vector3(1, 1, 1);
            }

            m_component.NormalList.Add(myStateList);
        }

        if (m_component.NormalList.Count > 0)
        {
            if (GUILayout.Button("RemoveAll", GUILayout.Width(100), GUILayout.Height(18)))
            {
                m_component.NormalList.Clear();
            }
        }

        GUILayout.EndHorizontal();
    }

    private void DrawCanSortList()
    {
        GUILayout.Space(10);

        GUILayout.Label("CanSortList", EditorStyles.boldLabel);

        m_CanSortList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}