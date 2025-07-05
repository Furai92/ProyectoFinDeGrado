using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudPageDisplay : MonoBehaviour
{
    [SerializeField] private List<Image> pageElements;

    private const float SIZE_HIGHLIGHTED = 1f;
    private const float SIZE_NORMAL = 0.5f;

    public void UpdateDisplay(int pages, int current) 
    {
        pages = Mathf.Min(pages, pageElements.Count);
        current = Mathf.Clamp(current, 0, pages - 1);

        for (int i = 0; i < pageElements.Count; i++) 
        {
            if (pages > i)
            {
                pageElements[i].gameObject.SetActive(true);
                pageElements[i].transform.localScale = Vector3.one * (current == i ? SIZE_HIGHLIGHTED : SIZE_NORMAL);
                pageElements[i].color = current == i ? Color.white : Color.gray;
            }
            else 
            {
                pageElements[i].gameObject.SetActive(false);
            }
        }
    }
}
