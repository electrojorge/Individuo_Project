using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EnemyPatrol_Controller : MonoBehaviour
{
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
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [Tooltip("Distancia mínima para considerar que ha llegado al punto")]
    [SerializeField] private float arrivalThreshold = 0.2f;
    [Tooltip("Segundos que espera el enemigo en cada patrol point antes de seguir")]
    [SerializeField] private float waitAtPoint = 1f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4f;
    [Tooltip("Distancia a la que detecta al jugador y pasa a Chase (usa también para el cono de visión)")]
    [SerializeField] private float chaseRange = 6f;
    [Tooltip("Si el jugador se aleja más que esto, vuelve a patrullar")]
    [SerializeField] private float lostChaseRange = 8f;

    [Header("Vision")]
    [Tooltip("Ángulo total del cono de visión en grados")]
    [SerializeField] private float viewAngle = 90f;
    [Tooltip("Altura del 'ojo' del enemigo desde donde se hacen los raycast de visión")]
    [SerializeField] private float eyeHeight = 1.0f;
    [Tooltip("Si está activado comprueba línea de visión (raycast) contra `obstructionMask`")]
    [SerializeField] private bool requireLineOfSight = true;
    [Tooltip("Capas que bloquean la visión del enemigo")]
    [SerializeField] private LayerMask obstructionMask = ~0; // por defecto todo

    [Header("Attack")]
    [SerializeField] private string playerTag = "Player";
    [Tooltip("Índice de la escena que se cargará en Attack (por defecto 1)")]
    [SerializeField] private int sceneIndexOnAttack = 1;

    // internals
    private int currentPatrolIndex;
    private Transform playerTransform;

    // espera en punto de patrulla
    private bool isWaiting = false;
    private float waitTimer = 0f;

    void Start()
    {
        // cachear jugador por su tag (si existe)
        var playerGO = GameObject.FindGameObjectWithTag(playerTag);
        if (playerGO != null) playerTransform = playerGO.transform;

        // iniciar valores
        currentPatrolIndex = 0;
        isWaiting = false;
        waitTimer = 0f;

        // estado por defecto
        enemyActivity = EnemyBehavior.Patrol;
    }

    void Update()
    {
        switch (enemyActivity)
        {
            case EnemyBehavior.Patrol:
                Patrol();
                // comprobar transición a Chase por proximidad + cono de visión + line of sight
                if (IsPlayerInVision(false))
                {
                    enemyActivity = EnemyBehavior.Chase;
                }
                break;

            case EnemyBehavior.Chase:
                Chase();
                // Si el jugador se pierde (se aleja o sale del cono/LOS) volver a patrullar y forzar el punto de patrol más cercano
                if (!IsPlayerInVision(true))
                {
                    ReturnToNearestPatrolPoint();
                    enemyActivity = EnemyBehavior.Patrol;
                }
                break;

            case EnemyBehavior.Attack:
                // Estado Attack ahora se activa únicamente cuando ocurre la colisión física.
                break;
        }

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

        // Si está dentro del umbral y no se estaba esperando, iniciar espera.
        if (dist <= arrivalThreshold)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitAtPoint;
            }
            else
            {
                // Durante la espera reducir el temporizador; cuando termine avanzar al siguiente punto
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
            // Si no ha llegado aún, moverse hacia el punto
            MoveTowards(target.position, patrolSpeed);
        }
    }

    void Chase()
    {
        if (playerTransform == null) return;
        // mover hacia el jugador
        MoveTowards(playerTransform.position, chaseSpeed);
    }

    public void Attack()
    {
        // Cargar la escena configurada
        SceneManager.LoadScene(sceneIndexOnAttack);
    }

    // utility: mover hacia destino y rotar suavemente
    private void MoveTowards(Vector3 worldTarget, float speed)
    {
        Vector3 dir = (worldTarget - transform.position);
        dir.y = 0f; // opcional: mantener en el plano XZ
        if (dir.sqrMagnitude <= 0.0001f) return;

        Vector3 move = dir.normalized * speed * Time.deltaTime;
        // evitar sobrepasar el objetivo
        if (move.sqrMagnitude > dir.sqrMagnitude) move = dir;

        transform.position += move;

        // rotación hacia movimiento
        Vector3 lookDir = dir;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Determina si el jugador está dentro del cono de visión y distancia (y opcionalmente línea de visión).
    /// withHysteresis = true usa lostChaseRange en lugar de chaseRange para evitar cambios constantes.
    /// </summary>
    private bool IsPlayerInVision(bool withHysteresis = false)
    {
        if (playerTransform == null) return false;

        float maxDist = withHysteresis ? lostChaseRange : chaseRange;
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 toPlayer = (playerTransform.position - eyePos);
        float dist = toPlayer.magnitude;
        if (dist > maxDist) return false;

        // comprobar ángulo
        Vector3 forwardFlat = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 toPlayerFlat = Vector3.ProjectOnPlane(toPlayer, Vector3.up).normalized;
        float halfAngle = viewAngle * 0.5f;
        float angleTo = Vector3.Angle(forwardFlat, toPlayerFlat);
        if (angleTo > halfAngle) return false;

        // línea de visión opcional
        if (requireLineOfSight)
        {
            RaycastHit hit;
            if (Physics.Raycast(eyePos, toPlayer.normalized, out hit, dist, obstructionMask))
            {
                // Si el primer impacto no es el jugador, la visión está obstruida.
                if (!hit.collider.CompareTag(playerTag))
                    return false;
            }
        }

        return true;
    }

    private bool PlayerInChaseRange(bool withHysteresis = false)
    {
        // mantenido por compatibilidad: ahora delega en IsPlayerInVision()
        return IsPlayerInVision(withHysteresis);
    }

    // Detectar colisión física con el Player (usar el collider "pequeño" y no el trigger grande)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            // Se ha chocado físicamente con el player -> ataque
            enemyActivity = EnemyBehavior.Attack;
            Attack();
        }
    }

    /// <summary>
    /// Selecciona el patrol point más cercano y ajusta currentPatrolIndex para que el enemigo
    /// retome el patrullaje desde ese punto y continúe recorriendo el array.
    /// </summary>
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

        // Establecer el índice en el punto más cercano. El patrullaje continuará desde ese índice.
        currentPatrolIndex = nearestIndex;
        // Reiniciar estado de espera para que el enemigo vaya hacia ese punto y luego espere al llegar.
        isWaiting = false;
        waitTimer = 0f;
    }

    // Opcional: dibujar radios y el cono de visión en el editor para depuración
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