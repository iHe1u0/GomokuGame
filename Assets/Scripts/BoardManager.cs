using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("Board")]
    public int width = 15;
    public int height = 15;
    public float cellSize = 0.6f;
    public GameObject cellPrefab;

    Cell[,] cells;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        // cleanup existing
        foreach (Transform t in transform) Destroy(t.gameObject);

        cells = new Cell[width, height];
        Vector2 origin = new Vector2(-width / 2f * cellSize + cellSize / 2f, -height / 2f * cellSize + cellSize / 2f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject go = Instantiate(cellPrefab, transform);
                go.transform.localPosition = origin + new Vector2(x * cellSize, y * cellSize);
                Cell c = go.GetComponent<Cell>();
                c.x = x;
                c.y = y;
                c.SetState(Stone.Empty);
                cells[x, y] = c;
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return null;
        return cells[x, y];
    }

    public void OnCellClicked(Cell cell)
    {
        GameManager.Instance.HandlePlayerMove(cell.x, cell.y);
    }

    public IEnumerable<Cell> AllCells()
    {
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                yield return cells[x, y];
    }
}
