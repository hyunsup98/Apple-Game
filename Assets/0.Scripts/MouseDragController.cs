using UnityEngine;
using System.Collections.Generic;

public class MouseDragController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AppleSpawner appleSpawner;
    [SerializeField] private RectTransform dragRectangle;
    [SerializeField] private RectTransform appleParent;     //appleParent 오브젝트 기준으로 마우스, 사과의 로컬 좌표 계산

    private Canvas canvas;
    private Rect dragRect;
    private Vector2 start = Vector2.zero;
    private Vector2 end = Vector2.zero;
    private List<Apple> selectedAppleList = new List<Apple>();
    private int sum = 0;

    private void Awake()
    {
        canvas = dragRectangle.GetComponentInParent<Canvas>();
        dragRect = new Rect();

        DrawDragRectangle();
    }

    private void Update()
    {
        if (!gameManager.isGameStart) return;

        if (Input.GetMouseButtonDown(0))
        {
            start = ScreenToCanvasPos(Input.mousePosition);
            dragRect.Set(0, 0, 0, 0);
        }

        if (Input.GetMouseButton(0))
        {
            end = ScreenToCanvasPos(Input.mousePosition);

            DrawDragRectangle();
            CalculateDragRect();
            SelectApples();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (sum == 10)
            {
                foreach (Apple apple in selectedAppleList)
                {
                    appleSpawner.DestroyApple(apple);
                }
                gameManager.AddScore(selectedAppleList.Count);
            }
            else
            {
                foreach (Apple apple in selectedAppleList)
                {
                    apple.OnDeselected();
                }
            }
            start = end = Vector2.zero;
            DrawDragRectangle();
        }
    }

    private Vector2 ScreenToCanvasPos(Vector2 ScreenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            appleParent,
            ScreenPos,
            canvas.worldCamera,
            out Vector2 localPos
            );

        return localPos;
    }

    public void Init()
    {
        if(selectedAppleList.Count > 0)
        {
            foreach(Apple apple in selectedAppleList)
            {
                apple.OnDeselected();
            }
        }
        dragRectangle.sizeDelta = Vector2.zero;
    }

    private void DrawDragRectangle()
    {
        dragRectangle.anchoredPosition = (start + end) * 0.5f;
        dragRectangle.sizeDelta = new Vector2(Mathf.Abs(start.x - end.x), Mathf.Abs(start.y - end.y));
    }

    private void CalculateDragRect()
    {
        Vector2 mousePos = ScreenToCanvasPos(Input.mousePosition);
        if (mousePos.x < start.x)
        {
            dragRect.xMin = mousePos.x;
            dragRect.xMax = start.x;
        }
        else
        {
            dragRect.xMin = start.x;
            dragRect.xMax = mousePos.x;
        }

        if (mousePos.y < start.y)
        {
            dragRect.yMin = mousePos.y;
            dragRect.yMax = start.y;
        }
        else
        {
            dragRect.yMin = start.y;
            dragRect.yMax = mousePos.y;
        }
    }

    private void SelectApples()
    {
        sum = 0;
        selectedAppleList.Clear();

        foreach (Apple apple in appleSpawner.AppleList)
        {
            if (IsAppleOverlapping(apple))
            {
                apple.OnSelected();
                selectedAppleList.Add(apple);
                sum += apple.Number;
            }
            else
            {
                apple.OnDeselected();
            }
        }
    }

    private bool IsAppleOverlapping(Apple apple)
    {
        Vector2 applePos = apple.rect.anchoredPosition;
        Vector2 appleSize = apple.rect.sizeDelta;

        Rect appleRect = new Rect(applePos.x - appleSize.x * 0.5f, applePos.y - appleSize.y * 0.5f, appleSize.x, appleSize.y);

        return dragRect.Overlaps(appleRect);
    }
}
