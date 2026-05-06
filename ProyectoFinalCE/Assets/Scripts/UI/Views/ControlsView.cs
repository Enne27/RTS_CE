using UnityEngine;
using UnityEngine.UI;

public class ControlsView : View
{
    [Header("Buttons")]
    [SerializeField] Button backButton;

    [Header("Effects")]
    [SerializeField] float effectTime = 0.3f;
    [SerializeField] RectTransform panel;
    Vector2 shownPos;
    Vector2 hiddenPos;

    public override void Initialize()
    {
        if (backButton != null)
            backButton.onClick.AddListener(() => ViewManager.ShowLastView(1, false));

        if (panel == null) 
            panel = GetComponentInChildren<RectTransform>();

        shownPos = panel.anchoredPosition;

        hiddenPos = shownPos + new Vector2(0, -gameObject.GetComponent<RectTransform>().rect.height);

        panel.anchoredPosition = hiddenPos;
    }

    public override void Show()
    {
        base.Show();
        UIEffects.instance.SlideUI(panel, shownPos, effectTime);
    }

    public override void Hide()
    {
        UIEffects.instance.SlideUI(panel, hiddenPos, effectTime, ()=> base.Hide());
    }
}
