/// <summary>
/// 绘制组件中List类型属性中的元素类
/// </summary>
using UnityEngine;

[System.Serializable] //如果该类在一个公共数组中使用或作为一个公共变量在组件中使用，则序列化该类使其可保存
public class MyStateList
{
    public string Name;
    public Vector3 Position;
}