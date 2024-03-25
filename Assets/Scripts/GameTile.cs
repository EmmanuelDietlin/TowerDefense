using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    #region Fields
    #region Serialized
    [SerializeField] private Transform m_arrow = default;
    #endregion

    #region Private
    private GameTile m_north, m_south, m_east, m_west, m_nextOnPath;
    private int m_distance;
    private GameTileContent m_content;
    #endregion
    #endregion

    #region Static
    private static Quaternion
        northRot = Quaternion.Euler(90, 0, 0),
        southRot = Quaternion.Euler(90, 180, 0),
        eastRot = Quaternion.Euler(90, 90, 0),
        westRot = Quaternion.Euler(90, 270, 0);
    #endregion

    #region Properties
    public bool HasPath => m_distance < int.MaxValue;
    public bool IsAlternative { get; set; }
    public Vector3 ExitPoint { get; private set; }
    public PathDirection PathDirection { get; private set; }
    public GameTile NextOnPath => m_nextOnPath;
    public GameTile GrowPathNorth() => GrowPath(m_north, PathDirection.South);
    public GameTile GrowPathSouth() => GrowPath(m_south, PathDirection.North);
    public GameTile GrowPathEast() => GrowPath(m_east, PathDirection.West);
    public GameTile GrowPathWest() => GrowPath(m_west, PathDirection.East);

    public GameTileContent Content
    {
        get => m_content;
        set
        {
            Debug.Assert(value != null, "Null assigned to content!");
            if (m_content != null)
            {
                m_content.Recycle();
            }
            m_content = value;
            m_content.transform.position = transform.position;
            m_content.transform.SetParent(transform, true);
        }
    }
    #endregion


    #region Methods
    #region Private
    private GameTile GrowPath(GameTile a_neighbor, PathDirection a_pathDirection)
    {
        if (!HasPath || a_neighbor == null || a_neighbor.HasPath)
        {
            return null;
        }
        a_neighbor.m_distance = m_distance + 1;
        a_neighbor.m_nextOnPath = this;
        a_neighbor.ExitPoint = (a_neighbor.transform.position + transform.position) * .5f;
        a_neighbor.PathDirection = a_pathDirection;
        return a_neighbor.Content.BlocksPath ?  null : a_neighbor;
    }
    #endregion

    #region Public
    public static void MakeEastWestNeighbors(GameTile a_east, GameTile a_west)
    {
        Debug.Assert(a_west.m_east == null && a_east.m_west == null, "Redefined neighbors");
        a_east.m_west = a_west;
        a_west.m_east = a_east;
    }

    public static void MakeNorthSouthNeighbors(GameTile a_north, GameTile a_south)
    {
        Debug.Assert(a_north.m_south == null && a_south.m_north== null, "Redefined neighbors");
        a_north.m_south = a_south;
        a_south.m_north = a_north;
    }

    public void ShowPath()
    {
        if (m_distance == 0) m_arrow.gameObject.SetActive(false);
        else
        {
            m_arrow.gameObject.SetActive(true);
            m_arrow.localRotation =
                m_nextOnPath == m_north ? northRot :
                m_nextOnPath == m_south ? southRot :
                m_nextOnPath == m_east ? eastRot :
                westRot;

        }
    }

    public void ClearPath()
    {
        m_nextOnPath = null;
        m_distance = int.MaxValue;
    }

    public void SetAsDestination()
    {
        m_nextOnPath = null;
        m_distance = 0;
        ExitPoint = transform.position;
    }

    public void TogglePath(bool a_state)
    {
        if (a_state == false)
            m_arrow.gameObject.SetActive(a_state);
        else
        {
            ShowPath();
        }
    }
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

public enum GameTileContentType
{
    Empty,
    Destination,
    Wall,
    SpawnPoint,
    Turret
}