using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;



#if UNITY_EDITOR
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(BaseBlock)), CanEditMultipleObjects, InitializeOnLoad]
public class BaseBlockEditor : Editor
{
    BaseBlock block;

    protected virtual void OnSceneGUI()
    {
        if (block == null) block = (BaseBlock)target;
        block.Prop.Size = Mathf.Clamp(block.Prop.Size, 1, 100);
        #region Draw Grid
        Handles.color = Color.green;
        for (int i = 0; i <= block.Prop.Size; i++)
        {
            Handles.DrawLine(block.transform.position + block.transform.localScale.x / block.Prop.Size * 2 * (0.5f - block.Prop.WallThick) * new Vector3(i - block.Prop.Size / 2f, -block.Prop.Size / 2f, 1),
                block.transform.position + block.transform.localScale.x / block.Prop.Size * 2 * (0.5f - block.Prop.WallThick) * new Vector3(i - block.Prop.Size / 2f, block.Prop.Size / 2f, 1));
            Handles.DrawLine(block.transform.position + block.transform.localScale.x / block.Prop.Size * 2 * (0.5f - block.Prop.WallThick) * new Vector3(-block.Prop.Size / 2f, i - block.Prop.Size / 2f, 1),
             block.transform.position + block.transform.localScale.x / block.Prop.Size * 2 * (0.5f - block.Prop.WallThick) * new Vector3(block.Prop.Size / 2f, i - block.Prop.Size / 2f, 1));
        }
        #endregion

        #region Draw Corner
        //draw Corner
        Handles.color = Color.black;
        for (int i = 0; i < 4; i++)
        {
            Vector3 scale = block.Prop.WallThick * block.transform.localScale;
            Vector3 pos = new Vector3(i % 2 - 0.5f, i / 2 - 0.5f, 1) * 2;
            pos = block.transform.position + block.transform.localScale.x * pos * (0.5f - block.Prop.WallThick / 2);
            Handles.DrawLine(pos + new Vector3(scale.x, scale.y) / 2, pos + new Vector3(scale.x, -scale.y) / 2);
            Handles.DrawLine(pos + new Vector3(scale.x, scale.y) / 2, pos + new Vector3(-scale.x, scale.y) / 2);
            Handles.DrawLine(pos + new Vector3(scale.x, scale.y) / 2, pos + new Vector3(-scale.x, -scale.y) / 2);
            Handles.DrawLine(pos + new Vector3(-scale.x, -scale.y) / 2, pos + new Vector3(scale.x, -scale.y) / 2);
            Handles.DrawLine(pos + new Vector3(-scale.x, -scale.y) / 2, pos + new Vector3(-scale.x, scale.y) / 2);
            Handles.DrawLine(pos + new Vector3(scale.x, -scale.y) / 2, pos + new Vector3(-scale.x, scale.y) / 2);
        }
        List<Vector2Int> doorlist = block.Prop.Doors.Select(d => new Vector2Int((int)(d.Direction.x * -0.5f + d.Direction.y * 1.5f + 1.5f),
         (d.Direction.y < 0 || d.Direction.x > 0) ? -d.Position.x * d.Direction.y + d.Position.y * d.Direction.x : (block.Prop.Size - 1 - d.Position.x * d.Direction.y + d.Position.y * d.Direction.x) % block.Prop.Size)).ToList();
        #endregion

        #region Draw wall
        for (int j = 0; j < block.Prop.Size; j++)
        {
            for (int i = 0; i < 4; i++) //four direction
            {
                Vector2 start = new Vector2(i % 2, i / 2);
                Vector3 scale = new Vector3(block.transform.localScale.x * ((start.x + start.y) % 2 == 0 ? (1 - block.Prop.WallThick * 2) / block.Prop.Size : block.Prop.WallThick),
                  block.transform.localScale.y * ((start.x + start.y) % 2 == 1 ? (1 - block.Prop.WallThick * 2) / block.Prop.Size : block.Prop.WallThick), 1);


                Vector3 pos = new Vector2(i % 2 - 0.5f, i / 2 - 0.5f) * 2 * (0.5f - block.Prop.WallThick / 2)
                    + (start.x == 0 ? 1 : -1) * ((start.x + start.y) % 2 == 0 ? Vector2.right : Vector2.down) * ((block.Prop.WallThick + (1 - block.Prop.WallThick * 2) / block.Prop.Size) / 2 + j * (1 - block.Prop.WallThick * 2) / block.Prop.Size);
                pos = block.transform.position + block.transform.localScale.x * pos;
                Handles.DrawLine(pos + new Vector3(scale.x, scale.y) / 2, pos + new Vector3(scale.x, -scale.y) / 2);
                Handles.DrawLine(pos + new Vector3(scale.x, scale.y) / 2, pos + new Vector3(-scale.x, scale.y) / 2);
                Handles.DrawLine(pos + new Vector3(-scale.x, -scale.y) / 2, pos + new Vector3(scale.x, -scale.y) / 2);
                Handles.DrawLine(pos + new Vector3(-scale.x, -scale.y) / 2, pos + new Vector3(-scale.x, scale.y) / 2);

                if (doorlist.Contains(new Vector2Int(i, j)) == false)
                {
                    Handles.DrawLine(pos + new Vector3(scale.x, -scale.y) / 2, pos + new Vector3(-scale.x, scale.y) / 2);
                    Handles.DrawLine(pos + new Vector3(scale.x, scale.y) / 2, pos + new Vector3(-scale.x, -scale.y) / 2);
                }

            }
        }
        #endregion

        #region Mouse

        Event e = Event.current;

        Vector2 mousePosition = e.mousePosition;
        Ray worldRay = HandleUtility.GUIPointToWorldRay(mousePosition);

        if (Physics.Raycast(worldRay, out RaycastHit hitinfo))
        {
            Vector2 localPos = (hitinfo.point - block.transform.position) / block.transform.localScale.x;
            BlockBaseProp.DoorInfo temptDoor = null;
            if ((Mathf.Abs(localPos.x) < 0.5 && Mathf.Abs(localPos.x) > 0.5 - block.Prop.WallThick && Mathf.Abs(localPos.y) < 0.5 - block.Prop.WallThick))
            {
                temptDoor = new BlockBaseProp.DoorInfo()
                {
                    Direction = localPos.x > 0 ? Vector2Int.right : Vector2Int.left,
                    Position = new Vector2Int(0, Mathf.FloorToInt((localPos.y + 0.5f - block.Prop.WallThick) / (1 - block.Prop.WallThick * 2) * block.Prop.Size))
                };
            }
            else if ((Mathf.Abs(localPos.y) < 0.5 && Mathf.Abs(localPos.y) > 0.5 - block.Prop.WallThick && Mathf.Abs(localPos.x) < 0.5 - block.Prop.WallThick))
            {
                temptDoor = new BlockBaseProp.DoorInfo()
                {
                    Direction = localPos.y > 0 ? Vector2Int.up : Vector2Int.down,
                    Position = new Vector2Int(Mathf.FloorToInt((localPos.x + 0.5f - block.Prop.WallThick) / (1 - block.Prop.WallThick * 2) * block.Prop.Size), 0)
                };

            }
            if (temptDoor != null)
            {
                BlockBaseProp.DoorInfo d = temptDoor;
                Vector2Int p = new Vector2Int((int)(d.Direction.x * -0.5f + d.Direction.y * 1.5f + 1.5f),
                      (d.Direction.y < 0 || d.Direction.x > 0) ? -d.Position.x * d.Direction.y + d.Position.y * d.Direction.x : (block.Prop.Size - 1 - d.Position.x * d.Direction.y + d.Position.y * d.Direction.x) % block.Prop.Size);
                int i = p.x, j = p.y;

                Vector2 start = new Vector2(i % 2, i / 2);
                Vector3 scale = new Vector3(block.transform.localScale.x * ((start.x + start.y) % 2 == 0 ? (1 - block.Prop.WallThick * 2) / block.Prop.Size : block.Prop.WallThick),
                  block.transform.localScale.y * ((start.x + start.y) % 2 == 1 ? (1 - block.Prop.WallThick * 2) / block.Prop.Size : block.Prop.WallThick), 1);
                scale *= 0.8f;

                Vector3 pos = new Vector2(i % 2 - 0.5f, i / 2 - 0.5f) * 2 * (0.5f - block.Prop.WallThick / 2)
                    + (start.x == 0 ? 1 : -1) * ((start.x + start.y) % 2 == 0 ? Vector2.right : Vector2.down) * ((block.Prop.WallThick + (1 - block.Prop.WallThick * 2) / block.Prop.Size) / 2 + j * (1 - block.Prop.WallThick * 2) / block.Prop.Size);
                pos = block.transform.position + block.transform.localScale.x * pos;

                if (block.Prop.Doors.Contains(temptDoor)) Handles.color = Color.blue;
                else Handles.color = Color.red;

                Handles.DrawLine(pos + new Vector3(scale.x, scale.y) / 2, pos + new Vector3(scale.x, -scale.y) / 2);
                Handles.DrawLine(pos + new Vector3(scale.x, scale.y) / 2, pos + new Vector3(-scale.x, scale.y) / 2);
                Handles.DrawLine(pos + new Vector3(-scale.x, -scale.y) / 2, pos + new Vector3(scale.x, -scale.y) / 2);
                Handles.DrawLine(pos + new Vector3(-scale.x, -scale.y) / 2, pos + new Vector3(-scale.x, scale.y) / 2);

                if (e.type == EventType.KeyDown && e.keyCode == KeyCode.S)
                {
                    if (block.Prop.Doors.Contains(temptDoor)) block.Prop.Doors.Remove(temptDoor);
                    else block.Prop.Doors.Add(temptDoor);
                }
            }


        }

        if (e.type == EventType.MouseMove || e.type == EventType.Repaint)
        {
            SceneView.currentDrawingSceneView.Repaint();
        }
        #endregion
    }
    public override void OnInspectorGUI()
    {
        if (block == null) block = (BaseBlock)target;
        DrawDefaultInspector();
        GUILayout.Label("-Press S to turn on and off the wall that your mouse is above");
    }
}
#endif