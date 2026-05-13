using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    #region SINGLETON
    public static VFXManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    #endregion

    #region POOL STRUCTURE

    private class PooledVFX
    {
        public ParticleSystem ps;
        public bool inUse;
        public bool constructionLocked;
    }

    private Dictionary<ParticleSystem, List<PooledVFX>> poolsVFX = new();
    private Dictionary<ParticleSystem, Transform> hammerCache = new();

    #endregion

    #region VARIABLES

    [Header("Particles System")]
    [SerializeField] private ParticleSystem constructingParticles;
    [SerializeField] private ParticleSystem broodParticles;

    [Header("SFX")]
    [SerializeField] private StudioEventEmitter creationEmitter;

    #endregion

    #region CORE POOL

    public ParticleSystem GetParticle(ParticleSystem prefab)
    {
        if (!poolsVFX.ContainsKey(prefab))
            poolsVFX[prefab] = new List<PooledVFX>();

        var pool = poolsVFX[prefab];

        pool.RemoveAll(p => p == null || p.ps == null);

        foreach (var item in pool)
        {
            if (!item.inUse && !item.constructionLocked)
            {
                item.inUse = true;
                return item.ps;
            }
        }

        ParticleSystem newPS = Instantiate(prefab);

        pool.Add(new PooledVFX
        {
            ps = newPS,
            inUse = true,
            constructionLocked = false
        });

        return newPS;
    }

    private PooledVFX GetPoolItem(ParticleSystem ps, ParticleSystem prefab)
    {
        if (!poolsVFX.ContainsKey(prefab)) return null;

        foreach (var item in poolsVFX[prefab])
        {
            if (item.ps == ps)
                return item;
        }

        return null;
    }

    #endregion

    #region GENERIC PARTICLES

    public void PlayParticleWithTime(ParticleSystem prefab, Vector3 position, float duration)
    {
        ParticleSystem ps = GetParticle(prefab);

        ps.transform.position = position;

        var main = ps.main;
        main.duration = duration;
        main.startLifetime = duration;

        ps.Clear();
        ps.Play();
    }

    public void PlayParticle(ParticleSystem prefab, Vector3 position)
    {
        ParticleSystem ps = GetParticle(prefab);

        ps.transform.position = position;

        ps.Clear();
        ps.Play();
    }

    #endregion


    #region RELEASE LOGIC

    private IEnumerator ReleaseWhenFinished(ParticleSystem ps, PooledVFX poolItem, Transform hammer)
    {
        yield return new WaitUntil(() => ps == null || (!ps.IsAlive(true) && !ps.isEmitting));

        if (hammer != null)
            hammer.gameObject.SetActive(false);

        if (poolItem != null)
        {
            poolItem.inUse = false;
            poolItem.constructionLocked = false;
        }
    }

    #endregion


    #region HAMMER CACHE

    private Transform GetHammer(ParticleSystem ps)
    {
        if (!hammerCache.TryGetValue(ps, out Transform hammer) || hammer == null)
        {
            hammer = ps.transform.Find("Hammer");

            if (hammer != null)
                hammerCache[ps] = hammer;
        }

        return hammer;
    }

    #endregion

    #region CONSTRUCTION VFX

    public void PlayConstructionParticles(Vector3 position, float duration)
    {
        WorldUIManager.Instance.ShowTimer(position, duration);

        ParticleSystem ps = GetParticle(constructingParticles);
        ps.transform.position = position;

        var poolItem = GetPoolItem(ps, constructingParticles);
        if (poolItem == null) return;

        poolItem.inUse = true;
        poolItem.constructionLocked = true;

        Transform hammer = GetHammer(ps);

        if (hammer != null)
        {
            hammer.gameObject.SetActive(true);
        }

        
        if (TimeManager.Instance)
        {
            TimeManager.Instance.OneShotTimer(duration, () =>
            {
                if (ps != null)
                {
                    ps.Clear();
                    ps.Play();

                    StartCoroutine(ReleaseWhenFinished(ps, poolItem, hammer));
                }

                if (hammer != null)
                    hammer.gameObject.SetActive(false);

                if (SFXManager.instance != null)
                    SFXManager.PlaySFX(creationEmitter);

            });
        }
    }

    public void PlayBroodingChamberParticles(Vector3 position, float duration)
    {
        WorldUIManager.Instance.ShowTimerAnts(position, duration);

        
        if (TimeManager.Instance)
        {
            TimeManager.Instance.OneShotTimer(duration, () =>
            {

                if (SFXManager.instance != null)
                    SFXManager.PlaySFX(creationEmitter);

            });
        }
    }
    #endregion
}
