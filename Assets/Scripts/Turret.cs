using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class Turret : GameTileContent
{
    #region Fields
    #region Serialized
    [SerializeField, Range(1, 10)] protected float m_range;
    [SerializeField] protected LayerMask m_enemyLayer;
    #endregion

    #region Private
    #endregion
    #endregion

    #region Properties
    public abstract TurretType Type { get; }
    #endregion

    #region Methods
    #region Public
    #endregion

    #region Private
    protected bool AcquireTarget(out TargetPoint a_target)
    {
        if (TargetPoint.FillBuffer(transform.position, m_range, m_enemyLayer))
        {
            a_target = TargetPoint.RandomBuffered;
            return true;
        }
        a_target = null;
        return false;
    }

    protected bool TrackTarget(ref TargetPoint a_target)
    {
        if (a_target == null || a_target.gameObject.activeSelf == false || a_target.Enemy.Health < 0)
        {
            return false;
        }
        float x = transform.position.x - a_target.Position.x;
        float z = transform.position.z - a_target.Position.z;
        float r = m_range + a_target.Margin;
        //Equation de cercle : x² + y² = r²
        //Un point est donc dans le cercle si x² + y² <= r²
        if (x*x + z*z > r*r)
        {
            a_target = null;
            return false;
        }
        return true;
    }
    #endregion

    #region Lifecycle
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.position;
        position.y += .1f;
        Gizmos.DrawWireSphere(position, m_range);

    }
    #endregion
    #endregion
}

public enum TurretType
{
    Laser,
    Mortar
}
