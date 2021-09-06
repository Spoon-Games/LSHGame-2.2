#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LSHGame.Util
{
    [CustomGridBrush(true, false, false, "Pattern Brush")]
    public class PatternBrush : GridBrush
    {
        public Vector3Int PatternOffset { get; set; }

        private ArrayList m_Locations;
        private ArrayList m_Tiles;

        private ArrayList locations
        {
            get
            {
                if (m_Locations == null)
                    m_Locations = new ArrayList();
                return m_Locations;
            }
        }

        private ArrayList tiles
        {
            get
            {
                if (m_Tiles == null)
                    m_Tiles = new ArrayList();
                return m_Tiles;
            }
        }

        [MenuItem("Assets/Create/Pattern Brush")]
        public static void CreateBrush()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Pattern Brush", "New Pattern Brush", "asset", "Save Pattern Brush", "Assets");

            if (path == "")
                return;

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<PatternBrush>(), path);
        }


        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null)
                return;

            Tilemap map = brushTarget.GetComponent<Tilemap>();
            if (map == null)
                return;

            locations.Clear();
            tiles.Clear();
            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location + PatternOffset;
                
                BrushCell cell = cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                if (cell.tile == null)
                    continue;

                locations.Add(location);
                tiles.Add(cell.tile);
            }
            map.SetTiles((Vector3Int[])locations.ToArray(typeof(Vector3Int)), (TileBase[])tiles.ToArray(typeof(TileBase)));
            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location - position.min;
                BrushCell cell = cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                if (cell.tile == null)
                    continue;

                map.SetTransformMatrix(location, cell.matrix);
                map.SetColor(location, cell.color);
            }
        }

        public void AddOffset(int x,int y)
        {
            PatternOffset = PatternOffset + new Vector3Int(x, y,0);
        }



        #region Helper Methods

        public new int GetCellIndexWrapAround(int x, int y, int z)
        {
            return modl(x,size.x) + size.x * modl(y,size.y) + size.x * size.y * modl(z,size.z);
        }

        private int modl(int x, int m)
        {
            return (x % m + m) % m;
        }

        #endregion
    }

    [CustomEditor(typeof(PatternBrush))]
    public class PatternBrushEditor : UnityEditor.Tilemaps.GridBrushEditor
    {
        private PatternBrush Brush { get { return target as PatternBrush; } }


        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);
            GetInput();
        }


        public override void OnSceneGUI(GridLayout gridLayout, GameObject brushTarget)
        {
            GetInput();
            base.OnSceneGUI(gridLayout, brushTarget);
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            //lineBrush.zp
            GetInput();
            Brush.PatternOffset = EditorGUILayout.Vector3IntField("Offset", Brush.PatternOffset);

            if (GUI.changed)
                EditorUtility.SetDirty(Brush);


        }


        private void GetInput()
        {
            int controllId = GUIUtility.GetControlID(FocusType.Keyboard);
            Event e = Event.current;

            if (e.GetTypeForControl(controllId) == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.W)
                {
                    Brush.AddOffset(0, 1);
                    e.Use();
                }
                else if (e.keyCode == KeyCode.S)
                {
                    Brush.AddOffset(0, -1);
                    e.Use();
                }
                else if (e.keyCode == KeyCode.D)
                {
                    Brush.AddOffset(1, 0);
                    e.Use();
                }
                else if (e.keyCode == KeyCode.A)
                {
                    Brush.AddOffset(-1, 0);
                    e.Use();
                }

                EditorUtility.SetDirty(this);
            }
        }
    }

}

#endif