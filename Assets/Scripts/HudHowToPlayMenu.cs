using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class HudHowToPlayMenu : MonoBehaviour, IGameMenu
{
    [SerializeField] private Transform menuParent;
    [SerializeField] private List<Transform> headers;
    [SerializeField] private List<Transform> pages;
    [SerializeField] private HudPageDisplay pageDisplay;
    [SerializeField] private CanvasGroup pageAnimCG;

    private const float PAGE_ANIM_DISPLACEMENT = 150f;
    private const float PAGE_ANIM_DURATION = 0.25f;

    private int currentPage;

    public bool CanBeOpened()
    {
        return true;
    }

    public void OnCloseClicked()
    {
        CloseMenu();
    }
    public void OnNextClicked()
    {
        currentPage++;
        if (currentPage >= headers.Count) 
        {
            currentPage = 0;
        }
        UpdatePage();
        PlayPageAnim(true);
    }
    public void OnPrevClicked()
    {
        currentPage--;
        if (currentPage < 0)
        {
            currentPage = headers.Count-1;
        }
        UpdatePage();
        PlayPageAnim(false);
    }

    private void UpdatePage() 
    {
        for (int i = 0; i < headers.Count; i++) { headers[i].gameObject.SetActive(currentPage == i); }
        for (int i = 0; i < pages.Count; i++) { pages[i].gameObject.SetActive(currentPage == i); }

        pageDisplay.UpdateDisplay(headers.Count, currentPage);
    }

    public void CloseMenu()
    {
        EventManager.OnUiMenuClosed(this);
        menuParent.gameObject.SetActive(false);
    }

    public bool IsOpen()
    {
        return menuParent.gameObject.activeInHierarchy;
    }

    public void OpenMenu()
    {
        currentPage = 0;
        UpdatePage();
        PlayPageAnim(true);
        PersistentDataManager.DeletePreviousStageRecord();
        menuParent.gameObject.SetActive(true);
    }
    private void PlayPageAnim(bool next)
    {
        StopAllCoroutines();
        StartCoroutine(PageAnim(next));
    }
    IEnumerator PageAnim(bool next)
    {
        float t = 0;
        while (t < 1)
        {
            t = Mathf.MoveTowards(t, 1, Time.unscaledDeltaTime / PAGE_ANIM_DURATION);
            UpdatePageAnim(t, next);
            yield return null;
        }
    }
    private void UpdatePageAnim(float t, bool next)
    {
        pageAnimCG.transform.localPosition = (1 - t) * (next ? PAGE_ANIM_DISPLACEMENT : -PAGE_ANIM_DISPLACEMENT) * Vector3.right;
        pageAnimCG.alpha = t;
    }

    public string GetMenuNameID()
    {
        return "MENU_NAME_HOW_TO_PLAY";
    }
}
