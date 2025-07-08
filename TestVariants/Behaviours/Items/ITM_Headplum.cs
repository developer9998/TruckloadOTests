using System.Collections.Generic;
using UnityEngine;

namespace TestVariants.Behaviours.Items;

public class ITM_Headplum : Item
{
    private float life = 0f;

    private readonly List<ActivityModifier> activityModifiers = [];

    private readonly List<MovementModifier> movementModifiers = [];

    private float sizifier = 1f;

    private float speedymee = 1f;

    private float speedyyou = 1f;

    private Vector3 direction;

    private Entity entity;

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

    public void Prop(EnvironmentController ec, Vector3 pos)
    {
        SpriteRenderer spriteRenderer = CreateSpriteRender("SpriteBase", true, transform);
        spriteRenderer.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("HeadplumObject");
        render = spriteRenderer;
        spriteRenderer.gameObject.layer = 9;

        CapsuleCollider capsule = base.gameObject.AddComponent<CapsuleCollider>();
        capsule.radius = 0.1f;
        capsule.height = 1f;
        capsule.center = new Vector3(0f, 100f, 0f);

        SphereCollider sphere = base.gameObject.AddComponent<SphereCollider>();
        sphere.isTrigger = true;
        sphere.radius = 0.1f;
        sphere.center = new Vector3(0f, 100f, 0f);

        capsule.gameObject.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        this.entity = CreateEntity(base.gameObject, TestPlugin.Instance.assetMan.Get<Entity>("GumEntity").BaseHeight, true, capsule, sphere, base.gameObject.AddComponent<ActivityModifier>(), transform);
        height = 0f;
        orgheight = 0f;
        this.ec = ec;
        transform.position = pos;

        this.entity.Initialize(ec, transform.position);
        this.entity.SetActive(true);
        this.entity.Enable(true);
        this.entity.SetFrozen(false);
        this.entity.OnEntityMoveInitialCollision += new Entity.OnEntityMoveCollisionFunction(OnEntityMoveCollision);

        direction = Vector3.zero;
        tossed = true;
        falling = true;

        audMan = this.entity.gameObject.AddComponent<PropagatedAudioManager>();
        GameObject gameObject = new("Propagator");
        gameObject.transform.parent = transform;
        gameObject.transform.position = transform.position;
        gameObject.AddComponent<AudioSource>();
        propagate = gameObject.GetComponent<AudioSource>();
        propagate.playOnAwake = true;
        propagate.loop = true;
        audMan.audioDevice = propagate;
        audMan.overrideSubtitleColor = false;
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Headplum_Throw"));

        vel = -5f;
        height += 5f;
    }

    // Token: 0x06000098 RID: 152 RVA: 0x0000A364 File Offset: 0x00008564
    public override bool Use(PlayerManager pm)
    {
        SpriteRenderer spriteRenderer = CreateSpriteRender("SpriteBase", true, transform);
        spriteRenderer.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("HeadplumObject");
        render = spriteRenderer;
        spriteRenderer.gameObject.layer = 9;

        CapsuleCollider capsule = base.gameObject.AddComponent<CapsuleCollider>();
        capsule.radius = 0.1f;
        capsule.height = 1f;
        capsule.center = new Vector3(0f, 100f, 0f);

        SphereCollider sphere = base.gameObject.AddComponent<SphereCollider>();
        sphere.isTrigger = true;
        sphere.radius = 0.1f;
        sphere.center = new Vector3(0f, 100f, 0f);

        capsule.gameObject.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        this.entity = CreateEntity(base.gameObject, TestPlugin.Instance.assetMan.Get<Entity>("GumEntity").BaseHeight, true, capsule, sphere, base.gameObject.AddComponent<ActivityModifier>(), transform);
        height = 0f;
        orgheight = 0f;
        ec = pm.ec;
        transform.position = pm.transform.position;
        transform.rotation = Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).camCom.transform.rotation;

        this.entity.Initialize(pm.ec, pm.transform.position);
        this.entity.SetActive(true);
        this.entity.Enable(true);
        this.entity.SetFrozen(false);
        this.entity.OnEntityMoveInitialCollision += new Entity.OnEntityMoveCollisionFunction(OnEntityMoveCollision);

