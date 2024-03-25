using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileContentFactory : MonoBehaviour
{
    #region Fields
    #region Serialized
    [Header("Pool")]
    [SerializeField] private GameTileContentPool m_poolPrefab;

    [Header("Empty content")]
    [SerializeField] private GameTileContent m_emptyPrefab;
    [SerializeField, Min(0)] private int m_emptyPoolSize = 121;

    [Header("Destination content")]
    [SerializeField] private GameTileContent m_destinationPrefab;
    [SerializeField, Min(0)] private int m_destinationPoolSize = 5;

    [Header("Wall content")]
    [SerializeField] private GameTileContent m_wallPrefab;
    [SerializeField, Min(0)] private int m_wallPoolSize = 50;

    [Header("SpawnPoint content")]
    [SerializeField] private GameTileContent m_spawnPointPrefab;
    [SerializeField, Min(0)] private int m_spawnPointPoolSize = 10;

    [Header("Tower content")]
    [SerializeField] private GameTileContent m_laserTurretPrefab;
    [SerializeField] private GameTileContent m_mortarTurretPrefab;
    [SerializeField, Min(0)] private int m_turretPoolSize = 10;
    #endregion

    #region Private
    private Dictionary<GameTileContentType, GameTileContentPool> m_pools = new Dictionary<GameTileContentType, GameTileContentPool>();
    private Dictionary<TurretType, GameTileContentPool> m_turretPools = new Dictionary<TurretType, GameTileContentPool>();
    private static GameTileContentFactory m_instance;
    #endregion
    #endregion

    #region Properties
    public static GameTileContentFactory Instance
    {
        get => m_instance;
    }
    #endregion


    #region Methods
    #region Private
    #endregion

    #region Public
    public GameTileContent GetTileContent(GameTileContentType a_type)
    {
        if (a_type == GameTileContentType.Turret)
        {
            Debug.LogWarning("Use the other GetTileContent method with TowerType parameter");
            return null;
        }
        return m_pools[a_type].Pop();
    }

    public Turret GetTileContent(TurretType a_type)
    {
        return (Turret)m_turretPools[a_type].Pop();
    }

    public void Reclaim(GameTileContent a_content)
    {
        if (a_content.contentType == GameTileContentType.Turret)
        {
            Turret t = (Turret)a_content;
            m_turretPools[t.Type].Push(t);
        }
        else
        {
            m_pools[a_content.contentType].Push(a_content);
        }
    }
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (m_instance == null || m_instance == this)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GameTileContentPool emptyPool = Instantiate(m_poolPrefab);
        emptyPool.gameObject.transform.SetParent(transform, false);
        emptyPool.Initialize(m_emptyPoolSize, m_emptyPrefab);
        m_pools[GameTileContentType.Empty] = emptyPool;

        GameTileContentPool destinationPool = Instantiate(m_poolPrefab);
        destinationPool.gameObject.transform.SetParent(transform, false);
        destinationPool.Initialize(m_destinationPoolSize, m_destinationPrefab);
        m_pools[GameTileContentType.Destination] = destinationPool;

        GameTileContentPool wallPool = Instantiate(m_poolPrefab);
        wallPool.gameObject.transform.SetParent(transform, false);
        wallPool.Initialize(m_wallPoolSize, m_wallPrefab);
        m_pools[GameTileContentType.Wall] = wallPool;

        GameTileContentPool spawnPointPool = Instantiate(m_poolPrefab);
        spawnPointPool.gameObject.transform.SetParent(transform, false);
        spawnPointPool.Initialize(m_spawnPointPoolSize, m_spawnPointPrefab);
        m_pools[GameTileContentType.SpawnPoint] = spawnPointPool;

        GameTileContentPool laserTurretPool = Instantiate(m_poolPrefab);
        laserTurretPool.gameObject.transform.SetParent(transform, false);
        laserTurretPool.Initialize(m_turretPoolSize, m_laserTurretPrefab);
        m_turretPools[TurretType.Laser] = laserTurretPool;

        GameTileContentPool mortarTurretPool = Instantiate(m_poolPrefab);
        mortarTurretPool.transform.SetParent(transform, false);
        mortarTurretPool.Initialize(m_turretPoolSize, m_mortarTurretPrefab);
        m_turretPools[TurretType.Mortar] = mortarTurretPool;
    }
    #endregion


    #endregion
}
