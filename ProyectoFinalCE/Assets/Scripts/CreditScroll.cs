using UnityEngine;
using UnityEngine.InputSystem;

public class CreditScroll : MonoBehaviour
{
    #region VARIABLES
    [SerializeField, Tooltip("Velocidad normal del scroll.")] float scrollSpeed = 20;
    [SerializeField, Tooltip("Multiplicado de velocidad.")] float multiplyFastSpeed = 2;

    float doubleSpeed;
    float initialSpeed;

    [SerializeField, Tooltip("Vista de la escena.")] CreditsView view;

    [Header("Effects values")]

    [Tooltip("Distancia a la que quiero que se active el bot�n cuando los cr�ditos lleguen.")] 
    [SerializeField]float yDistance;

    Vector2 lastMousePos;
    [SerializeField] float hideDelay = 2f;
    bool buttonsShown = false;
    bool hideTimerRunning = false;
    bool showTimerRunning = false;

    #endregion

    private void Awake()
    {
        doubleSpeed = scrollSpeed * multiplyFastSpeed;
        initialSpeed = scrollSpeed;

        lastMousePos = Mouse.current.position.ReadValue();
    }

    void Update()
    {
        // Con el translate podemos mover hacia donde queramos.
        transform.Translate(Vector3.up * scrollSpeed * Time.unscaledDeltaTime);

        if (Keyboard.current.anyKey.isPressed || Mouse.current.leftButton.isPressed ||
        Mouse.current.rightButton.isPressed || Mouse.current.middleButton.isPressed)
        {
            scrollSpeed = doubleSpeed;
        }
        else scrollSpeed = initialSpeed;

        bool mouseMoved = Mouse.current.position.ReadValue() != lastMousePos;

        if (transform.position.y >= yDistance || mouseMoved)
        {
            ShowButtons();
        }

        if(!hideTimerRunning)
            return;
        else HideButtons();

        lastMousePos = Mouse.current.position.ReadValue();
    }

    private void HideButtons() 
    {
        if (view == null) return;

        TimeManager.Instance.OneShotTimer(hideDelay, ()=>
        {
            buttonsShown = false;
            hideTimerRunning = false;
            view.ActivateButtons(false);
        });
    }
    private void ShowButtons()
    {
        if (!buttonsShown && (!showTimerRunning || !hideTimerRunning) && view != null)
        {
            showTimerRunning = true;

            TimeManager.Instance.OneShotTimer(hideDelay, () => 
            {
                if (view == null) return;

                buttonsShown = true;
                hideTimerRunning = true;
                showTimerRunning = false;
                view.ActivateButtons(true);
            });
        }
    }
}
