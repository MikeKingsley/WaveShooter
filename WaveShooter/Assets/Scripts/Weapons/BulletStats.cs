using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Bullet Stats")]
public class BulletStats : ScriptableObject
{
    public float damage;
    public float force;
    public float speed;
    public float lifetime;

    public ParticleEffect defaultImpactEffect; 
}
