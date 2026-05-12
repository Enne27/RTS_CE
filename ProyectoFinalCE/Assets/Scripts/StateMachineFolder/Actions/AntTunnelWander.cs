using System.Collections.Generic;
using UnityEngine;

public class AntTunnelWander : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float reachDistance = 0.05f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Tunnel Detection")]
    [SerializeField] private float tunnelSearchRadius = 0.25f;
    [SerializeField] private LayerMask tunnelLayer;

    [Header("Behaviour")]
    [SerializeField] private Vector2 waitTimeRange = new Vector2(0.5f, 2.5f);
    [SerializeField] private float decisionChancePerSecond = 1f;

    [Header("Debug")]
    [SerializeField] private TunnelFunction currentTunnel;
    [SerializeField] private TunnelFunction targetTunnel;

    private TunnelFunction previousTunnel;

    private float waitTimer;
    private bool isWaiting;

    private void Start()
    {
        currentTunnel = FindCurrentTunnel();
    }

    private void Update()
    {
        if (currentTunnel == null)
        {
            currentTunnel = FindCurrentTunnel();
            return;
        }

        // =========================
        // ESTADO: ESPERANDO
        // =========================
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                targetTunnel = null;
            }

            return;
        }

        // =========================
        // DECISIÓN DE MOVIMIENTO
        // =========================
        if (targetTunnel == null)
        {
            float roll = Random.value;

            if (roll < decisionChancePerSecond * Time.deltaTime)
            {
                ChooseNextTunnel();
            }
            else
            {
                StartWaiting();
                return;
            }
        }

        // =========================
        // MOVIMIENTO
        // =========================
        if (targetTunnel != null)
        {
            MoveToTunnel();
        }
    }

    private void MoveToTunnel()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = targetTunnel.transform.position;

        Vector2 dir = (targetPos - currentPos).normalized;

        // SOLO ROTAR SI SE ESTÁ MOVIENDO DE VERDAD
        bool isMoving = Vector2.Distance(currentPos, targetPos) > reachDistance;

        if (isMoving && dir != Vector2.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Quaternion baseRotation = Quaternion.Euler(0, 90f, -90f);
            Quaternion lookRotation = Quaternion.Euler(0, 0, angle);

            Quaternion targetRotation = lookRotation * baseRotation;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // =========================
        // LLEGADA AL TÚNEL
        // =========================
        if (Vector2.Distance(transform.position, targetPos) <= reachDistance)
        {
            previousTunnel = currentTunnel;
            currentTunnel = targetTunnel;
            targetTunnel = null;
        }
    }

    private void ChooseNextTunnel()
    {
        List<TunnelFunction> connections = currentTunnel.TunnelConnections;

        if (connections == null || connections.Count == 0)
            return;

        List<TunnelFunction> possibleTunnels = new();

        foreach (TunnelFunction tunnel in connections)
        {
            if (tunnel != previousTunnel)
                possibleTunnels.Add(tunnel);
        }

        if (possibleTunnels.Count == 0)
            possibleTunnels = connections;

        int randomIndex = Random.Range(0, possibleTunnels.Count);

        targetTunnel = possibleTunnels[randomIndex];
    }

    private void StartWaiting()
    {
        isWaiting = true;
        waitTimer = Random.Range(waitTimeRange.x, waitTimeRange.y);
        targetTunnel = null;
    }

    private TunnelFunction FindCurrentTunnel()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            tunnelSearchRadius,
            tunnelLayer
        );

        foreach (Collider hit in hits)
        {
            TunnelFunction tunnel = hit.GetComponentInParent<TunnelFunction>();

            if (tunnel != null)
                return tunnel;
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        if (targetTunnel != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetTunnel.transform.position);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, tunnelSearchRadius);
    }
}