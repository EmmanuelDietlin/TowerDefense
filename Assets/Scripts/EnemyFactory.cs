using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    #region Fields
    #region Serialized
    [SerializeField] private EnemyPool m_enemyPoolPrefab;
    [SerializeField] private Enemy m_enemyPrefab;
    [SerializeField, Min(0)] private int m_enemyPoolSize = 10;
    [SerializeField, FloatRangeSlider(.5f, 1f)] private FloatRange m_scale = new FloatRange(1f);
    [SerializeField, FloatRangeSlider(-.4f, .4f)] private FloatRange m_pathOffset = new FloatRange(0f);
    [SerializeField, FloatRangeSlider(.5f, 2f)] private FloatRange m_speed = new FloatRange(1f);
    #endregion

    #region Private
    private EnemyPool m_enemyPool;
    private static EnemyFactory m_instance;
    #endregion
    #endregion

    #region Properties
    public static EnemyFactory Instance => m_instance;
    public Enemy GetEnemy() => m_enemyPool.Pop();
    public void Reclaim(Enemy a_enemy) => m_enemyPool.Push(a_enemy);
    #endregion

    #region Methods
    #region Public
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
        m_enemyPool = Instantiate(m_enemyPoolPrefab);
        m_enemyPool.Initialize(m_enemyPoolSize, m_enemyPrefab, m_scale, m_pathOffset, m_speed);
    }
    #endregion
    #endregion
}
