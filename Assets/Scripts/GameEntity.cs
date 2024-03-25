using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public abstract class GameEntity : MonoBehaviour
{
    #region Fields
    #region Private

    #endregion
    #region Serialized

    #endregion
    #endregion

    #region Properties
    #endregion

    #region Methods
    #region Private

    #endregion

    #region Public
    public virtual bool GameUpdate() => true;
    #endregion
    #region Lifecycle

    #endregion
    #endregion
}
