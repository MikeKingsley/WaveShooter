using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

[RequireComponent(typeof(AIPath), typeof(AIDestinationSetter))]
[RequireComponent(typeof(SimpleSmoothModifier))]
public class StateController : MonoBehaviour
{
    [Header("State Machine Setup")]
    public State currentState;
    public State remainState;
    public EnemyStats enemyStats;
    public Transform eyes;

    [Header("Optional")]
    public Transform StartDestination;
    public Canvas healthBar;
    public float healthBarOffset;
    public LayerMask enemyTargetLayer;
    public float despawnTimer = 30f;

    [Header("Editor")]
    public bool DrawGizmos;
    public bool DrawGizmosDetailed;

    [Header("Ragdoll")]
    public bool ragdoll;
    public Rigidbody[] bodyParts;

    [Header("Weapon Defaults")]
    public Transform weaponHand;
    public Transform shieldHand;
    public GameObject weaponToUse;
    public GameObject shieldToUse;
    [Range(0f,100f)] public float shieldProbability;

    [Header("Animator")]
    public string idleAnim = "IsIdle";
    public string walkingAnim = "IsWalking";
    public string runningAnim = "IsRunning";
    public string attackingAnim = "IsAttacking";
    public string combatIdleAnim = "IsCombat";
    public float velocity;

    [HideInInspector] public Transform startPosition;
    [HideInInspector] public Transform EnemySpawnContainer;
    [HideInInspector] public float stateTimeElapsed;

    [HideInInspector] public AIDestinationSetter destination;
    [HideInInspector] public AIPath aipathscript;

    [HideInInspector] public GateController gate;
    [HideInInspector] public GameObject gateObject;

    [HideInInspector] public Animator anim;

    [HideInInspector] public bool flee = false;
    [HideInInspector] public bool attacking = false;
    [HideInInspector] public bool alive = true;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public GameObject acquiredTarget;
    [HideInInspector] public float distanceFromTarget;
    [HideInInspector] public Color randomColor;

    GameObject weaponInUse;
    GameObject shieldInUse;
    Slider bar;
    Collider baseCollider;
    Rigidbody baseRigidbody;
    StateController con;
    Seeker seek;
    SimpleSmoothModifier smooth;

    void Awake()
    {
        InitController();
        randomColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    void InitController()
    {
        destination = GetComponent<AIDestinationSetter>();
        aipathscript = GetComponent<AIPath>();
        anim = GetComponent<Animator>();
        baseCollider = GetComponent<Collider>();
        baseRigidbody = GetComponent<Rigidbody>();
        con = GetComponent<StateController>();
        seek = GetComponent<Seeker>();
        smooth = GetComponent<SimpleSmoothModifier>();

        currentHealth = enemyStats.Health;

        if (aipathscript != null)
        {
            aipathscript.maxSpeed = enemyStats.runSpeed;
            aipathscript.endReachedDistance = enemyStats.attackRange;
        }

        if (startPosition == null)
        {
            SetStartPosition(transform);
        }

        if (healthBar != null)
        {
            Vector3 hbPos = new Vector3(transform.position.x, transform.position.y + healthBarOffset, transform.position.z);
            Canvas hb = Instantiate(healthBar, hbPos, transform.rotation, transform);
            healthBar = hb;
            bar = healthBar.transform.Find("Slider").GetComponent<Slider>();
            bar.maxValue = enemyStats.Health;
            bar.value = currentHealth;
        }

    }

    void Update()
    {
        if (alive) currentState.UpdateState(this);

        velocity = aipathscript.velocity.magnitude;
        if (aipathscript.velocity.magnitude >= enemyStats.walkSpeed)
        {
            SetAnimBool(walkingAnim, true);
            if (aipathscript.velocity.magnitude >= enemyStats.runSpeed)
            {
                SetAnimBool(runningAnim, true);
            }
        } else
        {
            SetAnimBool(walkingAnim, false);
            SetAnimBool(runningAnim, false);
        }
    }

    void OnDrawGizmos()
    {
        if (currentState != null && eyes != null && DrawGizmos)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), 0.1f);
            if (DrawGizmosDetailed)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, enemyStats.lookSphereCastRadius);
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, enemyStats.attackRange);
            }
        }
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
            OnExitState();
        }
    }

    public bool CheckHealth()
    {
        return (currentHealth > 0);
    }

    bool CheckProbability(float value)
    {
        return (Mathf.RoundToInt(Random.Range(value, 100f)) == 100);
    }

    public void DoDamage(float damage)
    {
        currentHealth -= damage;

        if (healthBar != null && bar != null)
        {
            bar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            alive = false;
            ToggleRagdoll(false);
        }
    }

    public void SpawnWeapons()
    {
        if (weaponHand != null && weaponToUse != null)
        {
            weaponInUse = Instantiate(weaponToUse, weaponHand.position, weaponHand.rotation, weaponHand) as GameObject;
        }

        if (shieldHand != null && shieldToUse != null && CheckProbability(shieldProbability))
        {
            shieldInUse = Instantiate(shieldToUse, shieldHand.position, shieldHand.rotation, shieldHand) as GameObject;
        }
    }

    void DropWeapons()
    {
        if (weaponInUse != null)
        {
            Rigidbody weapRb = weaponInUse.GetComponent<Rigidbody>();
            Collider weapCol = weaponInUse.GetComponent<Collider>();
            weaponInUse.transform.parent = null;
            weapRb.useGravity = true;
            weapRb.isKinematic = false;
            weapCol.enabled = true;
            Destroy(weaponInUse, despawnTimer);
        }

        if (shieldInUse != null)
        {
            Rigidbody shieldRb = shieldInUse.GetComponent<Rigidbody>();
            Collider shieldCol = shieldInUse.GetComponent<Collider>();
            shieldInUse.transform.parent = null;
            shieldRb.useGravity = true;
            shieldRb.isKinematic = false;
            shieldCol.enabled = true;
            Destroy(shieldInUse, despawnTimer);
        }
    }

    void ToggleRagdoll(bool set)
    {
        DropWeapons();
        anim.enabled = set;
        baseCollider.enabled = set;
        baseRigidbody.useGravity = set;
        baseRigidbody.isKinematic = !set;

        if (bodyParts != null)
        {
            foreach (Rigidbody part in bodyParts)
            {
                Collider collider = part.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = !set;
                }
                part.useGravity = !set;
                part.isKinematic = set;

            }
        }

        DestoryAI();
    }

    void DestoryAI()
    {
        transform.parent = null;
        currentState = remainState;
        Destroy(gameObject, despawnTimer);
        Destroy(healthBar.gameObject);
        Destroy(smooth, 2f);
        Destroy(aipathscript, 2f);
        Destroy(destination, 2f);
        Destroy(seek, 2f);
        Destroy(con);
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }

    public void ResetTimeElapsed()
    {
        stateTimeElapsed = 0;
    }

    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }

    public bool ChanceToFlee()
    {
        int chanceToFlee = Mathf.RoundToInt(Random.Range(0, enemyStats.bravery));

        return (chanceToFlee == 0);
    }

    public void SetStartPosition(Transform pos)
    {
        startPosition = pos;
    }

    public void SetAnimBool(string parm, bool value)
    {
        if (anim == null)
            return;

        if (parm == idleAnim && value)
        {
            anim.SetBool(walkingAnim, false);
            anim.SetBool(attackingAnim, false);
        }

        anim.SetBool(parm, value);
    }

}