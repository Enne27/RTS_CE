using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class ExtraSoundsSFX : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] float paramValueMin = 0f;
    [SerializeField] float paramValueMax = 1f;

    [Tooltip("Límites que queremos.")]
    [SerializeField] SplineContainer spline;

    [SerializeField] Transform cameraTransform;


    [Header("Distancias")]
    [SerializeField] float maxDistance = 100f;
    [SerializeField] float minDistance = 5f;

    [Header("Suavizado")]
    [SerializeField] float smoothSpeed = 5f;

    float currentValue;

    public bool canSound = true;
    #endregion

    #region "SINGLETON"
    public static ExtraSoundsSFX instance { get; private set; }
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        if (spline == null || cameraTransform == null)
            return;

        if (!canSound)
        {
            currentValue = 0f;
            WaterScript.instance.SetStrengthValue(currentValue);
            return;
        }

        float3 nearestPointLocal;
        float t;

        // pasar la cámara a espacio LOCAL del spline
        float3 localCamPos = spline.transform.InverseTransformPoint(cameraTransform.position);

        //  calcular punto más cercano en LOCAL
        SplineUtility.GetNearestPoint(spline.Spline, localCamPos, out nearestPointLocal, out t);

        // volver a WORLD
        Vector3 nearestPointWorld = spline.transform.TransformPoint(nearestPointLocal);

        // ignorar altura 
        Vector3 camPos = cameraTransform.position;
        Vector3 splinePos = nearestPointWorld;

        camPos.y = 0f;
        splinePos.y = 0f;

        float distance = Vector3.Distance(camPos, splinePos);

        // FILTRO: fuera de rango
        if (distance > maxDistance)
        {
            currentValue = Mathf.Lerp(currentValue, paramValueMin, Time.deltaTime * smoothSpeed);
            WaterScript.instance.SetStrengthValue(currentValue);
            return;
        }

        // NORMALIZAR
        float normalized = Mathf.InverseLerp(maxDistance, minDistance, distance);
        normalized = Mathf.Clamp01(normalized);

        // CURVA 
        normalized = Mathf.Pow(normalized, 0.4f);

        float targetValue = Mathf.Lerp(paramValueMin, paramValueMax, normalized);

        // SUAVIZADO
        currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * smoothSpeed);

        WaterScript.instance.SetStrengthValue(currentValue);

        // debug
        Debug.DrawLine(cameraTransform.position, nearestPointWorld, Color.red);
        Debug.DrawRay(nearestPointWorld, Vector3.up * 2f, Color.green);

        Debug.Log($"Dist: {distance} | Norm: {normalized} | Value: {currentValue}");
    }
}
