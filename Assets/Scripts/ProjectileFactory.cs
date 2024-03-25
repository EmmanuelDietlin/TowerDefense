using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{
    #region Fields
    #region Serialized
    [Header("Projectile pool")]
    [SerializeField] private ProjectilePool m_pool;

    [Header("Shell")]
    [SerializeField] private Shell m_shellPrefab;
    [SerializeField] private int m_shellPoolSize = 20;
    #endregion

    #region Private
    private static ProjectileFactory m_instance;
    private Dictionary<ProjectileType, ProjectilePool> m_pools = new Dictionary<ProjectileType, ProjectilePool>();
    #endregion
    #endregion

    #region Properties
    public static ProjectileFactory Instance => m_instance;
    #endregion

    #region Methods
    #region Public
    public void Reclaim(Projectile a_projectile)
    {
        m_pools[a_projectile.Type].Push(a_projectile);
    }

    public Projectile Get(ProjectileType a_type)
    {
        return m_pools[a_type].Pop();
    }
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;
        }

        ProjectilePool pool = Instantiate(m_pool);
        pool.Initialize(m_shellPoolSize, m_shellPrefab);
        pool.transform.SetParent(transform, false);
        m_pools[ProjectileType.Shell] = pool;
    }
    #endregion
    #endregion
}
