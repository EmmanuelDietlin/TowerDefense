using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    #region Fields
    #region Serialized
    [SerializeField] private Transform m_ground = default;
    [SerializeField] private GameTile m_tile = default;
    [SerializeField] private Texture2D m_gridTexture = default;
    #endregion

    #region Private
    private Vector2Int m_size;
    private List<GameTile> m_tiles = new List<GameTile>();
    private Queue<GameTile> m_searchFrontier = new Queue<GameTile>();
    private bool m_pathShown = false;
    private bool m_gridShown = false;
    private List<GameTile> m_spawnPoints = new List<GameTile>();
    private List<GameTileContent> m_updatingTiles = new List<GameTileContent>();
    #endregion
    #endregion

    #region Properties
    public bool ShowPaths
    {
        get => m_pathShown;
        set
        {
            m_pathShown=value;
            foreach(GameTile tile in m_tiles)
            {
                tile.TogglePath(m_pathShown);
            }
        }
    }

    public bool ShowGrid
    {
        get => m_gridShown;
        set
        {
            m_gridShown=value;
            Material mat = m_ground.GetComponent<MeshRenderer>().material;
            if(m_gridShown)
            {
                mat.mainTexture = m_gridTexture;
                mat.SetTextureScale("_MainTex", m_size);
            }
            else
            {
                mat.mainTexture = null;
            }
        }
    }

    public int SpawnPointsCount => m_spawnPoints.Count;
    #endregion

    #region Methods
    #region Public
    public void Initialize(Vector2Int a_size)
    {
        m_size = a_size;
        m_ground.localScale = new Vector3(a_size.x, a_size.y, 1f);

        Vector2 offset = new Vector2((m_size.x - 1)*0.5f, (m_size.y - 1)*0.5f);

        m_tiles = new List<GameTile>(m_size.x * m_size.y);

        for (int y = 0; y < m_size.y; y++)
        {
            for (int x = 0; x < m_size.x; x++)
            {
                GameTile tile = Instantiate(m_tile);
                tile.transform.position = new Vector3(x - offset.x, 0f, y - offset.y);
                tile.transform.SetParent(m_ground, true);
                m_tiles.Add(tile);
                if (x > 0) GameTile.MakeEastWestNeighbors(tile, m_tiles[x - 1 + y * m_size.x]);
                if (y > 0) GameTile.MakeNorthSouthNeighbors(tile, m_tiles[x + (y-1) * m_size.x]);
                tile.IsAlternative = (x & 1) == 0;
                if ((y & 1) == 0) tile.IsAlternative = !tile.IsAlternative;

                tile.Content = GameTileContentFactory.Instance.GetTileContent(GameTileContentType.Empty);
            }
        }

        ToggleDestination(m_tiles[m_tiles.Count / 2]);
        ToggleSpawnPoint(m_tiles[0]);
    }

    public GameTile GetSelectedTile(Ray a_ray)
    {
        if (Physics.Raycast(a_ray, out RaycastHit hit, float.MaxValue, 1))
        {
            int x = (int)(hit.point.x + m_size.x * .5f);
            int y = (int)(hit.point.z + m_size.y * .5f);
            if (x >= 0 && x < m_size.x && y >= 0 && y < m_size.y)
                return m_tiles[x + y * m_size.x];
        }
        return null;
    }

    public void ToggleDestination(GameTile a_tile)
    {
        if (a_tile.Content.contentType == GameTileContentType.Destination)
        {
            a_tile.Content = GameTileContentFactory.Instance.GetTileContent(GameTileContentType.Empty);
            if (!SearchPaths())
            {
                a_tile.Content = GameTileContentFactory.Instance.GetTileContent(GameTileContentType.Destination);
                SearchPaths();
            }
        }
        else
        {
            a_tile.Content = GameTileContentFactory.Instance.GetTileContent(GameTileContentType.Destination);
            SearchPaths();
        }
    }

    public void ToggleWall(GameTile a_tile)
    {
        if (a_tile.Content.contentType == GameTileContentType.Wall)
        {
            a_tile.Content = GameTileContentFactory.Instance.GetTileContent(GameTileContentType.Empty);
            SearchPaths();
        }
        else if (a_tile.Content.contentType == GameTileContentType.Empty)
        {
            a_tile.Content = GameTileContentFactory.Instance.GetTileContent(GameTileContentType.Wall);
            if(!SearchPaths())
            {
                a_tile.Content = GameTileContentFactory.Instance.GetTileContent(GameTileContentType.Empty);
                SearchPaths();
            }
        }
    }

    public void ToggleTurret(GameTile a_tile, TurretType a_selectedTurretType)
    {
        if (a_tile.Content.contentType == GameTileContentType.Turret)
        {
            a_tile.Content = GameTileContentFactory.Instance.GetTileContent(GameTileContentType.Empty);
            m_updatingTiles.Remove(a_tile.Content);
            SearchPaths();
        }
        else if (a_tile.Content.contentType == GameTileContentType.Empty)
        {
            a_tile.Content = GameTileContentFactory.Instance.GetTileContent(a_selectedTurretType);
            if (SearchPaths())
            {
                m_updatingTiles.Add(a_tile.Content);
            }
            else
            {
                a_tile.Content = GameTileContentFactory.Instance.GetTileContent(GameTileContentType.Empty);
                SearchPaths();
            }
        }
        else if (a_tile.Content.contentType == GameTileContentType.Wall)
        {
            a_tile.Content = GameTileContentFactory.Instance.GetTileContent(a_selectedTurretType);
            m_updatingTiles.Add(a_tile.Content);
        }
    }

    public void ToggleSpawnPoint(GameTile a_tile)
    {
        if (a_tile.Content.contentType == GameTileContentType.SpawnPoint && m_spawnPoints.Count > 1)
        {
            m_spawnPoints.Remove(a_tile);
            a_tile.Content = GameTileContentFactory.Instance.GetTileContent(GameTileContentType.Empty);
        }
        else if (a_tile.Content.contentType == GameTileContentType.Empty)
        {
            a_tile.Content = GameTileContentFactory.Instance.GetTileContent(GameTileContentType.SpawnPoint);
            m_spawnPoints.Add(a_tile);
        }
    }

    public GameTile GetSpawnPoint(int a_index)
    {
        if (a_index < 0 || a_index >= m_spawnPoints.Count)
        {
            return null;
        }
        return m_spawnPoints[a_index];
    }

    public void GameUpdate()
    {
        foreach (var tile in m_updatingTiles)
        {
            tile.GameUpdate();
        }
    }

    #endregion

    #region Private
    private bool SearchPaths()
    {
        foreach(GameTile tile in m_tiles)
        {
            tile.ClearPath();
            if(tile.Content.contentType == GameTileContentType.Destination)
            {
                tile.SetAsDestination();
                m_searchFrontier.Enqueue(tile);
            }
        }

        if (m_searchFrontier.Count == 0) return false;


        while(m_searchFrontier.Count > 0)
        {
            GameTile tile = m_searchFrontier.Dequeue();
            if (tile == null) continue;

            if (tile.IsAlternative)
            {
                m_searchFrontier.Enqueue(tile.GrowPathNorth());
                m_searchFrontier.Enqueue(tile.GrowPathSouth());
                m_searchFrontier.Enqueue(tile.GrowPathEast());
                m_searchFrontier.Enqueue(tile.GrowPathWest());
            }
            else
            {
                m_searchFrontier.Enqueue(tile.GrowPathWest());
                m_searchFrontier.Enqueue(tile.GrowPathEast());
                m_searchFrontier.Enqueue(tile.GrowPathSouth());
                m_searchFrontier.Enqueue(tile.GrowPathNorth());
            }            
        }

        foreach(GameTile tile in m_tiles)
        {
            if (!tile.HasPath)
            {
                return false;
            }
        }

        if (m_pathShown)
        {
            foreach (var tile in m_tiles)
            {
                tile.ShowPath();
            }
        }

        return true;
    }
    #endregion

    #region Lifecycle
   
    #endregion
    #endregion
}
