using MTM101BaldAPI.Reflection;
using UnityEngine;

namespace TestVariants.Behaviours.Items;

public class ITM_Laptop : Item
{
    private Vector3 direction;

    public Entity entity;

    private bool tossed = false;

    private float height = 0f;

    private float orgheight = 0f;

    private float vel = 5f;

    private bool falling = false;

    private EnvironmentController ec;

    private AudioManager audMan;

    private SpriteRenderer render;

    private AudioSource propagate;

    public static Entity CreateEntity(GameObject entityObject, float height, bool triggerBool, Collider collider, Collider trigger, ActivityModifier actMod, Transform renderBase)
    {
        bool wasActive = entityObject.activeSelf;
        entityObject.SetActive(false);

        Entity entity = entityObject.AddComponent<Entity>();
        entity.collider = collider;
        entity.trigger = trigger;
        entity.externalActivity = actMod;
        entity.rendererBase = renderBase;
        entityObject.SetActive(wasActive);

        entity.SetHeight(height);
        entity.SetTrigger(triggerBool);

        return entity;
    }

    public static SpriteRenderer CreateSpriteRender(string name, bool billboarded, Transform baseObject)
    {
        GameObject spriteObject = new(name);
        spriteObject.transform.parent = baseObject;
        spriteObject.transform.position = baseObject.position;
        spriteObject.AddComponent<SpriteRenderer>();
        SpriteRenderer component = spriteObject.GetComponent<SpriteRenderer>();
        component.material = TestPlugin.FindResourceOfName<Material>(billboarded ? "SpriteStandard_Billboard" : "SpriteWithFog_Forward_NoBillboard", null);
        return component;
    }

    public void Prop(EnvironmentController ec, Vector3 pos, Vector3 dir)
    {
        SpriteRenderer spriteRenderer = CreateSpriteRender("SpriteBase", true, transform);
        spriteRenderer.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Laptop_Entity");
        render = spriteRenderer;
        render.transform.localScale *= 10f;
        spriteRenderer.gameObject.layer = 9;

        CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
        capsule.radius = 0.1f;
        capsule.height = 1f;
        capsule.center = new Vector3(0f, 100f, 0f);

        SphereCollider sphere = gameObject.AddComponent<SphereCollider>();
        sphere.isTrigger = true;
        sphere.radius = 0.1f;
        sphere.center = new Vector3(0f, 100f, 0f);

        capsule.gameObject.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        this.entity = CreateEntity(gameObject, TestPlugin.Instance.assetMan.Get<Entity>("GumEntity").BaseHeight, true, capsule, sphere, gameObject.AddComponent<ActivityModifier>(), transform);
        height = 0f;
        orgheight = 0f;
        this.ec = ec;
        transform.position = pos;
        transform.rotation = transform.rotation;

        this.entity.Initialize(ec, transform.position);
        this.entity.SetActive(true);
        this.entity.Enable(true);
        this.entity.SetFrozen(false);
        this.entity.OnEntityMoveInitialCollision += new Entity.OnEntityMoveCollisionFunction(OnEntityMoveCollision);

        direction = dir;
        tossed = true;
        falling = true;
        this.entity.gameObject.AddComponent<PropagatedAudioManager>();
        audMan = this.entity.GetComponent<PropagatedAudioManager>();

        GameObject propagator = new("Propagator");
        propagator.transform.parent = transform;
        propagator.transform.position = transform.position;
        propagator.AddComponent<AudioSource>();
        propagate = propagator.GetComponent<AudioSource>();
        propagate.playOnAwake = true;
        propagate.loop = true;
        audMan.audioDevice = propagate;
        audMan.ReflectionSetVariable("overrideSubtitleColor", false);

        vel = 0f;
    }

    public override bool Use(PlayerManager pm)
    {
        Prop(pm.ec, pm.transform.position, pm.transform.forward);
        return true;
    }

    private void OnEntityMoveCollision(RaycastHit hit)
    {
        if (falling & height <= 6f)
        {
            render.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Laptop_Entity_Broken");
            audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("TesterlyLaptop_Break"));
            Bounce(hit);
            vel += 0.25f;
        }
    }

    public void Update()
    {
        if (tossed & falling)
        {
            vel += -Time.deltaTime * 25f * ec.EnvironmentTimeScale;
            height += vel * Time.deltaTime * ec.EnvironmentTimeScale;

            if (height <= orgheight - 2.6f)
            {
                height = orgheight - 2.6f;
                falling = false;
                render.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Laptop_Entity_Broken");
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("TesterlyLaptop_Break"));
                entity.AddForce(new Force(direction, 42f, -42f));
                Invoke(nameof(Die), 30f);
            }

            render.transform.position = new Vector3(render.transform.position.x, height + 5f, render.transform.position.z);
            entity.UpdateInternalMovement(direction * 42f * ec.EnvironmentTimeScale);
            return;
        }

        height = orgheight - 2.6f;
        render.transform.position = new Vector3(render.transform.position.x, height + 5f, render.transform.position.z);
        entity.UpdateInternalMovement(Vector3.zero);
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void Bounce(RaycastHit hit)
    {
        direction = Vector3.Reflect(entity.Velocity.normalized, hit.normal);
    }
}
