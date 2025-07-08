using MTM101BaldAPI.AssetTools;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TestVariants.Behaviours.Items;

public class ITM_TestFolder : Item
{
    private AudioSource mus;

    private bool musicdown = false;

    private bool musicup = false;

    private float dist = 0f;

    private TimeScaleModifier tms;

    private bool talk = false;

    private int tog = 1;

    private float vel;

    private float height;

    private float orgheight = 0f;

    private Vector3 floatVel;

    private float animtimer = 1f;

    private int timr = 0;

    private bool back = false;

    private bool go;

    private EnvironmentController ec;

    private AudioManager audMan;

    private SpriteRenderer render;

    private AudioSource propagate;

    private Entity entity;

    private readonly List<string> descriptionKey =
    [
        // Characters
        "Bluxam",
        "Orlan7",
        "Plit",
        "QuestionShelfman",
        "Yestholomew",
        "Testholder", 
        "Cren", 
        "TheBestOne",
        "AJOPI9", 
        "MeMest",
        "PalbyFan", 
        "DaSketch", 
        "Canny", 
        "Qrid", 
        "Gummin",
        "ThroneTest",
        "Testerly", 
        // Items
        "Bubblerum",
        "CannedTest", 
        "GoodSoup",
        "Grebstermote",
        "GumminChunk", 
        "Headplum", 
        "LuckiestCoin", 
        "Potion", 
        "TestFolder", 
        "Testiporter", 
        "TestyDar", 
        "TestYTP", 
        "TestYTPEVIL"
    ];

    private readonly List<string> descriptions =
    [
        // Characters
        "Bluxam \n Will wander around, and if he sees you running, he'll chase you. If he catches you, \n he'll take you to a random classroom on the floor. \n He will then wait a bit before repeating this cycle.",
        "Orlan 7 \n Will slowly stalk the player, only scurrying away if you get near him and \n look at him. He'll steal ytps from you until you look at him if \n he catches you.",
        "Plit \n Will jump at anyone who passes by him. He will make them run in a certain \n direction for a while until you hit a wall straight on. He'll let \n you go then, but you will be slower for a while.",
        "Question Shelfman \n Does nothing gameplay wise. He'll come up to you and ask you \n a question, forcing you to stop moving and face him though.",
        "Yestholomew \n Will hide in rooms. Will only come out if you go into his room. \n He'll run out of the room screaming, and goes to another class. \n He'll lock the doors of that class for 30 seconds.",
        "Testholder9 \n Does nothing. ALL HE DOES IS WANDERS. this guy SUCKS i hate him \n the only thing he does is open doors",
        "Cren \n Slowly stalks the player, spitting Bubblerum Soda at them once \n he gets close enough. He'll run away and despawn for a while if \n Bubblerum soda hits him though.",
        "The Best One \n He'll pee on you for 20 seconds if he catches you. \n You cannot move during this time.",
        "AJOPI9 \n Will wander at an insanely slow pace until you see him. \n He will then begin screaming, accelerating towards you forever. \n He always knows where you are. Getting caught will blind \n you for 5 minutes. Although, it's too late for you now.",
        "Me Mest \n Wanders around blocking off hallways. He can be pushed \n out of his blocking spot. That's about it.",
        "PalbyFan1 \n If he sees you, he'll show you his Palby. Look at his Palby \n and he'll be happy. Don't and you'll be chased down. \n If he catches you, he punches you, causing you \n to be dizzy. He can also punch NPCs.", 
        "Da Sketch \n Will hide in papers on the desks in classrooms. \n Will pop out and alert Baldi to your location if you're \n in his room for too long.", 
        "Canny \n You really wasted me on an NPC that is basically \n just an advanced BSODA spray? Good choice.", 
        "Qrid \n He'll befriend you if he gets close enough to you. \n If anyone (Including you) causes the friendship to end \n he'll get mad at them, dragging them around for a while.", 
        "The Gummin \n Will wander around and shove a chunk of himself into you if \n he sees you. The chunk is useless. \n The chunk gets lost in the flood, though.", 
        "xXThroneTestXx \n Wanders around, running over anyone whos in his way. \n He'll make some weird noises if he gets a combo or something. \n I heard he can break windows too.", 
        "Testerly \n Holds her laptop and will get mad calling the principal \n if anyone bumps into her and knocks it out of her hands. \n She'll get another laptop afterwards.", 
        // Items
        "Bubblerum Soda \n A throwable item that can push NPCs away if they are hit by it \n or create a puddle on the floor that NPCs or \n the player will slip over. The puddle decays after 3 slips.",
        "Canned Test \n Releases a Test that runs at the nearest NPC and pushes them back \n for a while before despawning.",
        "Good Soup \n Will slowly fill up your stamina for ten seconds after you eat it.",
        "The Grebstermote \n Can grab items from far distances or if you're squished, \n can press buttons from far distances, or grab notebooks. \n (From far distances)",
        "Gummin Chunk \n why did you waste me on this",
        "Headplum \n Can create sticky spots on the ground that slow down NPCs or \n the player. It can bounce off of walls, and can also \n create stickier spots if it hits someone directly.",
        "The Luckiest Coin \n Can be used in the store to set an item on discount. \n Discounts can stack.",
        "Eye Sharpening Potion \n Removes all fog from your vision. You do not understand \n how much you will need this. Also gives you \n a dizzy effect for a while.",
        "Test Folder \n Greetings! I'm the Test Folder. I tell you things. \n You kind of wasted me here. I will die now.",
        "Testiporter \n Spawns a random Test Variant infront of you. The variant will be \n shoved forward and can be used to push back other NPCs.",
        "Testy Dar \n Gives you full stamina and stops time, but your stamina \n slowly depletes no matter what and the time will \n go back to normal after it's gone.",
        "Test YTP \n Gives you 300 YTPs but blinds you for a while.",
        "Angry Test YTP \n Gives you 600 YTPs but blinds you for a long while."
    ];

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

