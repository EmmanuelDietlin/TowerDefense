using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class GameTileContent : MonoBehaviour
{
    #region Fields
    #region Serialized
    [SerializeField] private GameTileContentType m_contentType;
    [SerializeField] private bool m_blocksPath;
    #endregion

    #region Private
    private GameTileContentFactory m_originFactory;
    #endregion
    #endregion

    #region Properties
    public GameTileContentType contentType => m_contentType;
    public bool BlocksPath => m_blocksPath;
    #endregion

    #region Methods
    #region Public
    public void Recycle()
    {
        GameTileContentFactory.Instance.Reclaim(this);
    }

    public virtual void GameUpdate() { }
    #endregion

    #region Lifecycle
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
    #endregion
}
