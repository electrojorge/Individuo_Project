using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EnemyPatrol_Controller : MonoBehaviour
{
    public static EnemyPatrol_Controller instance;
    public enum EnemyBehavior
    {
        Patrol,
        Chase,
        Attack
    }

    [Header("State")]
    public EnemyBehavior enemyActivity = EnemyBehavior.Patrol;

    [Header("Patrol")]
    [Tooltip("Puntos por los que patrullará el enemigo")]
    [SerializeField] public Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [Tooltip("Distancia mínima para considerar que ha llegado al punto")]
    [SerializeField] private float arrivalThreshold = 0.2f;
    [Tooltip("Segundos que espera el enemigo en cada patrol point antes de seguir")]
    [SerializeField] private float waitAtPoint = 1f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4f;
    [Tooltip("Distancia a la que detecta al jugador y pasa a perseguir")]
    [SerializeField] private float chaseRange = 6f;
    [Tooltip("Si el jugador se aleja más que esto, vuelve a patrullar")]
    [SerializeField] private float lostChaseRange = 8f;

    [Header("Vision")]
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float eyeHeight = 1.0f;
    [SerializeField] private LayerMask obstructionMask = ~0;
    private bool requireLineOfSight = true;
    
    private string playerTag = "Player";

    // internals
    private int currentPatrolIndex;
    private Transform playerTransform;

    // espera en punto de patrulla
    private bool isWaiting = false;
    private float waitTimer = 0f;

    public int enemyID;

    void Start()
    {
        // Buscar jugador por tag
        var playerGO = GameObject.FindGameObjectWithTag(playerTag);
        if (playerGO != null) playerTransform = playerGO.transform;

        // Inicializar estado de patrulla
        currentPatrolIndex = 0;
        isWaiting = false;
        waitTimer = 0f;
        enemyActivity = EnemyBehavior.Patrol;
    }

    void Update()
    {
        switch (enemyActivity)
        {
            case EnemyBehavior.Patrol:
                Patrol();
                // Si ve al jugador pasa a perseguir
                if (IsPlayerInVision(false))
                {
                    enemyActivity = EnemyBehavior.Chase;
                }
                break;

            case EnemyBehavior.Chase:
                Chase();
                // Si pierde al jugador vuelve a patrullar y busca el punto más cercano
                if (!IsPlayerInVision(true))
                {
                    ReturnToNearestPatrolPoint();
                    enemyActivity = EnemyBehavior.Patrol;
                }
                break;

            case EnemyBehavior.Attack:
                // Attack se dispara por colisión física
                break;
        }

        // Tecla de depuración para forzar ataque
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            Attack();
        }
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPatrolIndex];
        float dist = Vector3.Distance(transform.position, target.position);

        // Si llega al punto, espera un tiempo antes de ir al siguiente
        if (dist <= arrivalThreshold)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitAtPoint;
            }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    isWaiting = false;
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                }
            }
        }
        else
        {
            // Moverse hacia el punto de patrulla
            MoveTowards(target.position, patrolSpeed);
        }
    }

    void Chase()
    {
        if (playerTransform == null) return;
        // Moverse hacia el jugador
        MoveTowards(playerTransform.position, chaseSpeed);
    }

    public void Attack()
    {
        SceneManager.LoadScene(2);
    }

    // Mueve y rota suavemente hacia un objetivo en el plano XZ
    private void MoveTowards(Vector3 worldTarget, float speed)
    {
        Vector3 dir = (worldTarget - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude <= 0.0001f) return;

        Vector3 move = dir.normalized * speed * Time.deltaTime;
        if (move.sqrMagnitude > dir.sqrMagnitude) move = dir;

        transform.position += move;

        // Rotar hacia la dirección de movimiento
        Vector3 lookDir = dir;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Comprueba si el jugador está dentro del cono de visión y rango.
    /// withHysteresis usa lostChaseRange para evitar cambios constantes.
    /// </summary>
    private bool IsPlayerInVision(bool withHysteresis = false)
    {
        if (playerTransform == null) return false;

        float maxDist = withHysteresis ? lostChaseRange : chaseRange;
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 toPlayer = (playerTransform.position - eyePos);
        float dist = toPlayer.magnitude;
        if (dist > maxDist) return false;

        // Comprobar ángulo de visión
        Vector3 forwardFlat = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 toPlayerFlat = Vector3.ProjectOnPlane(toPlayer, Vector3.up).normalized;
        float halfAngle = viewAngle * 0.5f;
        float angleTo = Vector3.Angle(forwardFlat, toPlayerFlat);
        if (angleTo > halfAngle) return false;

        // Comprobar línea de visión si está requerida
        if (requireLineOfSight)
        {
            RaycastHit hit;
            if (Physics.Raycast(eyePos, toPlayer.normalized, out hit, dist, obstructionMask))
            {
                if (!hit.collider.CompareTag(playerTag))
                    return false;
            }
        }

        return true;
    }

    private bool PlayerInChaseRange(bool withHysteresis = false)
    {
        return IsPlayerInVision(withHysteresis);
    }

    // Colisión física con el jugador entoces ataca
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            enemyActivity = EnemyBehavior.Attack;
            Game_Manager.instance.savedIDs.Add(enemyID);
            Game_Manager.instance.PlayerPos = collision.gameObject.GetComponent<Transform>().position;
            Attack();
        }
    }

    // Ajusta el índice al patrol point más cercano
    private void ReturnToNearestPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        float minDist = float.MaxValue;
        int nearestIndex = 0;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null) continue;
            float d = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (d < minDist)
            {
                minDist = d;
                nearestIndex = i;
            }
        }

        currentPatrolIndex = nearestIndex;
        isWaiting = false;
        waitTimer = 0f;
    }

    // Gizmos: rangos, puntos y cono de visión
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, lostChaseRange);
        if (patrolPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var p in patrolPoints)
                if (p != null) Gizmos.DrawSphere(p.position, 0.1f);
        }

        // Dibujar cono de visión
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        int segments = 24;
        float halfAngle = viewAngle * 0.5f;
        Vector3 forwardFlat = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Quaternion startRot = Quaternion.AngleAxis(-halfAngle, Vector3.up) * Quaternion.LookRotation(forwardFlat, Vector3.up);
        Vector3 prevPoint = eyePos + (startRot * Vector3.forward) * chaseRange;
        for (int i = 1; i <= segments; i++)
        {
            float step = (float)i / segments;
            float angle = -halfAngle + step * viewAngle;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up) * Quaternion.LookRotation(forwardFlat, Vector3.up);
            Vector3 nextPoint = eyePos + (rot * Vector3.forward) * chaseRange;
            Gizmos.DrawLine(eyePos, nextPoint);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}