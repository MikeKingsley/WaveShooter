using UnityEngine;

public class Projectile_3 : MonoBehaviour
{
    public float maxRayDistance = 1;
    public float randomDirOffset = 0.2f;

    [HideInInspector] public float damage;
    [HideInInspector] public float force;
    [HideInInspector] public float speed;
    [HideInInspector] public float lifetime;
    [HideInInspector] public ParticleEffect defaultImpactEffect;

    Vector3 direction;
    Vector3 position;
    Vector3 shotOrigin;

    private void Start()
    {
        direction = transform.forward;
        position = transform.position ;//+ transform.forward * 0.75f

        Destroy(gameObject, lifetime);
    }

    private void SetNextPoint(Vector3 pos, Vector3 dir)
    {
        //Vector3 startingPosition = pos;

        //direction += Physics.gravity * speed * Time.deltaTime;

        Ray ray = new Ray(pos, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            direction = Vector3.Reflect(dir, hit.normal);

            direction = new Vector3(
                Random.Range(direction.x - randomDirOffset, direction.x + randomDirOffset), 
                Random.Range(direction.y - randomDirOffset, direction.y + randomDirOffset), 
                Random.Range(direction.z - randomDirOffset, direction.z + randomDirOffset));
            position = hit.point;

            ObjectHit(hit, dir);
        }
        else
        {
            position += direction * maxRayDistance;
        }
    }

    private void Update()
    {
        if (transform.position == position)
        {
            SetNextPoint(position, direction);
        }

        transform.position = Vector3.MoveTowards(transform.position, position, speed);
    }

    void ObjectHit(RaycastHit hit, Vector3 dir)
    {
        GameObject obj = hit.transform.gameObject;

        if (hit.rigidbody != null)
        {
            hit.rigidbody.AddForceAtPosition(dir * force, hit.point, ForceMode.Impulse);
        }

        StateController stateController = obj.GetComponent<StateController>();
        if (stateController != null)
        {
            float shotDistance = Vector3.Distance(shotOrigin, hit.point);
            float damageCalc = damage * force;

            stateController.DoDamage(damageCalc);

            HitImpact(stateController.enemyStats.impactEffect.effect, hit.point, hit.normal, stateController.enemyStats.impactEffect.lifetime);

            Destroy(gameObject);
        }
    }

    void HitImpact(GameObject effect, Vector3 pos, Vector3 rot, float lifetime)
    {
        GameObject impact = Instantiate(effect, pos, Quaternion.LookRotation(rot)) as GameObject;
        Destroy(impact, lifetime);
    }
}
