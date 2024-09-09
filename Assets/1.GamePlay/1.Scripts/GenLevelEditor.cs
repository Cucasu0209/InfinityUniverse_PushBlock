using UnityEditor;
using UnityEngine;

public class GenLevelEditor : EditorWindow
{
    private Rect nodeRect = new Rect(100, 100, 100, 100);
    private bool isDragging = false;

    [MenuItem("Window/Node Editor")]
    public static void ShowWindow()
    {
        GetWindow<GenLevelEditor>("Node Editor");
    }

    private void OnGUI()
    {
        // Vẽ ô vuông đại diện cho node
        GUI.color = Color.white;
        GUI.Box(nodeRect, "Node");

        // Xử lý sự kiện kéo thả
        HandleEvents();
    }

    private void HandleEvents()
    {
        Event e = Event.current;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (nodeRect.Contains(e.mousePosition))
                {
                    isDragging = true;
                }
                break;

            case EventType.MouseDrag:
                if (isDragging)
                {
                    nodeRect.position += e.delta;
                    Repaint();
                }
                break;

            case EventType.MouseUp:
                isDragging = false;
                break;
        }
    }
}
