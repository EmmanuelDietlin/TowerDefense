using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarTurret : Turret
{
    #region Fields
    #region Serialized
    [SerializeField, Range(.5f,2f)] private float m_shotsPerSecond = 1f;
    [SerializeField] private Transform m_mortar = default;
    [SerializeField, Range(.5f, 3f)] private float m_blastRadius = 1f;
    [SerializeField, Range(1, 100)] private float m_shellDamage = 10f;
    #endregion

    #region Private
    private float m_launchSpeed = 0f;
    private float m_launchProgress = 1f;
    #endregion
    #endregion

    #region Properties
    public override TurretType Type => TurretType.Mortar;
    #endregion

    #region Methods
    #region Public
    public override void GameUpdate()
    {
        base.GameUpdate();
        if(m_launchProgress >= 1)
        {
            if(AcquireTarget(out TargetPoint target))
            {
                Launch(target);
                m_launchProgress -= 1f;
            }
            else
            {
                m_launchProgress -= Time.deltaTime * m_shotsPerSecond;
            }
        } 
        m_launchProgress += Time.deltaTime * m_shotsPerSecond; ;

    }
    #endregion

    #region Private
    private void Launch(TargetPoint a_targetPoint)
    {
        //We seek to determine the launch angle of the projectile so that it falls on the target at the end of its movement
        //On veut que le projectile se déplace avec une vitesse FIXE
        //En posant les équations de mouvement, on a  (notamment de par les conditions initiales nulles)
        //dx = vx * t
        //dy = vy * t - m*g*t²*0.5
        //avec vx et vy les deux composantes de la vitesse, que l'on peut également exprimer comme v*cos(0) et v*sin(0),
        //avec 0 l'angle de lancement
        //Soit x la position finale souhaitée du projectile, on a t = x / v*cos(0)
        //On remplace alors dans l'équation de dy, en prenant dy = y (condition finale)
        //On isole ensuite 0 en fonction des autres paramètres
        //On obtient alors une équation du second degré de tan(0), ce qui nous donne deux solutions possibles.
        //On utilisera ici la solution donnant l'angle le plus grand, cela permettant d'avoir une courbe en cloche plus visuellement distinctive
        Vector3 launchPoint = m_mortar.position;
        Vector3 targetPoint = a_targetPoint.Position;
        targetPoint.y = 0;
        //Direction du tir
        Vector2 dir;
        dir.x = targetPoint.x - launchPoint.x;
        dir.y = targetPoint.z - launchPoint.z;
        float x = dir.magnitude;
        float y = -launchPoint.y;
        dir.Normalize();

        float g = Mathf.Abs(Physics.gravity.y);
        //s la vitesse du projectile
        float s = m_launchSpeed;
        float s2 = s * s;

        float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
        Debug.Assert(r >= 0f, "Launch velocity insufficient for range!");
        float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
        float sinTheta = cosTheta * tanTheta;

        m_mortar.rotation = Quaternion.LookRotation(new Vector3(dir.x, tanTheta, dir.y));

        Shell shell = (Shell)GameManager.Instance.SpawnProjectile(ProjectileType.Shell);
        shell.transform.SetParent(transform, false);
        Vector3 vel = new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y);
        shell.Initialize(launchPoint, targetPoint, vel, m_shellDamage, m_blastRadius, m_enemyLayer);

        //Vector3 prev = launchPoint, next;
        //for (int i = 1; i <= 10; i++)
        //{
        //    float t = i / 10f;
        //    float dx = s * cosTheta * t;
        //    float dy = s * sinTheta * t - 0.5f * g * t * t;
        //    next = launchPoint + new Vector3(dir.x * dx, dy, dir.y * dx);
        //    Debug.DrawLine(prev, next, Color.blue, 1f);
        //    prev = next;
        //}

        //Debug.DrawLine(launchPoint, targetPoint, Color.yellow, 1f);
        //Debug.DrawLine(
        //    new Vector3(launchPoint.x, 0.01f, launchPoint.z),
        //    new Vector3(
        //        launchPoint.x + dir.x * x, 0.01f, launchPoint.z + dir.y * x
        //    ),
        //    Color.white, 1f
        //);

    }
    #endregion

    #region Lifecycle
    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        //Ajout d'une marge
        float x = m_range + 0.25f;
        float y = -m_mortar.position.y;
        //On trouve la vitesse nécessaire via les conditions aux limites, soit r = 0
        //Si r < 0 alors on a la racine d'un nombre négatif et donc pas de solutions réelles
        //La vitesse minimale est donc trouvée en résolvant l'équation r = 0, i.e
        float a = Mathf.Abs(Physics.gravity.y) * (y + Mathf.Sqrt(x * x + y * y));
        m_launchSpeed = Mathf.Sqrt(Mathf.Abs(Physics.gravity.y) * (y + Mathf.Sqrt(x * x + y * y)));
    }
    #endregion
    #endregion
}
