using UnityEngine;

public class SpriteRotation : MonoBehaviour
{
    public int offset;

    private Transform cam;

    public SpriteRenderer spriteRenderer;

    public Sprite[] sprites = (Sprite[])(object)new Sprite[0];

    private float angleRange;

    public void Start()
    {
        angleRange = 360 / sprites.Length;
        cam = Camera.main.transform;
    }

    public void Update()
    {
        if (cam == null || !cam)
        {
            cam = Camera.main.transform;
            return;
        }

        float atan2 = Mathf.Atan2(cam.position.z - transform.position.z, cam.position.x - transform.position.x) * 57.29578f;
        atan2 += transform.eulerAngles.y;
        if (atan2 < 0f) atan2 += 360f;
        else if (atan2 >= 360f) atan2 -= 360f;

        int i;
        for (i = Mathf.RoundToInt(atan2 / angleRange); i < 0 || i >= sprites.Length; i += (int)(-1 * sprites.Length * Mathf.Sign(i)))
        {

        }

        if (spriteRenderer != null && sprites != null)
        {
            spriteRenderer.sprite = sprites[(i + offset) % sprites.Length];
        }
    }
}
