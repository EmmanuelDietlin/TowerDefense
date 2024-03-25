using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.UIElements;

public class Enemy : GameEntity
{
    #region Fields
    #region Serialized
    [SerializeField] private Transform m_model = default;
    [SerializeField] private float m_maxHealth = 100;
    #endregion

    #region Private
    private GameTile m_tileFrom, m_tileTo;
    private Vector3 m_positionFrom, m_positionTo;
    private float m_progress, m_progressFactor;
    private PathDirection m_direction;
    private DirectionChange m_directionChange;
    float m_angleFrom, m_angleTo;
    float m_offset = 0f, m_speed = 0f;
    #endregion
    #endregion

    #region Properties
    public float Scale => m_model.localScale.x;
    public float Health { get; private set; }
    #endregion

    #region Methods
    #region Private
    private void NextState()
    {
        m_tileFrom = m_tileTo;
        m_tileTo = m_tileTo.NextOnPath;
        m_positionFrom = m_positionTo;
        if(m_tileTo == null)
        {
            Outro();
            return;
        }
        m_positionTo = m_tileFrom.ExitPoint;
        m_directionChange = m_direction.GetDirectionChange(m_tileFrom.PathDirection);
        m_direction = m_tileFrom.PathDirection;
        m_angleFrom = m_angleTo;

        switch(m_directionChange)
        {
            case DirectionChange.None: Forward(); break;
            case DirectionChange.TurnRight: TurnRight(); break;
            case DirectionChange.Turneft: TurnLeft(); break;
            case DirectionChange.TurnAround: TurnAround(); break;
        }
    }

    private void Forward()
    {
        transform.rotation = m_direction.GetRotation();
        m_angleTo = m_direction.GetAngle();
        m_model.localPosition = new Vector3(m_offset, 0f);
        m_progressFactor = m_speed;
    }

    private void TurnRight()
    {
        m_angleTo = m_angleFrom + 90f;
        m_model.localPosition = new Vector3(m_offset-.5f, 0f);
        transform.position = m_positionFrom + m_direction.GetHalfVector();
        m_progressFactor = m_speed / (Mathf.PI * .5f * (.5f - m_offset));
    }

    private void TurnLeft()
    {
        m_angleTo = m_angleFrom - 90f;
        m_model.localPosition = new Vector3(m_offset+.5f, 0f);
        transform.position = m_positionFrom + m_direction.GetHalfVector();
        m_progressFactor = m_speed / (Mathf.PI * .5f * (.5f - m_offset));

    }

    private void TurnAround()
    {
        m_angleTo = m_angleFrom + (m_offset < 0f ? 180f : -180f);
        m_model.localPosition = new Vector3(m_offset, 0f);
        transform.position = m_positionFrom;
        m_progressFactor = m_speed / (Mathf.PI * Mathf.Max(Mathf.Abs(m_offset), 0.2f));
    }

    private void Outro()
    {
        m_positionTo = m_tileFrom.transform.position;
        m_directionChange = DirectionChange.None;
        m_angleTo = m_direction.GetAngle();
        m_model.localPosition = new Vector3(m_offset, 0f);
        transform.rotation = m_direction.GetRotation();
        m_progressFactor = 2f * m_speed;
    }
    #endregion

    #region Public
    public void SetSpawnPoint(GameTile a_tile)
    {
        m_tileFrom = a_tile;
        m_tileTo = a_tile.NextOnPath;
        m_progress = 0f;
        m_model.localPosition = new Vector3(m_offset, 0f);

        m_positionFrom = m_tileFrom.transform.position;
        m_positionTo = m_tileFrom.ExitPoint;
        m_direction = m_tileFrom.PathDirection;
        m_directionChange = DirectionChange.None;
        m_angleFrom = m_angleTo = m_direction.GetAngle();
        transform.localRotation = m_direction.GetRotation();

        m_progressFactor = 2f * m_speed;

        //transform.position = a_tile.transform.position;
    }

    public override bool GameUpdate()
    {
        if (Health <= 0)
        {
            EnemyFactory.Instance.Reclaim(this);
            return false;
        }
        //transform.position += Vector3.forward * Time.deltaTime;
        m_progress += Time.deltaTime * m_progressFactor;
        while (m_progress >= 1f)
        {
            if (m_tileTo == null)
            {
                EnemyFactory.Instance.Reclaim(this);
                return false;
            }
            m_progress = (m_progress - 1f) / m_progressFactor;
            NextState();
            m_progress *= m_progressFactor;
        }
        if (m_directionChange != DirectionChange.None)
        {
            float angle = Mathf.LerpUnclamped(m_angleFrom, m_angleTo, m_progress);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);            
            //We don't move the enemy when rotating because the movement is already done for the model
            //As we moved the model away from the center so that the circular movement is calculated by Unity when rotating the actor.
        }
        else
        {
            transform.position = Vector3.LerpUnclamped(m_positionFrom, m_positionTo, m_progress);
        }
        return true;
    }

    public void TakeDamage(float a_damage)
    {
        Health -= a_damage;
    }
    #endregion

    #region Public
    public void Initialize(float a_scale, float a_offset, float a_speed)
    {
        m_model.localScale = new Vector3(a_scale, a_scale, a_scale);
        m_offset = a_offset;
        m_speed = a_speed;
        Health = m_maxHealth * Scale;
    }
    #endregion

    #region Lifecycle

    #endregion
    #endregion
}

