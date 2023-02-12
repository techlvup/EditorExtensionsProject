using UnityEngine;
using UnityEditor;

public class MyMenuExpand
{
    //编辑器上方的菜单栏按钮
    [MenuItem("MyTest/AddAttriubteComponent")]
    public static void AddAttriubteComponent()
    {
        if (Selection.objects.Length > 0)
        {
            foreach (var item in Selection.objects)
            {
                GameObject gob = item as GameObject;

                if (gob.GetComponent<AllAttributeText>() == null)
                {
                    gob.AddComponent<AllAttributeText>();
                }
            }
        }
    }

    //AllAttributeText组件的右上角齿轮展示出的菜单选项
    [MenuItem("CONTEXT/AllAttributeText/AddBoxCollider")]
    public static void AddBoxCollider()
    {
        if (Selection.objects.Length > 0)
        {
            foreach (var item in Selection.objects)
            {
                GameObject gob = item as GameObject;

                if (gob.GetComponent<BoxCollider>() == null)
                {
                    gob.AddComponent<BoxCollider>();
                }
                else
                {
                    Undo.DestroyObjectImmediate(gob);
                }
            }
        }
    }

    //编辑器上方的菜单栏按钮
    [MenuItem("GameObject/MyPrefabs/CreateAllAttributeText")]
    public static void CreateAllAttributeText()
    {
        GameObject go = new GameObject("AllAttributeText");

        go.AddComponent<AllAttributeText>();
    }

    //使用了该特性的静态函数会在Unity工程加载时自动执行，不需要用户做任何操作
    [InitializeOnLoadMethod]
    private static void SetDrawHierarchyMenuMode()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        if (Event.current != null && Event.current.button == 1 && Event.current.type == EventType.MouseUp)
        {
            Vector2 mousePosition = Event.current.mousePosition;

            EditorUtility.DisplayPopupMenu(new Rect(mousePosition.x, mousePosition.y, 0, 0), "GameObject/", null);
            Event.current.Use();
        }
    }
}