using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurret : Turret
{
    #region Fields
    #region Serialized
    [SerializeField] private Transform m_turret = default;
    [SerializeField] private Transform m_laser = default;
    [SerializeField, Range(1, 100)] private float m_damagePerSecond = 10f;
    #endregion

    #region Private
    private TargetPoint m_target;
    private Vector3 m_laserScale;
    #endregion
    #endregion

    #region Properties
    public override TurretType Type => TurretType.Laser;
    #endregion

    #region Methods
    #region Public
    public override void GameUpdate()
    {
        base.GameUpdate();
        if (TrackTarget(ref m_target) || AcquireTarget(out m_target))
        {
            Shoot();
        }
        else
        {
            m_laser.localScale = Vector3.zero;
        }
    }
    #endregion

    #region Private
    private void Shoot()
    {
        Vector3 point = m_target.Position;
        m_turret.LookAt(point);
        m_laser.localRotation = m_turret.localRotation;

        float distance = Vector3.Distance(m_turret.position, point);
        m_laserScale.z = distance;
        m_laser.localScale = m_laserScale;
        m_laser.position = m_turret.position + .5f * distance * m_laser.forward;

        m_target.Enemy.TakeDamage(m_damagePerSecond * Time.deltaTime);
    }
    #endregion

    #region Lifecycle
    private void Awake()
    {
        m_laserScale = m_laser.localScale;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (m_target != null)
        {
            Gizmos.DrawLine(transform.position, m_target.Position);
        }
    }
    #endregion
    #endregion
}
