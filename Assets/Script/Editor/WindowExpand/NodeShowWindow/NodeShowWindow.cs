using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class NodeShowWindow : EditorWindow
{
    private Vector2 m_posOffset = Vector2.zero;
    private List<Node> m_nodes = new List<Node>();
    private string configPath = "EditorConfigData/NodeShowWindowData.bin";
    private int m_dragIndex = -1;
    private ConnectType m_startConnectType = ConnectType.None;
    private Node m_startConnectNode = null;
    private Dictionary<Vector2, Vector2> m_bezierList = new Dictionary<Vector2, Vector2>();

    [MenuItem("MyTest/MyWindows/NodeShowWindow")]
    public static void DisplayNodeShowWindow()
    {
        EditorWindow window = GetWindow<NodeShowWindow>("节点展示窗口");
        window.minSize = new Vector2(1200, 800);
        window.Show();
    }

    private void Awake()
    {
        if (!File.Exists(configPath))
        {
            for (int i = 0; i < 3; i++)
            {
                Node node = null;

                if (i == 0)
                {
                    node = new NodeRoot(100, 100, 250, 180);
                }
                else if (i == 1)
                {
                    node = new NodeType1(300, 300, 250, 180);
                }
                else if (i == 2)
                {
                    node = new NodeType2(500, 500, 250, 180);
                }

                m_nodes.Add(node);
            }

            SaveData();
        }
        else
        {
            NodeShowWindowData config = DataManager.ReadBinaryData<NodeShowWindowData>(configPath);

            m_posOffset = config.posOffset.Value;

            for (int i = 0; i < config.nodes.Count; i++)
            {
                m_nodes.Add(config.nodes[i]);
                DataManager.AddItemUId(ItemType.Node, config.nodes[i].uid);
            }
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        DrawGrid();

        DrawNode();

        ReceiveViewEvent(Event.current);

        DrawStartConnectLine();

        DrawConnectLine();

        DrawButton();
    }

    private void OnDestroy()
    {
        m_posOffset = Vector2.zero;
        m_nodes.Clear();
        m_dragIndex = -1;
        m_startConnectType = ConnectType.None;
        m_startConnectNode = null;
        m_bezierList.Clear();
        DataManager.ClearItemUId(ItemType.Node);
    }

    private void DrawGrid()
    {
        Color color1 = Color.gray;
        color1.a = 0.2f;

        Color color2 = Color.gray;
        color2.a = 0.4f;

        DrawGrid(20, color1);
        DrawGrid(100, color2);
    }

    private void DrawGrid(float space, Color color)
    {
        int colCount = Mathf.CeilToInt(position.width / space);
        int rowCount = Mathf.CeilToInt(position.height / space);

        Handles.color = color;

        for (int i = 0; i < colCount; i++)
        {
            float x = space / 2 + space * i + m_posOffset.x;
            float y = position.height;
            float width = colCount * space;

            if (x > width)
            {
                x %= width;
            }
            else if (x < 0)
            {
                x = x % width + width;
            }

            Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, y, 0));
        }

        for (int i = 0; i < rowCount; i++)
        {
            float x = position.width;
            float y = space / 2 + space * i + m_posOffset.y;
            float height = rowCount * space;

            if (y > position.height)
            {
                y %= height;
            }
            else if (y < 0)
            {
                y = y % height + height;
            }

            Handles.DrawLine(new Vector3(0, y, 0), new Vector3(x, y, 0));
        }

        Handles.color = Color.white;
    }

    private void DrawButton()
    {
        GUILayout.BeginHorizontal();

        if(GUI.Button(new Rect(10, 0, 50, 20), "保存"))
        {
            SaveData();
        }

        GUILayout.EndHorizontal();
    }

    private void DrawNode()
    {
        for (int i = 0; i < m_nodes.Count; i++)
        {
            Node node = m_nodes[i];

            GUIStyle gUIStyle;

            if(m_dragIndex == i)
            {
                gUIStyle = node.StyleDrag;
            }
            else
            {
                gUIStyle = node.StyleNormal;
            }

            //中间节点
            GUI.Box(node.currRect.Value, "", gUIStyle);

            if (node is NodeRoot)
            {
                NodeRoot currNode = node as NodeRoot;
                Vector2 size = node.StyleNormal.CalcSize(new GUIContent(currNode.name));
                GUI.Box(new Rect(node.currRect.Value.x + (node.currRect.Value.width - size.x - 50) / 2, node.currRect.Value.y + 3, size.x + 50, size.y + 15), currNode.name, node.StyleNameBox);
            }
            else if(node is NodeType1)
            {
                NodeType1 currNode = node as NodeType1;
                Vector2 size = node.StyleNormal.CalcSize(new GUIContent(currNode.name));
                GUI.Box(new Rect(node.currRect.Value.x + (node.currRect.Value.width - size.x - 50) / 2, node.currRect.Value.y + 3, size.x + 50, size.y + 15), currNode.name, node.StyleNameBox);
            }
            else if (node is NodeType2)
            {
                NodeType2 currNode = node as NodeType2;
                Vector2 size = node.StyleNormal.CalcSize(new GUIContent(currNode.name));
                GUI.Box(new Rect(node.currRect.Value.x + (node.currRect.Value.width - size.x - 50) / 2, node.currRect.Value.y + 3, size.x + 50, size.y + 15), currNode.name, node.StyleNameBox);
            }

            //入口点
            node.inNodeRect = new RectSerializable(node.currRect.Value.x - 2, node.currRect.Value.y + (node.currRect.Value.height / 2 - 5), 10, 20);
            if (GUI.Button(node.inNodeRect.Value, "", node.StyleInNode))
            {
                if (m_startConnectType == ConnectType.Out)
                {
                    if(m_startConnectNode.uid != node.uid)
                    {
                        bool isConnect1 = true;
                        bool isHave1 = node.connectState[ConnectType.In].TryGetValue(m_startConnectNode, out isConnect1);

                        bool isConnect2 = true;
                        bool isHave2 = m_startConnectNode.connectState[ConnectType.Out].TryGetValue(node, out isConnect2);

                        if ((!isHave1 || !isConnect1) && (!isHave2 || !isConnect2))
                        {
                            node.connectState[ConnectType.In][m_startConnectNode] = true;
                        }
                    }

                    m_startConnectType = ConnectType.None;
                }
                else
                {
                    m_startConnectNode = node;
                    m_startConnectType = ConnectType.In;
                }
            }

            //出口点
            node.outNodeRect = new RectSerializable(node.currRect.Value.x + node.currRect.Value.width - 8, node.currRect.Value.y + (node.currRect.Value.height / 2 - 5), 10, 20);
            if (GUI.Button(node.outNodeRect.Value, "", node.StyleOutNode))
            {
                if (m_startConnectType == ConnectType.In)
                {
                    if (m_startConnectNode.uid != node.uid)
                    {
                        bool isConnect1 = true;
                        bool isHave1 = node.connectState[ConnectType.Out].TryGetValue(m_startConnectNode, out isConnect1);

                        bool isConnect2 = true;
                        bool isHave2 = m_startConnectNode.connectState[ConnectType.In].TryGetValue(node, out isConnect2);

                        if ((!isHave1 || !isConnect1) && (!isHave2 || !isConnect2))
                        {
                            node.connectState[ConnectType.Out][m_startConnectNode] = true;
                        }
                    }

                    m_startConnectType = ConnectType.None;
                }
                else
                {
                    m_startConnectNode = node;
                    m_startConnectType = ConnectType.Out;
                }
            }

            node.DrawProperty();
        }
    }

    private void ReceiveViewEvent(Event operation)
    {
        switch (operation.type)
        {
            case EventType.MouseDown:
                {
                    Vector2 mousePosition = operation.mousePosition;
                    m_dragIndex = -1;

                    if (m_nodes.Count > 1)
                    {
                        Node node = null;

                        for (int i = m_nodes.Count - 1; i >= 0; i--)
                        {
                            if (m_dragIndex == -1 &&
                                (m_nodes[i].currRect.Value.Contains(mousePosition)
                                || m_nodes[i].inNodeRect.Value.Contains(mousePosition)
                                || m_nodes[i].outNodeRect.Value.Contains(mousePosition)))
                            {
                                node = m_nodes[i];
                                m_dragIndex = i;
                            }
                        }

                        if(node != null)
                        {
                            if ((m_startConnectType == ConnectType.In && !node.outNodeRect.Value.Contains(mousePosition))
                                || (m_startConnectType == ConnectType.Out && !node.inNodeRect.Value.Contains(mousePosition)))
                            {
                                m_startConnectType = ConnectType.None;
                            }

                            if (operation.button == 1 && node.currRect.Value.Contains(mousePosition))
                            {
                                ShowRemoveMenu(new Rect(mousePosition.x, mousePosition.y, 250, 60));
                            }
                        }
                        else
                        {
                            m_startConnectType = ConnectType.None;

                            if (operation.button == 1)
                            {
                                ShowAddMenu(new Rect(mousePosition.x, mousePosition.y, 250, 180));
                            }
                        }
                    }
                    else
                    {
                        m_startConnectType = ConnectType.None;

                        if (operation.button == 1)
                        {
                            ShowAddMenu(new Rect(mousePosition.x, mousePosition.y, 250, 180));
                        }
                    }
                }
                break;

            case EventType.MouseDrag:
                {
                    if (operation.button == 0)
                    {
                        if(operation.delta.x != 0 || operation.delta.y != 0)
                        {
                            if (m_dragIndex != -1)
                            {
                                DragNode(operation.delta, m_dragIndex);
                            }
                            else
                            {
                                m_posOffset += operation.delta;
                                DragNode(m_posOffset);
                            }
                        }
                    }
                }
                break;

            default:
                break;
        }
    }

    private void SaveData()
    {
        Vector2Serializable posOffset = new Vector2Serializable(m_posOffset.x, m_posOffset.y);

        NodeShowWindowData nodeShowWindowData = new NodeShowWindowData(posOffset, m_nodes);

        DataManager.SaveBinaryData(configPath, nodeShowWindowData);
    }

    private void DragNode(Vector2 posOffset, int dragIndex = -1)
    {
        if (dragIndex != -1)
        {
            Node node = m_nodes[dragIndex];

            if(node == null)
            {
                return;
            }

            node.startRect = new RectSerializable(node.startRect.Value.x + posOffset.x, node.startRect.Value.y + posOffset.y, node.startRect.Value.width, node.startRect.Value.height);
            node.currRect = new RectSerializable(node.startRect.Value.x + m_posOffset.x, node.startRect.Value.y + m_posOffset.y, node.startRect.Value.width, node.startRect.Value.height);
        }
        else
        {
            if (m_nodes.Count > 1)
            {
                for (int i = 0; i < m_nodes.Count; i++)
                {
                    Node node = m_nodes[i];

                    node.currRect = new RectSerializable(node.startRect.Value.x + m_posOffset.x, node.startRect.Value.y + m_posOffset.y, node.startRect.Value.width, node.startRect.Value.height);
                }
            }
        }
    }

    private void DrawStartConnectLine()
    {
        if(m_startConnectType == ConnectType.None)
        {
            return;
        }

        Vector2 startPoint = Vector2.zero;
        Vector2 endPoint = Event.current.mousePosition;
        Vector2 startTangent = Vector2.zero;
        Vector2 endTangent = Vector2.zero;

        if (m_startConnectType == ConnectType.In)
        {
            startPoint = m_startConnectNode.inNodeRect.Value.center;
            startTangent = m_startConnectNode.inNodeRect.Value.center - Vector2.right * 100;
            endTangent = Event.current.mousePosition + Vector2.right * 100;
        }
        else if(m_startConnectType == ConnectType.Out)
        {
            startPoint = m_startConnectNode.outNodeRect.Value.center;
            startTangent = m_startConnectNode.outNodeRect.Value.center + Vector2.right * 100;
            endTangent = Event.current.mousePosition - Vector2.right * 100;
        }

        Handles.DrawBezier(startPoint, Event.current.mousePosition,
                        startTangent, endTangent,
                        Color.white, null, 2f);
    }

    private void DrawConnectLine()
    {
        //绘制贝塞尔曲线（起始位置，结束位置，起始切线(从起始点绘制到某位置后开始弯曲)，终止切线(从起始点绘制到某位置后停止弯曲)，颜色，图片，宽度）
        for (int i = 0; i < m_nodes.Count; i++)
        {
            Node currNode = m_nodes[i];
            DrawConnectLine(currNode, ConnectType.In);
            DrawConnectLine(currNode, ConnectType.Out);
        } 
    }

    private void DrawConnectLine(Node currNode, ConnectType connectType)
    {
        if (currNode.connectState[connectType].Count <= 0)
        {
            return;
        }

        foreach (var item in currNode.connectState[connectType])
        {
            bool isConnect = item.Value;

            if (!isConnect)
            {
                continue;
            }

            Node connectNode = item.Key;

            Vector2 startPoint = Vector2.zero;
            Vector2 endPoint = Vector2.zero;

            if (connectType == ConnectType.In)
            {
                startPoint = connectNode.outNodeRect.Value.center;
                endPoint = currNode.inNodeRect.Value.center;
            }
            else if (connectType == ConnectType.Out)
            {
                startPoint = currNode.outNodeRect.Value.center;
                endPoint = connectNode.inNodeRect.Value.center;
            }

            Vector2 tempStartPoint;
            bool isHave = m_bezierList.TryGetValue(endPoint, out tempStartPoint);

            if (isHave && tempStartPoint == startPoint)
            {
                continue;
            }

            Handles.DrawBezier(startPoint, endPoint,
                startPoint + Vector2.right * 100, endPoint - Vector2.right * 100,
                Color.white, null, 2f);

            m_bezierList[startPoint] = endPoint;

            if (Handles.Button((startPoint + endPoint) / 2, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            {
                currNode.connectState[connectType][connectNode] = false;

                ConnectType tempConnectType = ConnectType.None;

                if (connectType == ConnectType.In)
                {
                    tempConnectType = ConnectType.Out;
                }
                else if (connectType == ConnectType.Out)
                {
                    tempConnectType = ConnectType.In;
                }

                if(connectNode.connectState[tempConnectType].Count > 0)
                {
                    bool isDraw = false;
                    bool isHave2 = connectNode.connectState[tempConnectType].TryGetValue(currNode, out isDraw);

                    if (isHave2 && isDraw)
                    {
                        connectNode.connectState[tempConnectType][currNode] = false;
                    }
                }

                m_bezierList.Remove(startPoint);

                break;
            }
        }
    }

    private void ShowAddMenu(Rect rect)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("添加TypeRoot"), false, () => { AddItemClick(rect, NodeType.TypeRoot); });
        menu.AddItem(new GUIContent("添加Type1"), false, () => { AddItemClick(rect, NodeType.Type1); });
        menu.AddItem(new GUIContent("添加Type2"), false, () => { AddItemClick(rect, NodeType.Type2); });

        menu.ShowAsContext();
    }

    private void ShowRemoveMenu(Rect rect)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("移除节点"), false, RemoveNode);

        menu.ShowAsContext();
    }

    private void RemoveNode()
    {
        for (int i = 0; i < m_nodes.Count; i++)
        {
            RemoveNode(ConnectType.In);
            RemoveNode(ConnectType.Out);
        }

        m_nodes.RemoveAt(m_dragIndex);
    }

    private void RemoveNode(ConnectType connectType)
    {
        for (int i = 0; i < m_nodes.Count; i++)
        {
            if(m_nodes[i].connectState[connectType].ContainsKey(m_nodes[m_dragIndex]))
            {
                m_nodes[i].connectState[connectType].Remove(m_nodes[m_dragIndex]);
            }
        }
    }

    private void AddItemClick(Rect rect, NodeType nodeType)
    {
        if (nodeType == NodeType.TypeRoot)
        {
            NodeRoot node = new NodeRoot(rect);
            m_nodes.Add(node);
        }
        else if (nodeType == NodeType.Type1)
        {
            NodeType1 node = new NodeType1(rect);
            m_nodes.Add(node);
        }
        else if (nodeType == NodeType.Type2)
        {
            NodeType2 node = new NodeType2(rect);
            m_nodes.Add(node);
        }
    }
}