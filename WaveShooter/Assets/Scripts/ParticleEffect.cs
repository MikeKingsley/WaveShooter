using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Particle Effect Reference")]
public class ParticleEffect : ScriptableObject {
    public GameObject effect;
    public Vector3 offset;
    public float lifetime = 2f;
    public bool loop;
}
