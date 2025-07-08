using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Behaviours.Items;

public class ITM_BubblerumSoda : Item
{
    private Collider myTrig = null;

    private float spin = 0f;

    private float life = 0f;

    private readonly List<ActivityModifier> activityModifiers = [];

    private bool isSPLAT = false;

    private int degrades = 3;

    private readonly List<MovementModifier> movementModifiers = [];

    private float speedMult = 1f;

    private Entity myPusher;

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
        GameObject gameObject = new(name);
        gameObject.transform.parent = baseObject;
        gameObject.transform.position = baseObject.position;

        SpriteRenderer component = gameObject.AddComponent<SpriteRenderer>();
        if (billboarded)
            component.material = TestPlugin.FindResourceOfName<Material>("SpriteStandard_Billboard", null);
        else
            component.material = TestPlugin.FindResourceOfName<Material>("SpriteWithFog_Forward_NoBillboard", null);

        return component;
    }

    public void Prop(EnvironmentController ec, Vector3 pos, Vector3 dir)
    {
        SpriteRenderer spriteRenderer = CreateSpriteRender("SpriteBase", true, transform);
        spriteRenderer.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("BubblerumSprite");
        spriteRenderer.transform.localScale *= 2f;
        render = spriteRenderer;
        spriteRenderer.gameObject.layer = 9;

        CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
        capsule.radius = 0.1f;
        capsule.height = 1f;
        capsule.center = new Vector3(0f, 100f, 0f);

        SphereCollider sphere = gameObject.AddComponent<SphereCollider>();
        sphere.isTrigger = true;
        myTrig = sphere;
        sphere.enabled = false;
        sphere.radius = 5f;

        capsule.gameObject.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        entity = CreateEntity(gameObject, TestPlugin.Instance.assetMan.Get<Entity>("GumEntity").BaseHeight, true, capsule, sphere, gameObject.AddComponent<ActivityModifier>(), transform);
        height = 0f;
        orgheight = 0f;
        this.ec = ec;
        transform.position = pos;
        transform.rotation = transform.rotation;

        entity.Initialize(ec, transform.position);
        entity.SetActive(true);
        entity.Enable(true);
        entity.SetFrozen(false);

        entity.OnEntityMoveInitialCollision += new Entity.OnEntityMoveCollisionFunction(OnEntityMoveCollision);
        direction = dir;
        tossed = true;
        falling = true;

        audMan = entity.gameObject.AddComponent<PropagatedAudioManager>();
        GameObject propagator = new("Propagator");
        propagator.transform.parent = transform;
        propagator.transform.position = transform.position;
        propagator.AddComponent<AudioSource>();
        propagate = propagator.GetComponent<AudioSource>();
        propagate.playOnAwake = true;
        propagate.loop = true;
        audMan.audioDevice = propagate;
        audMan.overrideSubtitleColor = false;
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Bubblerum_Throw"));

        vel = -5f;
        height += 5f;
        Invoke(nameof(Spin), 0.08f);
        speedMult = 1.6f;
        life = 1f;
    }

    public override bool Use(PlayerManager pm)
    {
        pm.RuleBreak("Drinking", 1f, 0.25f);

        SpriteRenderer spriteRenderer = CreateSpriteRender("SpriteBase", true, transform);
        spriteRenderer.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("BubblerumSprite");
        spriteRenderer.transform.localScale *= 2f;
        render = spriteRenderer;
        spriteRenderer.gameObject.layer = 9;

        CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
        capsule.radius = 0.1f;
        capsule.height = 1f;
        capsule.center = new Vector3(0f, 100f, 0f);

        SphereCollider sphere = gameObject.AddComponent<SphereCollider>();
        sphere.isTrigger = true;
        myTrig = sphere;
        sphere.radius = 5f;
        sphere.enabled = false;

        capsule.gameObject.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        entity = CreateEntity(gameObject, TestPlugin.Instance.assetMan.Get<Entity>("GumEntity").BaseHeight, true, capsule, sphere, gameObject.AddComponent<ActivityModifier>(), transform);
        height = 0f;
        orgheight = 0f;
        ec = pm.ec;
        transform.position = pm.transform.position;
        transform.rotation = Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).camCom.transform.rotation;

        entity.Initialize(pm.ec, pm.transform.position);
        entity.SetActive(true);
        entity.Enable(true);
        entity.SetFrozen(false);
        entity.OnEntityMoveInitialCollision += new Entity.OnEntityMoveCollisionFunction(OnEntityMoveCollision);

        direction = Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).camCom.transform.forward;
        tossed = true;
        falling = true;

        audMan = entity.gameObject.AddComponent<PropagatedAudioManager>();
        GameObject propatagor = new("Propagator");
        propatagor.transform.parent = transform;
        propatagor.transform.position = transform.position;
        propatagor.AddComponent<AudioSource>();
        propagate = propatagor.GetComponent<AudioSource>();
        propagate.playOnAwake = true;
        propagate.loop = true;
        audMan.audioDevice = propagate;
        audMan.overrideSubtitleColor = false;
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Bubblerum_Throw"));

        Invoke(nameof(Spin), 0.08f);

        return true;
    }

    private void OnEntityMoveCollision(RaycastHit hit)
    {
        if (falling & height <= 6f) SPLAT(true);
    }

    private void Spin()
    {
        if (falling)
        {
            spin += 45f;
            render.SetSpriteRotation(spin);
            Invoke(nameof(Spin), 0.08f);
        }
    }

    public void Update()
    {
        List<ActivityModifier> modifiers = [.. ec.Npcs.Select(npc => npc?.Navigator?.Entity?.ExternalActivity).Where(activityMod => activityMod != null && activityMod)];
        modifiers.Add(ec.Players[0].plm.Entity.ExternalActivity);

        life += Time.deltaTime * ec.EnvironmentTimeScale;

        if (tossed & falling)
        {
            vel -= Time.deltaTime * 15f * ec.EnvironmentTimeScale;
            height += vel * Time.deltaTime * ec.EnvironmentTimeScale;

            if (height <= orgheight - 4.9f)
            {
                height = orgheight - 4.9f;
                falling = false;
                SPLAT(true);
            }

            render.transform.position = new Vector3(render.transform.position.x, height + 5f, render.transform.position.z);
            entity.UpdateInternalMovement(direction * 42f * ec.EnvironmentTimeScale * speedMult);

            foreach (ActivityModifier activityModifier in modifiers)
            {
                if (activityModifier.gameObject.GetComponent<NPC>() || (activityModifier.gameObject.GetComponent<PlayerManager>() & (life >= 0.5f)))
                {
                    if (((activityModifier.transform.position - transform.position).magnitude <= 2.5f) & falling)
                    {
                        transform.position = activityModifier.transform.position;
                        myPusher = activityModifier.gameObject.GetComponent<Entity>();
                        SPLAT(false);
                    }
                }
            }
            return;
        }

        if (tossed & isSPLAT)
        {
            foreach (ActivityModifier activityMod in modifiers)
            {
                Vector3 originPosition = new(transform.position.x, 0f, transform.position.z);
                Vector3 modPosition = new(activityMod.transform.position.x, 0f, activityMod.transform.position.z);
                bool inRange = (originPosition - modPosition).magnitude <= 7f;
                bool hasActivityMod = activityModifiers.Contains(activityMod);

                if (inRange != hasActivityMod)
                {
                    if (hasActivityMod)
                    {
                        activityModifiers[activityModifiers.IndexOf(activityMod)].moveMods.Remove(movementModifiers[activityModifiers.IndexOf(activityMod)]);
                        movementModifiers.Remove(movementModifiers[activityModifiers.IndexOf(activityMod)]);
                        activityModifiers.Remove(activityMod);
                        
                        degrades += -1;
                        switch(degrades)
                        {
                            case 2:
                                render.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("BubblerumSplat2");
                                break;
                            case 1:
                                render.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("BubblerumSplat1");
                                break;
                            default:
                                Die();
                                break;
                        }
                    }
                    else
                    {
                        activityModifiers.Add(activityMod);
                        MovementModifier movementModifier = new(activityMod.gameObject.GetComponent<Entity>().Velocity / Time.deltaTime, 0f);
                        movementModifiers.Add(movementModifier);
                        activityMod.moveMods.Add(movementModifier);
                        audMan.PlaySingle(TestPlugin.FindResourceOfName<SoundObject>("Nana_Slip", null));
                        StartCoroutine(RemoveMod(activityMod, movementModifier, 10f));
                    }
                }
            }

            Vector3 position = transform.position;
            position = new Vector3(Mathf.RoundToInt((position.x - 5f) / 10f) * 10 + 5, transform.position.y, Mathf.RoundToInt((position.z - 5f) / 10f) * 10 + 5);
            transform.position = position;
            entity.UpdateInternalMovement(Vector3.zero);
        }

        if (tossed & !isSPLAT)
        {
            entity.UpdateInternalMovement(direction * 42f * ec.EnvironmentTimeScale);
        }
    }

    public IEnumerator RemoveMod(ActivityModifier am, MovementModifier mm, float tim)
    {
        yield return new WaitForSeconds(tim);
        am.moveMods.Clear();
        yield break;
    }

    private void SPLAT(bool big)
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject gameObject = Instantiate(render.gameObject, render.transform.position, render.transform.rotation);
            SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
            component.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("BubblerumParticle");
            component.SetSpriteRotation(Random.Range(0, 360));
            gameObject.AddComponent<ParticleScript>();
            gameObject.transform.SetParent(null);
            Destroy(gameObject, Random.Range(1f, 5f));
        }

        falling = false;
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Bubblerum_Explode"));

        if (big)
        {
            isSPLAT = true;
            gameObject.transform.localScale /= 2f;
            entity.UpdateInternalMovement(Vector3.zero);
            height = orgheight - 4.9f;
            render.transform.position = new Vector3(render.transform.position.x, height + 5f, render.transform.position.z);
            vel = 0f;
            render.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("BubblerumSplat3");
            render.material = TestPlugin.FindResourceOfName<Material>("SpriteWithFog_Forward_NoBillboard", null);
            render.gameObject.transform.LookAt(render.gameObject.transform.position + Vector3.up);
            render.SetSpriteRotation(Random.Range(0, 360));
            return;
        }

        activityModifiers.Add(myPusher.ExternalActivity);
        Invoke(nameof(Die), 2.5f);
        gameObject.transform.localScale *= 4f;
        height = orgheight;
        render.transform.position = new Vector3(render.transform.position.x, height + 5f, render.transform.position.z);
        vel = 0f;
        render.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("BubblerumLiquid");
        render.SetSpriteRotation(Random.Range(0, 360));
        myTrig.enabled = true;
        MovementModifier movementModifier = new(direction * 50f, 0f);
        myPusher.ExternalActivity.moveMods.Add(movementModifier);
        StartCoroutine(RemoveMod(myPusher.ExternalActivity, movementModifier, 2.49f));
        if (myPusher.TryGetComponent(out Cren cren)) cren.Run();
    }

    private void Die()
    {
        foreach (ActivityModifier activityModifier in activityModifiers)
        {
            activityModifier.moveMods.Clear();
        }
        global::UnityEngine.Object.Destroy(gameObject);
    }
}
