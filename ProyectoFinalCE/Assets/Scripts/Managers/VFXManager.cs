using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    #region VARIABLES
    public static VFXManager Instance;
    private Dictionary<ParticleSystem, List<ParticleSystem>> poolsVFX = new();

    [Header("Particles System")]
    [SerializeField] ParticleSystem constructingParticles;

    #endregion

    private void Awake()
    {
        Instance = this;
    }

    #region GENERIC_PARTICLES
    /// <summary>
    /// Busca partículas que no están emitiendo dentro del pool (diccionario) que tenemos para reutilizarlas y si no encuentra, genera nuevas.
    /// </summary>
    /// <param name="prefab">Prefab de las particles a buscar.</param>
    /// <returns></returns>
    public ParticleSystem GetParticle(ParticleSystem prefab)
    {
        if (!poolsVFX.ContainsKey(prefab))
        {
            poolsVFX[prefab] = new List<ParticleSystem>();
        }

        var pool = poolsVFX[prefab];

        // Buscar uno libre
        foreach (var ps in pool)
        {
            if (!ps.isPlaying)
                return ps;
        }

        // Crear nuevo si todos ocupados
        ParticleSystem newPS = Instantiate(prefab);
        pool.Add(newPS);

        return newPS;
    }

    /// <summary>
    /// Forma general de activar un prefab de particles y modificar su tiempo y posición.
    /// </summary>
    /// <param name="prefab">Sistema particles a usar.</param>
    /// <param name="position">Nueva posición de emisión.</param>
    /// <param name="duration">Duración total de la emisión.</param>
    public void PlayParticle(ParticleSystem prefab, Vector3 position, float duration)
    {
        ParticleSystem ps = GetParticle(prefab);

        ps.transform.position = position;

        var main = ps.main;
        main.duration = duration;
        main.startLifetime = duration;

        ps.Clear();
        ps.Play();
    }

    #endregion

    #region SPECIFIC_MPA

    public void PlayConstructionParticles(Vector3 position, float duration)
    {
        // Partículas
        PlayParticle(constructingParticles, position, duration);

        // UI (delegado)
        WorldUIManager.Instance.ShowTimer(position, duration);
    }

    #endregion
}
