using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    #region Fields
    #region Private
    private Enemy m_prefab;
    private int m_poolSize;
    private Stack<Enemy> m_enemyPool;
    private FloatRange m_range, m_pathOffset, m_speed;
    #endregion
    #endregion

    #region Methods
    #region Public
    public void Initialize(int a_size, Enemy a_enemyPrefab, FloatRange a_range, FloatRange a_pathOffset, FloatRange a_speed)
    {
        m_prefab = a_enemyPrefab;
        m_poolSize = a_size;
        m_enemyPool = new Stack<Enemy>(a_size);
        m_range = a_range;
        m_pathOffset = a_pathOffset;
        m_speed = a_speed;
        for (int i = 0; i < m_poolSize; i++)
        {
            Enemy enemy = Instantiate(a_enemyPrefab);
            enemy.gameObject.SetActive(false);
            enemy.transform.SetParent(transform, false);
            m_enemyPool.Push(enemy);
        }
    } 

    public void Push(Enemy a_enemy)
    {
        if (m_enemyPool.Count >=  m_poolSize)
        {
            Destroy(a_enemy.gameObject);
            return;
        }
        a_enemy.gameObject.SetActive(false);
        a_enemy.transform.SetParent(transform, false);
        m_enemyPool.Push(a_enemy);
    }

    public Enemy Pop()
    {
        if (m_enemyPool.Count == 0)
        {
            Enemy newEnemy = Instantiate(m_prefab);
            return newEnemy;
        }
        Enemy enemy = m_enemyPool.Pop();
        enemy.gameObject.SetActive(true);
        float scale = m_range.RandomValueInRange;
        float offset = m_pathOffset.RandomValueInRange;
        float speed = m_speed.RandomValueInRange;
        enemy.Initialize(scale, offset, speed);
        return enemy;
    }
    #endregion

    #region Lifecycle
    #endregion
    #endregion
}
