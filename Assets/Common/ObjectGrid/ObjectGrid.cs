using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace APlusOrFail.ObjectGrid
{
    public class ObjectGrid : MonoBehaviour
    {
        private class GridObject
        {
            private readonly ObjectGrid outer;
            public readonly ReadOnlyCollection<RectInt> rects;
            public readonly GameObject obj;

            public GridObject(ObjectGrid outer, IEnumerable<RectInt> rects, GameObject obj)
            {
                this.outer = outer;
                this.rects = new ReadOnlyCollection<RectInt>(rects.ToList());
                this.obj = obj;
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
        private readonly Dictionary<GameObject, GridObject> objectToGridMap = new Dictionary<GameObject, GridObject>();

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

            grid = new GridObject[gridColumnCount, gridRowCount];
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        public Vector2Int WorldToGridPosition(Vector2 worldPosition)
        {
            return new Vector2Int(
                Mathf.FloorToInt((worldPosition.x - gridX) / gridColumnWidth),
                Mathf.FloorToInt((worldPosition.y - gridY) / gridRowHeight)
            );
        }

        public Vector2 GridToWorldPosition(Vector2Int gridPosition)
        {
            return new Vector2(
                gridPosition.x * gridColumnWidth + gridX,
                gridPosition.y * gridRowHeight + gridY
            );
        }

        public Vector2 GridToWorldSize(Vector2Int gridSize)
        {
            return new Vector2(
                gridSize.x * gridColumnWidth,
                gridSize.y * gridRowHeight
            );
        }

        public Rect GridToWorldRect(RectInt gridRect)
        {
            return new Rect(GridToWorldPosition(gridRect.min), GridToWorldSize(gridRect.size));
        }

        public bool IsPlaceable(IEnumerable<RectInt> gridRects)
        {
            return gridRects.All(r => IsPlaceable(r));
        }

        public bool IsPlaceable(RectInt gridRect)
        {
            if (!IsWithinRange(gridRect))
            {
                return false;
            }
            else
            {
                for (int x = gridRect.xMin; x < gridRect.xMax; ++x)
                {
                    for (int y = gridRect.yMin; y < gridRect.yMax; ++y)
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

        public void Add(IEnumerable<RectInt> gridRects, GameObject obj)
        {
            RectInt? tempRect;
            if ((tempRect = gridRects.Where(r => !IsWithinRange(r)).Select(r => (RectInt?)r).FirstOrDefault()) != null)
            {
                throw new ArgumentOutOfRangeException($"({tempRect.Value}) is not in range");
            }
            if ((tempRect = gridRects.Where(r => !IsPlaceable(r)).Select(r => (RectInt?)r).FirstOrDefault()) != null)
            {
                throw new ArgumentException($"({tempRect.Value}) has already been occupied");
            }
            
            GridObject gridObject = new GridObject(this, gridRects, obj);

            foreach (RectInt gridRect in gridRects)
            {
                for (int x = gridRect.xMin; x < gridRect.xMax; ++x)
                {
                    for (int y = gridRect.yMin; y < gridRect.yMax; ++y)
                    {
                        grid[x, y] = gridObject;
                    }
                }
            }

            objectToGridMap.Add(obj, gridObject);
        }

        public void Remove(GameObject obj)
        {
            GridObject gridObject;
            if (objectToGridMap.TryGetValue(obj, out gridObject))
            {
                foreach (RectInt objRect in gridObject.rects)
                {
                    for (int i = objRect.xMin; i < objRect.xMax; ++i)
                    {
                        for (int j = objRect.yMin; j < objRect.yMax; ++j)
                        {
                            grid[i, j] = null;
                        }
                    }
                }
                objectToGridMap.Remove(obj);
            }
        }

        public void Remove(IEnumerable<RectInt> gridRects)
        {
            foreach (RectInt gridRect in gridRects)
            {
                for (int x = gridRect.xMin; x < gridRect.xMax; ++x)
                {
                    for (int y = gridRect.yMin; y < gridRect.yMax; ++y)
                    {
                        GridObject gridObject = grid[x, y];
                        if (gridObject != null)
                        {
                            foreach (RectInt objRect in gridObject.rects)
                            {
                                for (int i = objRect.xMin; i < objRect.xMax; ++i)
                                {
                                    for (int j = objRect.yMin; j < objRect.yMax; ++j)
                                    {
                                        grid[i, j] = null;
                                    }
                                }
                            }
                            objectToGridMap.Remove(gridObject.obj);
                        }
                    }
                }
            }
        }

        private bool IsWithinRange(RectInt gridRect)
        {
            return gridRect.xMin >= 0 && gridRect.yMin >= 0 &&
                   gridRect.xMax < grid.GetLength(0) && gridRect.yMax < grid.GetLength(1);
        }
    }
}
