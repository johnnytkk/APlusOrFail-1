using System;
using UnityEngine;

namespace APlusOrFail.ObjectGrid
{
    [ExecuteInEditMode]
    public class ObjectGrid : MonoBehaviour
    {
        private class GridObject
        {
            public RectInt gridRect { get; }
            public readonly Transform objectTransform;

            public GridObject(RectInt gridRect, Transform objectTransform)
            {
                this.gridRect = gridRect;
                this.objectTransform = objectTransform;
            }
        }

        public static ObjectGrid instance { get; private set; }
        
        public int gridColumnCount;
        public int gridRowCount;
        public float gridX = 0;
        public float gridY = 0;
        public float gridColumnWidth = 1;
        public float gridRowHeight = 1;

        private GridObject[,] grid;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
                Debug.LogErrorFormat("Found another object grid!");
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
        
        private void Start()
        {
            grid = new GridObject[gridColumnCount, gridRowCount];
        }

        public Vector2 WorldToGridCoordinate(Vector2 worldPosition)
        {
            return new Vector2((worldPosition.x - gridX) / gridColumnWidth, (worldPosition.y - gridY) / gridRowHeight);
        }

        public RectInt SnapToGrid(Vector2 gridCenterPosition, Vector2Int gridSize)
        {
            int width = gridSize.x;
            int height = gridSize.y;
            int x;
            int y;
            if (width % 2 == 0)
            {
                x = Mathf.RoundToInt(gridCenterPosition.x) - width / 2;
            }
            else
            {
                x = Mathf.FloorToInt(Mathf.Round(gridCenterPosition.x - 0.5f) + 0.5f) - (width - 1) / 2;
            }
            if (height % 2 == 0)
            {
                y = Mathf.RoundToInt(gridCenterPosition.y) - height / 2;
            }
            else
            {
                y = Mathf.FloorToInt(Mathf.Round(gridCenterPosition.y - 0.5f) + 0.5f) - (height - 1) / 2;
            }
            return new RectInt(x, y, width, height);
        }

        public Rect GridRectToRect(RectInt gridRect)
        {
            float x = gridRect.x * gridColumnWidth + gridX;
            float y = gridRect.y * gridRowHeight + gridY;
            float width = gridRect.width * gridColumnWidth;
            float height = gridRect.height * gridRowHeight;
            return new Rect(x, y, width, height);
        }

        public bool IsPlaceable(RectInt gridRect)
        {
            if (!IsWithinRange(gridRect))
            {
                return false;
            }
            else
            {
                for (int x = gridRect.x; x < gridRect.width; ++x)
                {
                    for (int y = gridRect.y; y < gridRect.height; ++y)
                    {
                        if (grid[x, y] != null)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public void AddToGrid(RectInt gridRect, GameObject objectPrefab)
        {
            if (!IsWithinRange(gridRect))
            {
                throw new ArgumentOutOfRangeException($"({gridRect}) is not a valid rect");
            }
            if (!IsPlaceable(gridRect))
            {
                throw new ArgumentException($"({gridRect}) has already been occupied");
            }
            GameObject obj = Instantiate(objectPrefab, GridRectToRect(gridRect).center, Quaternion.identity);
            GridObject gridObject = new GridObject(gridRect, obj.transform);
            for (int x = gridRect.x; x < gridRect.width; ++x)
            {
                for (int y = gridRect.y; y < gridRect.height; ++y)
                {
                    grid[x, y] = gridObject;
                }
            }
        }

        public void RemoveFromGrid(RectInt gridRect)
        {
            if (!IsWithinRange(gridRect))
            {
                throw new ArgumentOutOfRangeException($"({gridRect}) is not a valid rect");
            }
            for (int x = gridRect.x; x < gridRect.width; ++x)
            {
                for (int y = gridRect.y; y < gridRect.height; ++y)
                {
                    GridObject gridObject = grid[x, y];
                    if (gridObject != null)
                    {
                        Destroy(gridObject.objectTransform.gameObject);
                        for (int i = gridObject.gridRect.x; i < gridObject.gridRect.width; ++i)
                        {
                            for (int j = gridObject.gridRect.y; j < gridObject.gridRect.height; ++j)
                            {
                                grid[i, j] = null;
                            }
                        }
                    }
                }
            }
        }

        private bool IsWithinRange(RectInt gridRect)
        {
            return gridRect.xMin >= 0 && gridRect.yMin >= 0 &&
                   gridRect.xMax <= grid.GetLength(0) && gridRect.yMax <= grid.GetLength(1);
        }
    }
}
