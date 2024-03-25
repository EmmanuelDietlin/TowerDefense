using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    #region Fields
    #region Serialized
    [SerializeField] private Vector2Int m_boardSize = new Vector2Int(11,11);
    [SerializeField] private GameBoard m_board;
    [SerializeField, Range(.1f, 10f)] private float m_enemySpawnSpeed = 1f;
    #endregion

    #region Private
    private float m_spawnProgress = 0f;
    private GameEntitiesCollection m_enemyCollection;
    private GameEntitiesCollection m_nonEnemyCollection;
    private TurretType m_selectedTurretType = TurretType.Laser;
    private static GameManager m_instance;
    #endregion
    #endregion

    #region Properties
    private Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);
    public static GameManager Instance => m_instance;
    
    #endregion

    #region Methods
    #region Public
    //Methods invoked by the new input system
    public void PlaceWall(InputAction.CallbackContext context)
    {
        GameTile tile = m_board.GetSelectedTile(TouchRay);
        if (tile != null)
        {
            m_board.ToggleWall(tile);
        }
    }

    public void PlaceTurret(InputAction.CallbackContext context)
    {
        GameTile tile = m_board.GetSelectedTile(TouchRay);
        if (tile != null)
        {
            m_board.ToggleTurret(tile, m_selectedTurretType);
        }
    }

    public void PlaceDestination(InputAction.CallbackContext context)
    {
        GameTile tile = m_board.GetSelectedTile(TouchRay);
        if (tile != null)
        {
            m_board.ToggleDestination(tile);
        }
    }

    public void SwitchToMortarTurret(InputAction.CallbackContext context)
    {
        m_selectedTurretType = TurretType.Mortar;
    }

    public void SwitchToLaserTurret(InputAction.CallbackContext context)
    {
        m_selectedTurretType = TurretType.Laser;
    }

    public void TogglePaths(InputAction.CallbackContext context)
    {
        m_board.ShowPaths = !m_board.ShowPaths;
    }

    public void ToggleGrid(InputAction.CallbackContext context)
    {
        m_board.ShowGrid = !m_board.ShowGrid;
    }

    public Projectile SpawnProjectile(ProjectileType a_type)
    {
        Projectile p = ProjectileFactory.Instance.Get(a_type);
        m_nonEnemyCollection.Add(p);
        return p;
    }

    public void AddNonEnemyEntity(GameEntity a_entity)
    {
        m_nonEnemyCollection.Add(a_entity);
    }

    public void AddEnemyEntity(GameEntity a_entity)
    {
        m_enemyCollection.Add(a_entity);
    }
    #endregion

    #region Private
    private void HandleTouch(int a_keyNbr)
    {
        //GameTile tile = m_board.GetSelectedTile(TouchRay);
        //if (tile != null)
        //{
        //    if(a_keyNbr == 0)
        //    {
        //        if (Input.GetKey(KeyCode.LeftShift))
        //        {
        //            m_board.ToggleTurret(tile, m_selectedTurretType);
        //        }
        //        else
        //        {
        //            m_board.ToggleWall(tile);
        //        }
        //    }
        //    else
        //    {
        //        m_board.ToggleDestination(tile);
        //    }
        //}
    }

    private void SpawnEnemies()
    {
        int count = m_board.SpawnPointsCount;
        for (int i = 0; i < count; i++)
        {
            GameTile tile = m_board.GetSpawnPoint(i);
            Enemy enemy = EnemyFactory.Instance.GetEnemy();
            enemy.transform.SetParent(transform, false);
            enemy.SetSpawnPoint(tile);
            m_enemyCollection.Add(enemy);
        }
    }
    #endregion

    #region Lifecycle
    private void Awake()
    {
        m_board.Initialize(m_boardSize);
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        m_spawnProgress = m_enemySpawnSpeed;
        m_enemyCollection = new GameEntitiesCollection();
        m_nonEnemyCollection = new GameEntitiesCollection();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO
        //Replace with Unity new input system
        //TODO


        //if (Input.GetMouseButtonDown(0))
        //{
        //    HandleTouch(0);
        //}
        //if (Input.GetMouseButtonDown(1))
        //{
        //    HandleTouch(1);
        //}
        //if(Input.GetKeyDown(KeyCode.V))
        //{
        //    m_board.ShowPaths = !m_board.ShowPaths;
        //}
        //if(Input.GetKeyDown(KeyCode.G))
        //{
        //    m_board.ShowGrid = !m_board.ShowGrid;
        //}
        //if(Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    m_selectedTurretType = TurretType.Laser;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    m_selectedTurretType = TurretType.Mortar;
        //}

        m_spawnProgress += Time.deltaTime;
        if (m_spawnProgress >= m_enemySpawnSpeed)
        {
            m_spawnProgress -= m_enemySpawnSpeed;
            SpawnEnemies();
        }

        m_enemyCollection.UpdateEntities();
        //Fixes the case where enemies would be targeted by the turrets for one frame on their appearance in the world,
        //As they are spawned in the world center  before being moved
        Physics.SyncTransforms();
        m_board.GameUpdate();
        m_nonEnemyCollection.UpdateEntities();
    }

    private void OnValidate()
    {
        if (m_boardSize.x < 2) m_boardSize.x = 2;
        if (m_boardSize.y < 2) m_boardSize.y = 2;
    }
    #endregion
    #endregion
}
