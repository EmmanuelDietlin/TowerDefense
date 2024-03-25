using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Explosion : GameEntity
{
    #region Fields
    #region Serialized
    [SerializeField, Min(0)] private float m_duration = 1f;
    [SerializeField] private AnimationCurve m_m_explosionOpacityCurve;
    [SerializeField] private AnimationCurve m_explosionScaleCurve;
    #endregion

    #region Private
    private float m_elapsedTime = 0f;
    private float m_scale;
    private MeshRenderer m_meshRenderer;
    private static int m_colorPropertyID = Shader.PropertyToID("_Color");
    private static MaterialPropertyBlock m_propertyBlock;
    #endregion
    #endregion

    #region Methods
    #region Public
    public void Initialize(Vector3 a_position, float a_radius, float a_damage, LayerMask a_enemyLayer) 
    {
        TargetPoint.FillBuffer(a_position, a_radius, a_enemyLayer);
        for (int i = 0; i < TargetPoint.BufferedCount; i++)
        {
            TargetPoint.GetBuffered(i).Enemy.TakeDamage(a_damage);
        }
        transform.position = a_position;
        m_scale = 2f * a_radius;
    }

    public override bool GameUpdate()
    {
        m_elapsedTime += Time.deltaTime;
        if (m_elapsedTime > m_duration)
        {
            Destroy(this);
            return false;
        }
        if (m_propertyBlock == null)
        {
            m_propertyBlock = new MaterialPropertyBlock();
        }
        float t = m_elapsedTime / m_duration;
        Color c = Color.clear;
        c.a = m_m_explosionOpacityCurve.Evaluate(t);
        m_propertyBlock.SetColor(m_colorPropertyID, c);
        m_meshRenderer.SetPropertyBlock(m_propertyBlock);
        transform.localScale = Vector3.one * (m_scale * m_explosionScaleCurve.Evaluate(t));
        return true;
    }
    #endregion

    #region Private

    #endregion

    #region Lifecycle
    private void Awake()
    {
        m_meshRenderer = GetComponentInChildren<MeshRenderer>();
        Debug.Assert(m_meshRenderer != null, "Explosion without renderer!");
    }
    #endregion
    #endregion
}
