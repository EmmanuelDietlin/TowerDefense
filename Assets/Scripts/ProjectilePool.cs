using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    #region Fields
    private int m_size;
    private Projectile m_projectile;
    private Stack<Projectile> m_pool;
    #endregion

    #region Methods
    public void Push(Projectile a_projectile)
    {
        if (m_pool.Count >= m_size)
        {
            Destroy(a_projectile.gameObject);
            return;
        }
        a_projectile.gameObject.SetActive(false);
        a_projectile.transform.SetParent(transform, false);
        m_pool.Push(a_projectile);
    }

    public Projectile Pop()
    {
        if (m_pool.Count == 0)
        {
            Projectile newProjectile = Instantiate(m_projectile);
            return newProjectile;
        }
        Projectile projectile = m_pool.Pop();
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public void Initialize(int a_size, Projectile a_prefab)
    {
        m_projectile = a_prefab;
        m_size = a_size;
        m_pool = new Stack<Projectile>(a_size);
        for (int i = 0; i <  m_size; i++)
        {
            Projectile p = Instantiate(m_projectile);
            p.gameObject.SetActive(false);
            p.transform.SetParent(transform, false);
            m_pool.Push(p);
        }
    }
    #endregion
}
