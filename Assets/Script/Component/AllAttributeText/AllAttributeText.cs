using System.Collections.Generic;
using UnityEngine;

public class AllAttributeText : MonoBehaviour
{
    [HideInInspector] //public变量在Inspector面板隐藏
    public List<MyStateList> NormalList = new List<MyStateList>();

    [HideInInspector] //public变量在Inspector面板隐藏
    public List<MyStateList> CanSortList = new List<MyStateList>();

    [Range(0, 100)] //限制数值范围
    public float num_1 = 0;

    [ContextMenuItem("Option1", "SetNum2Value1")] //定义属性右键的菜单选项
    [ContextMenuItem("Option2", "SetNum2Value2")] //定义属性右键的菜单选项
    public int num_2 = 0;

    private void SetNum2Value1()
    {
        num_2 = 100;
    }

    private void SetNum2Value2()
    {
        num_2 = 200;
    }


    [Multiline(5)] //设置文本输入框的行数
    public string str_1 = "";

    [Tooltip("显示属性的提示信息")] //显示属性的提示信息
    [TextArea(10, 20)] //设置文本输入框的最小和最大行数
    public string str_2 = "";

    [ContextMenu("SetString2Text")] //组件右键齿轮的菜单选项
    public void SetString2Text()
    {
        str_2 = "SetString2Text";
    }

    [HideInInspector] //public变量在Inspector面板隐藏
    public string str_3 = "public变量在Inspector面板隐藏";

    [Header("显示属性的头部标题")] //显示属性的头部标题
    public string str_4 = "";

    [Space(20)] //设置与上一条属性的间隔
    public string str_5 = "";

    [Space(10)] //设置与上一条属性的间隔
    public string str_6 = "";

    [ColorUsage(true)] //可以设置是否展示颜色面板上显示度的设置条
    public Color color = Color.white;

    [ContextMenu("SetAllValue")] //组件右键齿轮的菜单选项
    public void SetAllValue()
    {
        num_1 = 50;
        num_2 = 50;
        str_1 = "SetString1Text";
        str_2 = "SetString2Text";
        str_4 = "显示属性的头部标题";
        str_5 = "设置与上一条属性的间隔";
        str_6 = "设置与上一条属性的间隔";
        color = Color.red;
    }
}