    // Token: 0x06000025 RID: 37 RVA: 0x00003314 File Offset: 0x00001514
    public override bool Use(PlayerManager pm)
    {
        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("TestFolder_Use"));

        SpriteRenderer spriteRenderer = CreateSpriteRender("SpriteBase", true, transform);
        spriteRenderer.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("FolderSpin0000");
        render = spriteRenderer;
        spriteRenderer.transform.localScale *= 8f;
        spriteRenderer.gameObject.layer = 9;

        CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
        capsule.radius = 0.1f;
        capsule.height = 1f;
        capsule.center = new Vector3(0f, 100f, 0f);
        capsule.gameObject.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        SphereCollider sphere = gameObject.AddComponent<SphereCollider>();
        sphere.isTrigger = true;
        sphere.radius = 0.1f;
        sphere.center = new Vector3(0f, 100f, 0f);

        height = spriteRenderer.transform.position.y;
        orgheight = spriteRenderer.transform.position.y;

        this.entity = CreateEntity(gameObject, TestPlugin.Instance.assetMan.Get<Entity>("GumEntity").BaseHeight, true, capsule, sphere, gameObject.AddComponent<ActivityModifier>(), transform);
        ec = pm.ec;
        transform.position = pm.transform.position;
        transform.rotation = Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).camCom.transform.rotation;

        this.entity.Initialize(pm.ec, pm.transform.position);
        this.entity.SetActive(true);
        this.entity.Enable(true);
        this.entity.SetFrozen(false);
        this.entity.OnEntityMoveInitialCollision += new Entity.OnEntityMoveCollisionFunction(OnEntityMoveCollision);

        transform.forward = Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).camCom.transform.forward;
        go = true;

        audMan = this.entity.gameObject.AddComponent<PropagatedAudioManager>();
        GameObject propagator = new("Propagator");
        propagator.transform.parent = transform;
        propagator.transform.position = transform.position;
        propagator.AddComponent<AudioSource>();
        propagate = propagator.GetComponent<AudioSource>();
        propagate.playOnAwake = true;
        propagate.loop = true;
        audMan.audioDevice = propagate;
        audMan.overrideSubtitleColor = false;

        return true;
    }

    private void OnEntityMoveCollision(RaycastHit hit)
    {
        if (go)
        {
            go = false;
            back = true;
            height = orgheight;
            render.transform.position = new Vector3(render.transform.position.x, height + 5f, render.transform.position.z);
            audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("TestFolder_Bounce"));
        }
    }

    public void DoCheck()
    {
        // Characters
        List<string> gameObjectNames =
        [
            "Bluxam(Clone)",
            "Orlan 7(Clone)",
            "Plit(Clone)",
            "Question Shelfman(Clone)",
            "Yestholomew(Clone)",
            "Testholder9(Clone)",
            "Cren(Clone)",
            "The Best One(Clone)",
            "AJOPI9(Clone)",
            "Me Mest(Clone)",
            "PalbyFan1(Clone)",
            "Da Sketch(Clone)",
            "Canny(Clone)",
            "Qrid(Clone)",
            "The Gummin(Clone)", 
            "xXThroneTestXx(Clone)", 
            "Testerly(Clone)", 
            // Items
            "ITM_Bubblerum",
            "ITM_CannedTest",
            "ITM_GoodSoup",
            "ITM_Grebstermote",
            "ITM_GumminChunk",
            "ITM_Headplum", 
            "ITM_LuckiestCoin", 
            "ITM_Potion", 
            "ITM_TestFolder", 
            "ITM_Testiporter", 
            "ITM_TestyDar", 
            "ITM_TestYTP",
            "ITM_TestYTPEVIL"
        ];

        bool hasRecognisedContent = false;

        foreach (NPC npc in ec.Npcs)
        {
            if ((npc.transform.position - transform.position).magnitude <= 5f && gameObjectNames.Contains(npc.name) & !hasRecognisedContent)
            {
                hasRecognisedContent = true;
                SayLine(gameObjectNames.IndexOf(npc.name));
                break;
            }
        }

        if (!hasRecognisedContent)
        {
            foreach (Pickup pickup in ec.items)
            {
                if ((pickup.transform.position - transform.position).magnitude <= 5f && gameObjectNames.Contains(pickup.item.nameKey) & !hasRecognisedContent)
                {
                    hasRecognisedContent = true;
                    SayLine(gameObjectNames.IndexOf(pickup.item.nameKey));
                    break;
                }
            }
        }
    }

    // Token: 0x06000028 RID: 40 RVA: 0x00003A48 File Offset: 0x00001C48
    public void SayLine(int index)
    {
        timr = 0;
        go = false;
        back = false;
        height = orgheight;
        render.transform.position = new Vector3(render.transform.position.x, height + 5f, render.transform.position.z);
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("TestFolder_Bounce"));
        transform.LookAt(ec.Players[0].transform.position);
        ec.Players[0].gameObject.GetComponent<Entity>().AddForce(new Force(transform.forward, 15f, -15f));
        dist = (transform.position - ec.Players[0].transform.position).magnitude + 30f;
        go = false;
        back = false;
        talk = true;

        AudioSource musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.clip = TestPlugin.Instance.assetMan.Get<SoundObject>("TestFolder_Loop").soundClip;
        musicSource.volume = 0.4f;
        musicSource.pitch = 0f;
        musicSource.Play();

        AudioSource speechSource = gameObject.AddComponent<AudioSource>();
        speechSource.clip = AssetLoader.AudioClipFromFile(Path.Combine(AssetLoader.GetModPath(TestPlugin.Instance), "Audio", $"TestFolder_{descriptionKey[index]}.wav"));
        speechSource.rolloffMode = AudioRolloffMode.Linear;
        speechSource.spatialBlend = audMan.audioDevice.spatialBlend;
        audMan.volumeModifier = 0.75f;
        speechSource.gameObject.AddComponent<AudioDistortionFilter>().distortionLevel = 0.5f;
        speechSource.Play();

        Singleton<CoreGameManager>.Instance.GetHud(ec.Players[0].playerNumber).SetTooltip(descriptions[index]);

        tms = new TimeScaleModifier(0f, 0f, 1f);
        ec.AddTimeScale(tms);

        Invoke(nameof(StopYapping), speechSource.clip.length);
        mus = musicSource;
        musicup = true;
        Invoke(nameof(MusicDown), speechSource.clip.length + 4.5f);
        Invoke(nameof(MusicUp), 0.5f);
        Invoke(nameof(Despaw), speechSource.clip.length + 5f);
    }

    private void MusicDown()
    {
        musicdown = true;
    }

    private void MusicUp()
    {
        musicup = false;
        mus.pitch = 1f;
    }

    private void StopYapping()
    {
        talk = false;
        render.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("FolderTalk0000");
    }

    private void Despaw()
    {
        ec.RemoveTimeScale(tms);
        Singleton<CoreGameManager>.Instance.GetHud(ec.Players[0].playerNumber).CloseTooltip();
        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("TestFolder_Despawn"));
        Destroy(gameObject);
    }

    public void Update()
    {
        if (musicdown) mus.pitch = Mathf.Max(mus.pitch - (Time.deltaTime * 2f), 0);

        if (musicup) mus.pitch += Time.deltaTime * 2f;

        if (talk)
        {
            if ((transform.position - ec.Players[0].transform.position).magnitude > dist)
            {
                talk = false;
                Despaw();
            }

            animtimer += -Time.deltaTime * 14f;
            if (animtimer <= 0f)
            {
                animtimer = 1f;
                timr += tog;
                if (timr == 4)
                {
                    timr = 3;
                    tog = -1;
                }
                if (timr == -1)
                {
                    timr = 0;
                    tog = 1;
                }
                render.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("FolderTalk000" + timr.ToString());
            }
        }

        if (go || back)
        {
            animtimer += -Time.deltaTime * 10f;
            if (animtimer <= 0f)
            {
                animtimer = 1f;
                timr = (timr + 1) % 8;
                render.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("FolderSpin000" + timr.ToString());
            }

            if (go)
            {
                vel -= Time.deltaTime * 18f * ec.EnvironmentTimeScale;
                height += vel * Time.deltaTime * ec.EnvironmentTimeScale;
                render.transform.position = new Vector3(render.transform.position.x, height + 5f, render.transform.position.z);
                DoCheck();
                if (height <= orgheight - 4.9f)
                {
                    go = false;
                    back = true;
                    height = orgheight;
                    render.transform.position = new Vector3(render.transform.position.x, height + 5f, render.transform.position.z);
                    audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("TestFolder_Bounce"));
                }
                entity.UpdateInternalMovement(transform.forward * 40f * ec.EnvironmentTimeScale);
                return;
            }

            transform.LookAt(ec.Players[0].transform.position);
            floatVel += transform.forward * 65f * Time.deltaTime * ec.EnvironmentTimeScale;
            entity.UpdateInternalMovement(floatVel * ec.EnvironmentTimeScale);
            if ((transform.position - ec.Players[0].transform.position).magnitude <= 5f & !ec.Players[0].itm.InventoryFull())
            {
                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("TestFolder_Despawn"));
                ec.Players[0].itm.AddItem(TestPlugin.Instance.assetMan.Get<ItemObject>("TestFolder"));
                Destroy(gameObject);
            }
        }
    }
}
