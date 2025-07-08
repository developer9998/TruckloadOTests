using UnityEngine;

namespace TestVariants.Behaviours;

public class Dizziness : MonoBehaviour
{
    private bool dizzinessStarted = false;

    private float rotation = 0f;

    public void StartDizziness()
    {
        dizzinessStarted = true;
        Destroy(this, 20f);
        RandomRotation();
    }

    private void RandomRotation()
    {
        rotation = Random.Range(-40f, 40f);
        Invoke(nameof(RandomRotation), Random.Range(0.1f, 5f));
    }

    public void Update()
    {
        if (!dizzinessStarted) return;
        transform.Rotate(0f, rotation * Time.deltaTime, 0f);
    }
}
