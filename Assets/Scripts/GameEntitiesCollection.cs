using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameEntitiesCollection
{
    #region Fields
    #region Private
    private List<GameEntity> m_entities = new List<GameEntity>();
    #endregion
    #endregion

    #region Properties
    public int EntityCount => m_entities.Count;
    #endregion

    #region Methods
    #region Public
    public void Add(GameEntity a_entity)
    {
        m_entities.Add(a_entity);
    }

    public void UpdateEntities()
    {
        for (int i = m_entities.Count - 1; i >= 0; i--)
        {
            if (!m_entities[i] || !m_entities[i].GameUpdate())
            {
                m_entities.RemoveAt(i);
            }
        }
    }
    #endregion
    #endregion
}
