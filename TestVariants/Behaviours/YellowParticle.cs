using UnityEngine;

namespace TestVariants.Behaviours;

public class YellowParticle : MonoBehaviour
{
    public Vector3 velocity;

    public void Start()
    {
        velocity = transform.forward * 15f + Vector3.up * 4.5f;
    }

    public void Update()
    {
        velocity += new Vector3(0f, (0f - Time.deltaTime) * 14f, 0f);
        velocity += -velocity / 2f * Time.deltaTime;
        transform.position += velocity * Time.deltaTime * 2f;
    }
}
