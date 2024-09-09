using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using Unity.VisualScripting;
public class BaseBlock : MonoBehaviour
{
    #region Variables
    public BlockBaseProp Prop;
    public Transform WallsHolder;
    private string WallPrefabInResource = "Block/Wall";
    #endregion

    #region Unity
    private void Start()
    {
        DrawWall();
    }
    #endregion

    #region Draw Initial Block
    private List<SpriteRenderer> TempWalls;
    public void DrawWall()
    {
        //clear Temp
        if (TempWalls != null)
        {
            for (int i = 0; i < TempWalls.Count; i++)
            {
                Destroy(TempWalls[i].gameObject);
            }
        }
        TempWalls = new List<SpriteRenderer>();

        //draw
        SpriteRenderer wallPrefab = Resources.Load<SpriteRenderer>(WallPrefabInResource);
        if (wallPrefab != null)
        {
            //draw Corner
            for (int i = 0; i < 4; i++)
            {
                SpriteRenderer wall = Instantiate(wallPrefab, WallsHolder);
                wall.transform.localScale = Prop.WallThick * Vector2.one;
                Vector2 pos = new Vector2(i % 2 - 0.5f, i / 2 - 0.5f) * 2;
                wall.transform.localPosition = pos * (0.5f - Prop.WallThick / 2);
                wall.color = Prop.WallColor;
                TempWalls.Add(wall);
            }
            List<Vector2Int> doorlist = Prop.Doors.Select(d => new Vector2Int((int)(d.Direction.x * -0.5f + d.Direction.y * 1.5f + 1.5f),
             (d.Direction.y < 0 || d.Direction.x > 0) ? -d.Position.x * d.Direction.y + d.Position.y * d.Direction.x : (Prop.Size - 1 - d.Position.x * d.Direction.y + d.Position.y * d.Direction.x) % Prop.Size)).ToList();
            //draw Wall
            for (int j = 0; j < Prop.Size; j++)
            {
                for (int i = 0; i < 4; i++) //four direction
                {
                    if (doorlist.Contains(new Vector2Int(i, j))) continue;
                    SpriteRenderer wall = Instantiate(wallPrefab, WallsHolder);
                    Vector2 start = new Vector2(i % 2, i / 2);
                    wall.transform.localScale = new Vector3((start.x + start.y) % 2 == 0 ? (1 - Prop.WallThick * 2) / Prop.Size : Prop.WallThick,
                        (start.x + start.y) % 2 == 1 ? (1 - Prop.WallThick * 2) / Prop.Size : Prop.WallThick, 1);

                    Vector2 pos = new Vector2(i % 2 - 0.5f, i / 2 - 0.5f) * 2;
                    wall.transform.localPosition = pos * (0.5f - Prop.WallThick / 2)
                        + (start.x == 0 ? 1 : -1) * ((start.x + start.y) % 2 == 0 ? Vector2.right : Vector2.down) * ((Prop.WallThick + (1 - Prop.WallThick * 2) / Prop.Size) / 2 + j * (1 - Prop.WallThick * 2) / Prop.Size);
                    wall.color = Prop.WallColor;
                    TempWalls.Add(wall);
                }
            }
        }
    }

    #endregion
}

[Serializable]
public class BlockBaseProp
{
    [Serializable]
    public class DoorInfo
    {
        public Vector2Int Position;
        public Vector2Int Direction;

        public override bool Equals(object obj)
        {
            if (obj is DoorInfo)
            {
                return Position == ((DoorInfo)obj).Position && Direction == ((DoorInfo)obj).Direction;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public int Size;
    public float WallThick = 0.1f;
    public Color WallColor;
    public List<DoorInfo> Doors;
}