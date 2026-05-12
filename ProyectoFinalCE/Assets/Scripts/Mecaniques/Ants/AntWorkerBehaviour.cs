using StateMachine.Runtime;
using System.Collections.Generic;
using UnityEngine;

public class AntWorkerBehaviour : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float reachDistance = 0.05f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Tunnel Detection")]
    [SerializeField] private float tunnelSearchRadius = 0.25f;
    [SerializeField] private LayerMask tunnelLayer;

    [Header("Debug")]
    [SerializeField] private TunnelFunction currentTunnel;
    [SerializeField] private TunnelFunction targetTunnel;

    public StateMachineComponent stateMachineManager;
    public TunnelFunction previousTunnel;
    public Animator animationController;
    
    public bool isMoving;

    private void Start()
    {
        stateMachineManager = GetComponent<StateMachineComponent>();
        currentTunnel = FindCurrentTunnel();
        animationController = GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentTunnel == null)
        {
            currentTunnel = FindCurrentTunnel();
            return;
        }

        isMoving = targetTunnel != null;
        animationController.SetBool("IsMoving", isMoving);

        // Escoger siguiente túnel
        if (targetTunnel == null)
        {
            ChooseNextTunnel();
        }

        // Moverse
        if (targetTunnel != null)
        {
            MoveToTunnel();
        }
    }

    private void MoveToTunnel()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = targetTunnel.transform.position;

        // Mantener Z fijo
        targetPos.z = currentPos.z;

        // Movimiento
        transform.position = Vector3.MoveTowards(
            currentPos,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // Dirección de movimiento
        Vector2 dir = (targetPos - currentPos).normalized;

        // =========================
        // ROTACIÓN ORGÁNICA
        // =========================

        if (dir != Vector2.zero)
        {
            // Ángulo real de movimiento
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // CORRECCIÓN BASE DEL MODELO (ajusta SOLO aquí una vez)
            Quaternion baseRotation = Quaternion.Euler(0, 90f, -90f);

            // Rotación hacia dirección en Z
            Quaternion lookRotation = Quaternion.Euler(0, 0, angle);

            // Combinamos base + dirección
            Quaternion targetRotation = lookRotation * baseRotation;

            // Suavizado
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Llegó al túnel
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

        // Evitar volver atrás instantáneamente
        foreach (TunnelFunction tunnel in connections)
        {
            if (tunnel != previousTunnel)
            {
                possibleTunnels.Add(tunnel);
            }
        }

        // Si no hay opciones -> usar cualquiera
        if (possibleTunnels.Count == 0)
        {
            possibleTunnels = connections;
        }

        int randomIndex = Random.Range(0, possibleTunnels.Count);

        targetTunnel = possibleTunnels[randomIndex];
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