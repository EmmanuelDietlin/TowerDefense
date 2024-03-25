using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetPoint : MonoBehaviour
{
    #region Fields

    #region Private
    private static Collider[] m_buffer = new Collider[100];
    #endregion
    #endregion

    #region Properties
    public Enemy Enemy {  get; private set; }
    public Vector3 Position => transform.position;
    public float Margin => Enemy.Scale * .125f;
    public static int BufferedCount { get; private set; }
    public static TargetPoint RandomBuffered =>
        GetBuffered(UnityEngine.Random.Range(0, BufferedCount));
    #endregion

    #region Methods
    #region Static
    public static bool FillBuffer(Vector3 a_position, float a_range, LayerMask a_enemyLayerMask)
    {
        Vector3 top = a_position;
        top.y += 3f;
        BufferedCount = Physics.OverlapCapsuleNonAlloc(
            a_position, top, a_range, m_buffer, a_enemyLayerMask
        );
        return BufferedCount > 0;
    }

    public static TargetPoint GetBuffered(int index)
    {
        var target = m_buffer[index].GetComponent<TargetPoint>();
        return target;
    }

    
    #endregion

    #region Lifecycle
    private void Awake()
    {
        Enemy = transform.root.GetComponent<Enemy>();
    }
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
