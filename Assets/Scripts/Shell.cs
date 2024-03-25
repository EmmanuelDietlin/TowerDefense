using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : Projectile
{
    #region Fields
    #region Serialized
    [SerializeField] private Explosion m_explosionPrefab;
    #endregion

    #region Private
    private Vector3 m_launchPoint = Vector3.zero;
    private Vector3 m_targetPoint = Vector3.zero;
    private Vector3 m_velocity = Vector3.zero;
    private float m_elapsedTime = 0f;
    private float m_damage;
    private float m_radius;
    private LayerMask m_layerMask;

    #endregion
    #endregion

    #region Properties
    public override ProjectileType Type => ProjectileType.Shell;
    #endregion

    #region Methods
    #region Public
    public void Initialize(Vector3 a_launchPoint, 
        Vector3 a_targetPoint, 
        Vector3 a_velocity, 
        float a_damage, 
        float a_radius,
        LayerMask a_enemyLayerMask)
    {
        m_launchPoint = a_launchPoint;
        m_targetPoint = a_targetPoint;
        m_velocity = a_velocity;
        m_damage = a_damage;
        m_radius = a_radius;
        m_layerMask = a_enemyLayerMask;
    }

    public override bool GameUpdate()
    {
        m_elapsedTime += Time.deltaTime;
        Vector3 p = m_launchPoint + m_velocity * m_elapsedTime;
        p.y -= .5f * Mathf.Abs(Physics.gravity.y) * m_elapsedTime * m_elapsedTime;
        transform.position = p;
        Vector3 d = m_velocity;
        d.y -= Mathf.Abs(Physics.gravity.y) * m_elapsedTime;
        transform.rotation = Quaternion.LookRotation(d);

        if (p.y < 0f)
        {
            SpawnExplosion();
            m_elapsedTime = 0f;
            Recycle();
            return false;
        }

        return true;
    }
    #endregion

    #region Private
    private void SpawnExplosion()
    {
        Explosion g = Instantiate(m_explosionPrefab);
        g.Initialize(m_targetPoint, m_radius, m_damage, m_layerMask);
        GameManager.Instance.AddNonEnemyEntity(g);
    }
    #endregion
    #endregion
}
