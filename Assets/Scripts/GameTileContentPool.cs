using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileContentPool : MonoBehaviour
{

    #region Fields
    #region Private
    private Stack<GameTileContent> m_tiles = new Stack<GameTileContent>();
    private GameTileContent m_prefab;
    private int m_size = 0;
    #endregion
    #endregion

    #region Methods
    public void Initialize(int a_size, GameTileContent a_type)
    {
        m_tiles = new Stack<GameTileContent>(a_size);
        m_prefab = a_type;
        m_size = a_size;
        for (int i = 0; i < a_size; i++)
        {
            GameTileContent content = Instantiate(m_prefab);
            content.gameObject.SetActive(false);
            content.transform.SetParent(transform, false);
            m_tiles.Push(content);
        }
    }

    public GameTileContent Pop()
    {
        if (m_tiles.Count == 0)
        {
            return Instantiate(m_prefab);
        }
        GameTileContent content = m_tiles.Pop();
        content.gameObject.SetActive(true);
        return content;
    }

    public void Push(GameTileContent a_content)
    {
        if (m_tiles.Count == m_size)
        {
            Destroy(a_content.gameObject);
            return;
        }
        a_content.gameObject.SetActive(false);
        a_content.transform.SetParent(transform, false);
        m_tiles.Push(a_content);
    }
    #endregion
}
