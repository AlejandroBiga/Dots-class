using UnityEngine;

public class SelectionArea : MonoBehaviour
{
    
    [SerializeField] private RectTransform selectionAreaTransform;

    private void Start()
    {
        UnittSelectorMg.Instance.OnSelectionAreaStart += UnitSelectionMG_OnSelectionStart;
        UnittSelectorMg.Instance.OnSelectionAreaEnd += UnitSelectionMG_OnSelectionEnd;
        selectionAreaTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (selectionAreaTransform.gameObject.activeSelf)
        {
            UpdateVisual();
        }
    }

    private void UnitSelectionMG_OnSelectionStart(object sender, System.EventArgs e)
    {
        selectionAreaTransform.gameObject.SetActive(true);
        
        UpdateVisual();
    }

    private void UnitSelectionMG_OnSelectionEnd(object sender, System.EventArgs e)
    {
        selectionAreaTransform.gameObject.SetActive(false);
        
    }

    private void UpdateVisual()
    {
        Rect selectionAreaRect = UnittSelectorMg.Instance.GetSelectionAreaRect();
        selectionAreaTransform.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y);
        selectionAreaTransform.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height);
    }
}
