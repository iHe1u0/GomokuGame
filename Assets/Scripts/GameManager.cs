using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Stone currentTurn = Stone.Black; // Black first
    public Text statusText; // optional UI text
    public Button restartButton;

    bool gameOver = false;
    public bool playerIsBlack = true;
    public bool vsAI = true;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        UpdateStatus();
    }

    void UpdateStatus()
    {
        if (statusText != null)
        {
            if (gameOver) statusText.text = "游戏结束";
            else statusText.text = (currentTurn == Stone.Black ? "黑方回合" : "白方回合");
        }
    }

    public void HandlePlayerMove(int x, int y)
    {
        if (gameOver) return;
        if (vsAI && currentTurn == Stone.White && playerIsBlack) return; // wait AI

        Cell c = BoardManager.Instance.GetCell(x, y);
        if (c == null || c.state != Stone.Empty) return;

        c.SetState(currentTurn);
        if (CheckWin(x, y, currentTurn))
        {
            gameOver = true;
            if (statusText) statusText.text = (currentTurn == Stone.Black ? "黑方胜利!" : "白方胜利!");
            return;
        }

        // swap turn
        currentTurn = (currentTurn == Stone.Black) ? Stone.White : Stone.Black;
        UpdateStatus();

        if (vsAI && !gameOver)
        {
            if ((playerIsBlack && currentTurn == Stone.White) || (!playerIsBlack && currentTurn == Stone.Black))
            {
                StartCoroutine(AIMoveRoutine());
            }
        }
    }

    IEnumerator AIMoveRoutine()
    {
        yield return new WaitForSeconds(0.2f); // small delay
        Vector2Int move = AI_FindBestMove();
        if (move.x >= 0)
        {
            Cell c = BoardManager.Instance.GetCell(move.x, move.y);
            c.SetState(currentTurn);
            if (CheckWin(move.x, move.y, currentTurn))
            {
                gameOver = true;
                if (statusText) statusText.text = (currentTurn == Stone.Black ? "黑方胜利!" : "白方胜利!");
                yield break;
            }

            currentTurn = (currentTurn == Stone.Black) ? Stone.White : Stone.Black;
            UpdateStatus();
        }
    }

    // Simple AI: if can win in one move -> take it.
    // Else if opponent can win next -> block.
    // Else random.
    Vector2Int AI_FindBestMove()
    {
        int w = BoardManager.Instance.width;
        int h = BoardManager.Instance.height;

        Stone aiStone = currentTurn;
        Stone opp = aiStone == Stone.Black ? Stone.White : Stone.Black;

        // 1) win if possible
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var cell = BoardManager.Instance.GetCell(x, y);
                if (cell.state != Stone.Empty) { continue; }
                cell.SetState(aiStone);
                bool win = CheckWin(x, y, aiStone);
                cell.SetState(Stone.Empty);
                if (win) { return new Vector2Int(x, y); }
            }
        }
        // 2) block opponent win
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var cell = BoardManager.Instance.GetCell(x, y);
                if (cell.state != Stone.Empty) continue;
                cell.SetState(opp);
                bool win = CheckWin(x, y, opp);
                cell.SetState(Stone.Empty);
                if (win) return new Vector2Int(x, y);
            }
        }

        // 3) else choose center-proximal random
        List<Vector2Int> empties = new List<Vector2Int>();
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (BoardManager.Instance.GetCell(x, y).state == Stone.Empty)
                {
                    empties.Add(new Vector2Int(x, y));
                }
            }
        }
        if (empties.Count == 0) { return new Vector2Int(-1, -1); }
        // prefer central positions
        empties.Sort((a, b) =>
        {
            float da = Mathf.Abs(a.x - w / 2f) + Mathf.Abs(a.y - h / 2f);
            float db = Mathf.Abs(b.x - w / 2f) + Mathf.Abs(b.y - h / 2f);
            return da.CompareTo(db);
        });
        // pick from top few
        int topN = Mathf.Max(1, Mathf.Min(10, empties.Count));
        return empties[Random.Range(0, topN)];
    }

    // Check win from position (x,y) for stone s
    public bool CheckWin(int x, int y, Stone s)
    {
        if (s == Stone.Empty) { return false; }
        // directions: (1,0), (0,1), (1,1), (1,-1)
        Vector2Int[] dirs = new Vector2Int[] {
            new Vector2Int(1,0),
            new Vector2Int(0,1),
            new Vector2Int(1,1),
            new Vector2Int(1,-1)
        };

        foreach (var d in dirs)
        {
            int count = 1;
            // positive direction
            count += CountDir(x, y, d.x, d.y, s);
            // negative direction
            count += CountDir(x, y, -d.x, -d.y, s);
            if (count >= 5) return true;
        }
        return false;
    }

    int CountDir(int x, int y, int dx, int dy, Stone s)
    {
        int cnt = 0;
        int nx = x + dx, ny = y + dy;
        while (true)
        {
            var c = BoardManager.Instance.GetCell(nx, ny);
            if (c == null) break;
            if (c.state == s) { cnt++; nx += dx; ny += dy; }
            else break;
        }
        return cnt;
    }

    public void RestartGame()
    {
        gameOver = false;
        currentTurn = Stone.Black;
        foreach (var c in BoardManager.Instance.AllCells())
        {
            c.SetState(Stone.Empty);
        }
        UpdateStatus();
    }
}
