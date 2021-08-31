using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodePiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int value;
    public Point index;

    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    public RectTransform rect;

    bool updating;

    public void Start()
    {
        if(GetComponent<Animator>()!= null)
        GetComponent<Animator>().SetFloat("Offset", Random.Range(0.0f, 2.0f)); //Random animations
    }
    public void Initialize(int v , Point p)
    {
        rect = GetComponent<RectTransform>();
  
        value = v;
        SetIndex(p);
    }
    public void SetIndex(Point p)
    {
        index = p;
        ResetPosition();
        UpdateName();
    }
    public void ResetPosition()
    {
        pos = new Vector2(128 + (176 * index.x), -128 - (178 * index.y));
    }

    public GameObject GetGameobject()
    {
        return this.gameObject;
    }
    public void MovePositionTo(Vector2 move)
    {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, move, Time.deltaTime * 16f);
    }
    
    public void MovePosition(Vector2 move)
    {
        rect.anchoredPosition += move * Time.deltaTime * 16f;
    }
    public bool UpdatePice()
    {
        if(Vector3.Distance(rect.anchoredPosition, pos) > 1)
        {
            MovePositionTo(pos);
            updating = true;
            return true;
        }
        else
        {
            rect.anchoredPosition = pos;
            updating = false;
            return false;
        }
    }
    void UpdateName()
    {
        transform.name = "Node [" +  (index.x + 1)  + ", " + (6 - index.y) + "]";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (updating) return;
        MovePieces.instance.MovePiece(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MovePieces.instance.DropPiece();
    }
}
