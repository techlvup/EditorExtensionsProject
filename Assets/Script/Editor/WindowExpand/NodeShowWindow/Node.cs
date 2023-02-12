using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

[Serializable]
public class NodeShowWindowData
{
    public Vector2Serializable posOffset = new Vector2Serializable(0, 0);
    public List<Node> nodes = new List<Node>();

    public NodeShowWindowData()
    {
    }

    public NodeShowWindowData(Vector2Serializable posOffset, List<Node> nodes)
    {
        this.posOffset = posOffset;
        this.nodes = nodes;
    }
}

[Serializable]
public class Node
{
    public int uid;
    public string name;

    public RectSerializable startRect;
    public RectSerializable currRect;
    public RectSerializable inNodeRect;
    public RectSerializable outNodeRect;

    private GUIStyle style;
    public GUIStyle StyleNormal
    {
        set { style = value; }

        get
        {
            style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            style.border = new RectOffset(12, 12, 12, 12);
            return style;
        }
    }

    public GUIStyle StyleDrag
    {
        set { style = value; }

        get
        {
            style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            style.border = new RectOffset(12, 12, 12, 12);
            return style;
        }
    }

    public GUIStyle StyleInNode
    {
        set { style = value; }

        get
        {
            style = new GUIStyle();
            style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            style.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            style.border = new RectOffset(4, 4, 12, 12);
            return style;
        }
    }

    public GUIStyle StyleOutNode
    {
        set { style = value; }

        get
        {
            style = new GUIStyle();
            style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            style.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            style.border = new RectOffset(4, 4, 12, 12);
            return style;
        }
    }

    public GUIStyle StyleNameBox
    {
        set { style = value; }

        get
        {
            style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node4.png") as Texture2D;
            style.border = new RectOffset(12, 12, 12, 12);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 10;
            return style;
        }
    }

    public Dictionary<ConnectType, Dictionary<Node,bool>> connectState;
    public ConnectType startConnect;

    public Node(float x, float y, float width, float height)
    {
        uid = DataManager.GetItemUId(ItemType.Node);
        name = uid.ToString();

        startRect = new RectSerializable(x, y, width, height);
        currRect = new RectSerializable(x, y, width, height);

        connectState = new Dictionary<ConnectType, Dictionary<Node, bool>>();
        connectState[ConnectType.In] = new Dictionary<Node, bool>();
        connectState[ConnectType.Out] = new Dictionary<Node, bool>();

        startConnect = ConnectType.None;
    }

    public void DrawProperty()
    {
        if (this is NodeRoot)
        {
            NodeRoot currNode = this as NodeRoot;
            currNode.DrawNodeProperty();
        }
        else if (this is NodeType1)
        {
            NodeType1 currNode = this as NodeType1;
            currNode.DrawNodeProperty();
        }
        else if (this is NodeType2)
        {
            NodeType2 currNode = this as NodeType2;
            currNode.DrawNodeProperty();
        }
    }
}

[Serializable]
public class NodeRoot : Node
{
    public NodeType NodeType { get; set; }

    public bool IsOpen { get; set; }

    public float Size { get; set; }

    public NodeRoot(float x, float y, float width, float height) : base(x, y, width, height)
    {
        NodeType = NodeType.TypeRoot;
        IsOpen = true;
        Size = 30;
    }

    public NodeRoot(Rect rect) : base(rect.x, rect.y, rect.width, rect.height)
    {
        NodeType = NodeType.TypeRoot;
        IsOpen = true;
        Size = 30;
    }

    public void DrawNodeProperty()
    {
        PropertyInfo[] propertyInfos = GetType().GetProperties();

        if (propertyInfos.Length > 0)
        {
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];

                GUILayout.BeginHorizontal();

                if (propertyInfo.PropertyType.FullName != typeof(GUIStyle).FullName)
                {
                    Vector2 size = StyleNormal.CalcSize(new GUIContent(propertyInfo.Name));
                    GUI.Label(new Rect(currRect.Value.x + 20, currRect.Value.y + 50 + i * 30, size.x + 5, size.y + 5), propertyInfo.Name);

                    if (propertyInfo.PropertyType.FullName == typeof(float).FullName)
                    {
                        float num = (float)propertyInfo.GetValue(this, null);
                        propertyInfo.SetValue(this, EditorGUI.Slider(new Rect(currRect.Value.x + currRect.Value.width - 100, currRect.Value.y + 50 + i * 30, 80, size.y + 5), num, 0, 100), null);
                    }
                    else if (propertyInfo.PropertyType.FullName == typeof(bool).FullName)
                    {
                        bool isOn = (bool)propertyInfo.GetValue(this, null);
                        propertyInfo.SetValue(this, EditorGUI.Toggle(new Rect(currRect.Value.x + currRect.Value.width - 65, currRect.Value.y + 50 + i * 30, 10, 10), isOn), null);
                    }
                    else if (propertyInfo.PropertyType.FullName == typeof(NodeType).FullName)
                    {
                        NodeType nodeType = (NodeType)propertyInfo.GetValue(this, null);
                        propertyInfo.SetValue(this, EditorGUI.EnumPopup(new Rect(currRect.Value.x + currRect.Value.width - 100, currRect.Value.y + 50 + i * 30, 80, size.y + 5), nodeType), null);
                    }
                }

                GUILayout.EndHorizontal();
            }
        }
    }
}

