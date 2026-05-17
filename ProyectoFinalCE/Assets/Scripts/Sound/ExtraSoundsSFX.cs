using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class ExtraSoundsSFX : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] float paramValueMin;
    [SerializeField] float paramValueMax;

    [Tooltip("Límites que queremos.")]
    [SerializeField] SplineContainer spline;

    [SerializeField] Transform cameraTransform;


    [SerializeField] float maxDistance = 20f;
    #endregion

    void Update()
    {
        float3 nearestPoint;
        float t;

        // Obtener punto más cercano del spline
        SplineUtility.GetNearestPoint(spline.Spline, cameraTransform.position, out nearestPoint, out t);

        float distance = Vector3.Distance(cameraTransform.position, nearestPoint);

        // Normalizar distancia (más cerca = más fuerte)
        float normalized = Mathf.InverseLerp(maxDistance, 0f, distance);

        float finalValue = Mathf.Lerp(paramValueMin, paramValueMax, normalized);

        WaterScript.instance.SetStrengthValue(finalValue);
    }


    /* private void OnCollisionEnter(Collision collision)
     {
         if (collision.gameObject.tag.Equals("Player"))
         {
             WaterScript.instance.SetStrengthValue(paramValueMax); // Como no tengo más de momento, está así.
         }
     }

     private void OnCollisionExit(Collision collision)
     {
         if (collision.gameObject.tag.Equals("Player"))
         {
             WaterScript.instance.SetStrengthValue(paramValueMin);
         }
     }*/
}
