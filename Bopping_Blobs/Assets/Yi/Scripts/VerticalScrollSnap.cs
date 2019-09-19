// Reference: https://forum.unity.com/threads/unity-ui-scroll-through-items-one-by-one.294523/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class VerticalScrollSnap : MonoBehaviour, IDragHandler, IEndDragHandler
{
    // The currently selected inventory "bag"
    public Transform selectedInventoryPanel;

    //Top padding and vertical spacing between icons
    public int paddingAndSpacing = 10;

    private int columnCount = 1;
    private List<Vector3> scrollPositions;
    private List<GameObject> scrollItems;
    private ScrollRect scrollRect;
    private Vector3 verticalLerpTarget;
    private bool isLerpingVertical;
    private float scrollWidth;
    private RectTransform childRect;

    void Start()
    {
        scrollWidth = (transform as RectTransform).sizeDelta.x;
        scrollRect = gameObject.GetComponent<ScrollRect>();
        scrollRect.inertia = false;
        isLerpingVertical = false;


        scrollPositions = new List<Vector3>();
        scrollItems = new List<GameObject>();
        CalculateChildren();

        childRect = (transform.GetChild(0).transform as RectTransform);
        (selectedInventoryPanel as RectTransform).position.Set(0, 0, 0);
    }

    void CalculateChildren()
    {
        scrollPositions.Clear();
        if (selectedInventoryPanel.childCount > 0)
        {
            int screens = Mathf.CeilToInt((float)selectedInventoryPanel.childCount / (float)columnCount);
            float imgSize = selectedInventoryPanel.GetComponent<GridLayoutGroup>().cellSize.y;

            for (int i = 0; i < screens; ++i)
            {
                int add = paddingAndSpacing;
                if (i == 0)
                {
                    add = paddingAndSpacing;
                }
                scrollPositions.Add(new Vector3(0, i * (imgSize + add), 0f));
                scrollItems.Add(selectedInventoryPanel.GetChild(i).gameObject);
            }
        }
    }

    void Update()
    {
        if (isLerpingVertical)
        {
            selectedInventoryPanel.localPosition = Vector3.Lerp(selectedInventoryPanel.localPosition, verticalLerpTarget, 10 * Time.deltaTime);
            //for(int i = 0; i <scrollPositions.Count;i++)
            //{
            //    if(scrollItems.Contains)
            //}
            if (Vector3.Distance(selectedInventoryPanel.localPosition, verticalLerpTarget) < 0.001f)
            {
                isLerpingVertical = false;
            }
        }
    }

    Vector3 FindClosestFrom(Vector3 start, List<Vector3> positions)
    {
        Vector3 closest = Vector3.zero;
        float distance = Mathf.Infinity;

        foreach (Vector3 position in positions)
        {
            Vector3 pos = position;
            if (Vector3.Distance(start, pos) < distance)
            {
                distance = Vector3.Distance(start, pos);
                closest = pos;
            }
        }

        return closest;
    }

    public void OnDrag(PointerEventData data)
    {
        isLerpingVertical = false;
    }


    public void OnEndDrag(PointerEventData data)
    {
        if (scrollRect.vertical)
        {
            verticalLerpTarget = FindClosestFrom(selectedInventoryPanel.localPosition, scrollPositions);
            isLerpingVertical = true;
        }
    }
}