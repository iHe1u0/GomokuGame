using UnityEngine;
using UnityEngine.EventSystems;

public enum Stone { Empty, Black, White }

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Cell : MonoBehaviour, IPointerClickHandler
{
    public int x, y; // board coordinates
    public Stone state = Stone.Empty;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateAppearance();
    }

    public void SetState(Stone s)
    {
        state = s;
        UpdateAppearance();
    }

    void UpdateAppearance()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (state == Stone.Empty)
        {
            sr.color = new Color(1f, 1f, 1f, 0.2f); // light empty (or choose grid sprite)
        }
        else if (state == Stone.Black)
        {
            sr.color = Color.black;
        }
        else if (state == Stone.White)
        {
            sr.color = Color.white;
        }
    }

    // When clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        // notify board manager
        BoardManager.Instance.OnCellClicked(this);
    }
    void OnMouseDown()
    {
        BoardManager.Instance.OnCellClicked(this);
    }

}
