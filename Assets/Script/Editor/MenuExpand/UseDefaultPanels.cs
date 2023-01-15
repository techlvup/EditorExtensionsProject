using UnityEditor;
using UnityEngine;
using System.Threading;

public class UseDefaultPanels
{
    [MenuItem("MyTest/ShowDefaultPanels/DisplayProgressBar")]
    public static void DisplayProgressBar()
    {
        for (int i = 0; i < 100; i++)
        {
            EditorUtility.DisplayProgressBar("我的进度条", "显示的进度", i * 1.0f / 100);
            Thread.Sleep(1);//线程暂停多少毫秒
        }

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("MyTest/ShowDefaultPanels/DisplayCancelableProgressBar")]
    public static void DisplayCancelableProgressBar()
    {
        for (int i = 0; i < 100; i++)
        {
            EditorUtility.DisplayCancelableProgressBar("我的进度条(可取消)", "显示的进度", i * 1.0f / 100);
            Thread.Sleep(1);//线程暂停多少毫秒
        }

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("MyTest/ShowDefaultPanels/DisplayDialog")]
    public static void DisplayDialog()
    {
        Debug.Log(100);

        bool result = EditorUtility.DisplayDialog("我的对话框", "对话框信息，返回对应按钮的bool值，直接叉掉会返回最后一个按钮的bool值", "返回true", "返回false");

        Debug.Log(result);
    }

    [MenuItem("MyTest/ShowDefaultPanels/DisplayDialogComplex")]
    public static void DisplayDialogComplex()
    {
        Debug.Log(100);

        int result = EditorUtility.DisplayDialogComplex("我的复杂对话框", "对话框信息，返回对应按钮的int值，直接叉掉会返回最后一个按钮的int值", "返回0", "返回1", "返回2");

        Debug.Log(result);
    }

    [MenuItem("MyTest/ShowDefaultPanels/DisplayPopupMenu")]
    public static void DisplayPopupMenu()
    {
        Rect rect = new Rect(10, 10, 100, 100);
        EditorUtility.DisplayPopupMenu(rect, "Assets/", null);//弹出某路径下的菜单
    }
}