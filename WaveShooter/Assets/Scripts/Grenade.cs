using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(VRTK_InteractableObject))]
public class Grenade : MonoBehaviour {

    public float Timer = 3f;
    public float Damage = 100f;
    public float explosionRadius = 2f;
    public ParticleEffect explosion;
    public bool DrawGizmos;

    VRTK_InteractableObject interact;

    public bool hasExploded = false;
    public bool hasBeenGrabbed = false;
    float TimeElapsed;
    Vector3 startPos;
    Vector3 finalPos;
    Scoreboard score;

    private void Awake()
    {
        interact = GetComponent<VRTK_InteractableObject>();
        score = FindObjectOfType<Scoreboard>();
    }

    bool CheckIfThrown()
    {
        if (!hasBeenGrabbed)
        {
            hasBeenGrabbed = interact.IsGrabbed();
        }
        return (hasBeenGrabbed && !interact.IsGrabbed());
    }

    private void Update()
    {
        if (CheckIfThrown())
        {
            startPos = transform.position;

            if (StartGrenadeTimer())
            {
                Explode();
            }
        }
    }

    bool StartGrenadeTimer()
    {
        TimeElapsed += Time.deltaTime;
        return (TimeElapsed >= Timer);
    }

    public void Explode()
    {
        if (startPos == Vector3.zero)
        {
            startPos = transform.position;
        }
        finalPos = transform.position;

        if (explosion != null)
        {
            GameObject e = Instantiate(explosion.effect, transform.position, Quaternion.identity);
            Destroy(e, 2f);
        }

        if (explosionRadius > 0f && !hasExploded)
        {
            hasExploded = true;
            DamageTargets();
        }

        Destroy(gameObject);
    }

    void DamageTargets()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(Damage * explosionRadius, transform.position, explosionRadius);
            }

            GameObject parent = collider.gameObject;

            if (parent != null)
            {
                Grenade g = parent.GetComponent<Grenade>();
                StateController controller = parent.GetComponent<StateController>();
                if (controller != null)
                {
                    float thrownDistance = Vector3.Distance(startPos, finalPos);
                    controller.DoDamage(Damage);

                    if (score != null)
                    {
                        score.UpdateScore(Mathf.RoundToInt(Damage * thrownDistance));
                    }
                }
                if (g != null && !g.hasExploded)
                {
                    g.Explode();
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (DrawGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }

}
