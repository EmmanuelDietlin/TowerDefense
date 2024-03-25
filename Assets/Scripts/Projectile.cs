using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : GameEntity
{
    #region Fields

    #endregion

    #region Properties
    public abstract ProjectileType Type { get; }
    #endregion

    #region Methods
    #region Public
    public void Recycle()
    {
        ProjectileFactory.Instance.Reclaim(this);
    }
    #endregion
    #endregion
}

public enum ProjectileType
{
    Shell
}