        direction = Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).camCom.transform.forward;
        tossed = true;
        falling = true;

        audMan = this.entity.gameObject.AddComponent<PropagatedAudioManager>();
        GameObject gameObject = new("Propagator");
        gameObject.transform.parent = transform;
        gameObject.transform.position = transform.position;
        gameObject.AddComponent<AudioSource>();
        propagate = gameObject.GetComponent<AudioSource>();
        propagate.playOnAwake = true;
        propagate.loop = true;
        audMan.audioDevice = propagate;
        audMan.overrideSubtitleColor = false;
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Headplum_Throw"));

        return true;
    }

    private void OnEntityMoveCollision(RaycastHit hit)
    {
        if (falling & height <= 6f)
        {
            Bounce(hit);
            vel += 0.25f;
        }
    }

    public void Update()
    {
        List<ActivityModifier> list = [];
        foreach (NPC npc in ec.Npcs)
        {
            if (npc?.Navigator?.Entity?.ExternalActivity is ActivityModifier activityMod)
                list.Add(activityMod);
        }
        list.Add(ec.Players[0].plm.Entity.ExternalActivity);

        life += Time.deltaTime * ec.EnvironmentTimeScale;

        if (tossed & falling)
        {
            vel += -Time.deltaTime * 15f * ec.EnvironmentTimeScale;
            height += vel * Time.deltaTime * ec.EnvironmentTimeScale;

            if (height <= orgheight - 4.9f)
            {
                height = orgheight - 4.9f;
                falling = false;
                SPLAT(true);
            }

            render.transform.position = new Vector3(render.transform.position.x, height + 5f, render.transform.position.z);
            entity.UpdateInternalMovement(direction * 32f * ec.EnvironmentTimeScale);
            foreach (ActivityModifier activityMod in list)
            {
                if ((activityMod.gameObject.GetComponent<NPC>() || activityMod.gameObject.GetComponent<PlayerManager>()) & life >= 0.5f && (activityMod.transform.position - transform.position).magnitude <= 2.25f)
                {
                    transform.position = activityMod.transform.position;
                    SPLAT(false);
                }
            }
            return;
        }

        if (tossed)
        {
            Vector3 movement = Vector3.zero;

            foreach (ActivityModifier activityMod in list)
            {
                Vector3 headplumPosition = new(transform.position.x, 0f, transform.position.z);
                Vector3 activityPosition = new(activityMod.transform.position.x, 0f, activityMod.transform.position.z);
                bool isActivityInBounds = (headplumPosition - activityPosition).magnitude <= 5f * sizifier;
                bool hasActivityModifier = activityModifiers.Contains(activityMod);

                if (isActivityInBounds != hasActivityModifier)
                {
                    audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Headplum_Stick"));
                    if (hasActivityModifier)
                    {
                        activityModifiers[activityModifiers.IndexOf(activityMod)].moveMods.Remove(movementModifiers[activityModifiers.IndexOf(activityMod)]);
                        movementModifiers.Remove(movementModifiers[activityModifiers.IndexOf(activityMod)]);
                        activityModifiers.Remove(activityMod);
                    }
                    else
                    {
                        activityModifiers.Add(activityMod);
                        MovementModifier movementModifier = new(Vector3.zero, 0.25f * speedyyou);
                        movementModifiers.Add(movementModifier);
                        activityMod.moveMods.Add(movementModifier);
                    }
                }

                if (hasActivityModifier)
                {
                    movement += activityMod.gameObject.GetComponent<Entity>().Velocity / Time.deltaTime * 0.25f * speedymee;
                }
            }

            entity.UpdateInternalMovement(movement);
        }
    }

    private void Die()
    {
        foreach (ActivityModifier activityMod in FindObjectsOfType<ActivityModifier>())
        {
            if (activityModifiers.Contains(activityMod))
            {
                activityModifiers[activityModifiers.IndexOf(activityMod)].moveMods.Remove(movementModifiers[activityModifiers.IndexOf(activityMod)]);
                movementModifiers.Remove(movementModifiers[activityModifiers.IndexOf(activityMod)]);
                activityModifiers.Remove(activityMod);
            }
        }
        Destroy(gameObject);
    }

    private void SPLAT(bool big)
    {
        entity.UpdateInternalMovement(Vector3.zero);
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Headplum_Splat"));
        height = orgheight - 4.9f;
        render.transform.position = new Vector3(render.transform.position.x, height + 5f, render.transform.position.z);
        vel = 0f;
        render.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("HeadplumSplatBig");
    
        if (big)
        {
            speedymee = 2.5f;
            Invoke(nameof(Die), 32f);
        }
        else
        {
            render.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("HeadplumSplatSmall");
            sizifier = 0.5f;
            speedymee = 3.5f;
            speedyyou = 0.2f;
            Invoke(nameof(Die), 16f);
        }

        render.material = TestPlugin.FindResourceOfName<Material>("SpriteWithFog_Forward_NoBillboard", null);
        render.gameObject.transform.LookAt(render.gameObject.transform.position + Vector3.up);
        render.SetSpriteRotation(Random.Range(0, 360));
    }

    private void Bounce(RaycastHit hit)
    {
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Headplum_Bounce"));
        direction = Vector3.Reflect(entity.Velocity.normalized, hit.normal);
    }
}
