using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditScroll : MonoBehaviour
{
    #region VARIABLES
    [SerializeField, Tooltip("Velocidad normal del scroll.")] float scrollSpeed = 20;
    [SerializeField, Tooltip("Multiplicado de velocidad.")] float multiplyFastSpeed = 2;

    float doubleSpeed;
    float initialSpeed;

    [SerializeField, Tooltip("Distancia a la que quiero que se active el botón cuando los créditos lleguen.")] float yDistance;
    [SerializeField, Tooltip("Vista de la escena.")] CreditsView view; 

    #endregion

    private void Awake()
    {
        doubleSpeed = scrollSpeed*multiplyFastSpeed;
        initialSpeed = scrollSpeed;
    }

    void Update()
    {
        // Con el translate podemos mover hacia donde queramos.
        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

        if (Input.anyKey)
        {
            scrollSpeed = doubleSpeed;
        }
        else
        {
            scrollSpeed = initialSpeed;
        }

        if (transform.position.y >= yDistance)
        {
            view.ActivateButtons();
        }
    }
}
