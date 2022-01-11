using System;
using Editor.NodeEditor;
using UnityEngine;

public enum ConnectionPointType { In, Out }

public class ConnectionPoint
{
    public Rect rect;

    public ConnectionPointType type;

    public GraphNode GraphNode;

    public GUIStyle style;

    public Action<ConnectionPoint> OnClickConnectionPoint;
    
    public ConnectionPoint(GraphNode graphNode, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint)
    {
        this.GraphNode = graphNode;
        this.type = type;
        this.style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        rect = new Rect(0, 0, 20f, 20f);
    }

    public void Draw()
    {
        rect.y = GraphNode.Rect.y + (GraphNode.Rect.height * 0.5f) - rect.height * 0.5f;

        switch (type)
        {
            case ConnectionPointType.In:
                rect.x = GraphNode.Rect.x - rect.width + 8f;
                break;

            case ConnectionPointType.Out:
                rect.x = GraphNode.Rect.x + GraphNode.Rect.width - 8f;
                break;
        }
        
        if (GUI.Button(rect, "", style))
        {
            if (OnClickConnectionPoint != null)
            {
                OnClickConnectionPoint(this);
            }
        }
    }
}