[Serializable]
public class NodeType1 : Node
{
    public NodeType NodeType { get; set; }

    public string Des { get; set; }

    public float Size { get; set; }

    public NodeType1(float x, float y, float width, float height) : base(x, y, width, height)
    {
        NodeType = NodeType.Type1;
        Des = "wwwwwww";
        Size = 30;
    }

    public NodeType1(Rect rect) : base(rect.x, rect.y, rect.width, rect.height)
    {
        NodeType = NodeType.Type1;
        Des = "wwwwwww";
        Size = 30;
    }

    public void DrawNodeProperty()
    {
        PropertyInfo[] propertyInfos = GetType().GetProperties();

        if (propertyInfos.Length > 0)
        {
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];

                GUILayout.BeginHorizontal();

                if (propertyInfo.PropertyType.FullName != typeof(GUIStyle).FullName)
                {
                    Vector2 size = StyleNormal.CalcSize(new GUIContent(propertyInfo.Name));
                    GUI.Label(new Rect(currRect.Value.x + 20, currRect.Value.y + 50 + i * 30, size.x + 5, size.y + 5), propertyInfo.Name);

                    if (propertyInfo.PropertyType.FullName == typeof(float).FullName)
                    {
                        float num = (float)propertyInfo.GetValue(this, null);
                        propertyInfo.SetValue(this, EditorGUI.Slider(new Rect(currRect.Value.x + currRect.Value.width - 100, currRect.Value.y + 50 + i * 30, 80, size.y + 5), num, 0, 100), null);
                    }
                    else if (propertyInfo.PropertyType.FullName == typeof(string).FullName)
                    {
                        string text = propertyInfo.GetValue(this, null).ToString();
                        propertyInfo.SetValue(this, EditorGUI.TextField(new Rect(currRect.Value.x + currRect.Value.width - 100, currRect.Value.y + 50 + i * 30, 80, size.y + 5), text), null);
                    }
                    else if (propertyInfo.PropertyType.FullName == typeof(NodeType).FullName)
                    {
                        NodeType nodeType = (NodeType)propertyInfo.GetValue(this, null);
                        propertyInfo.SetValue(this, EditorGUI.EnumPopup(new Rect(currRect.Value.x + currRect.Value.width - 100, currRect.Value.y + 50 + i * 30, 80, size.y + 5), nodeType), null);
                    }
                }

                GUILayout.EndHorizontal();
            }
        }
    }
}

[Serializable]
public class NodeType2 : Node
{
    public NodeType NodeType { get; set; }

    public bool IsOpen { get; set; }

    public string Des { get; set; }

    public NodeType2(float x, float y, float width, float height) : base(x, y, width, height)
    {
        NodeType = NodeType.Type2;
        IsOpen = false;
        Des = "qqqqqqq";
    }

    public NodeType2(Rect rect) : base(rect.x, rect.y, rect.width, rect.height)
    {
        NodeType = NodeType.Type2;
        IsOpen = false;
        Des = "qqqqqqq";
    }

    public void DrawNodeProperty()
    {
        PropertyInfo[] propertyInfos = GetType().GetProperties();

        if (propertyInfos.Length > 0)
        {
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];

                GUILayout.BeginHorizontal();

                if (propertyInfo.PropertyType.FullName != typeof(GUIStyle).FullName)
                {
                    Vector2 size = StyleNormal.CalcSize(new GUIContent(propertyInfo.Name));
                    GUI.Label(new Rect(currRect.Value.x + 20, currRect.Value.y + 50 + i * 30, size.x + 5, size.y + 5), propertyInfo.Name);

                    if (propertyInfo.PropertyType.FullName == typeof(string).FullName)
                    {
                        string text = propertyInfo.GetValue(this, null).ToString();
                        propertyInfo.SetValue(this, EditorGUI.TextField(new Rect(currRect.Value.x + currRect.Value.width - 100, currRect.Value.y + 50 + i * 30, 80, size.y + 5), text), null);
                    }
                    else if (propertyInfo.PropertyType.FullName == typeof(bool).FullName)
                    {
                        bool isOn = (bool)propertyInfo.GetValue(this, null);
                        propertyInfo.SetValue(this, EditorGUI.Toggle(new Rect(currRect.Value.x + currRect.Value.width - 65, currRect.Value.y + 50 + i * 30, 10, 10), isOn), null);
                    }
                    else if (propertyInfo.PropertyType.FullName == typeof(NodeType).FullName)
                    {
                        NodeType nodeType = (NodeType)propertyInfo.GetValue(this, null);
                        propertyInfo.SetValue(this, EditorGUI.EnumPopup(new Rect(currRect.Value.x + currRect.Value.width - 100, currRect.Value.y + 50 + i * 30, 80, size.y + 5), nodeType), null);
                    }
                }

                GUILayout.EndHorizontal();
            }
        }
    }
}