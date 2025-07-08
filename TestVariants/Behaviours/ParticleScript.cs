using UnityEngine;

namespace TestVariants.Behaviours;

public class ParticleScript : MonoBehaviour
{
    public Vector3 velocity;

    public void Start()
    {
        velocity = new Vector3(Random.Range(-8f, 8f), Random.Range(1f, 8f), Random.Range(-8f, 8f));
    }

    public void Update()
    {
        velocity += new Vector3(0f, (0f - Time.deltaTime) * 14f, 0f);
        velocity += -velocity / 2f * Time.deltaTime;
        transform.position += velocity * Time.deltaTime * 2f;
    }
}