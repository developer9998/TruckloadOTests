using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using PicnicPanic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestVariants.Behaviours.Characters;
using TestVariants.Behaviours.Items;
using TestVariants.Models;
using UnityEngine;
using CollectionExtensions = HarmonyLib.CollectionExtensions;
using Object = UnityEngine.Object;

namespace TestVariants;

[BepInPlugin(modGUID, modName, modVersion)]
public class TestPlugin : BaseUnityPlugin
{
    private const string modGUID = "txv.bbplus.testvariants";

    private const string modName = "BB+ Test Variants";

    private const string modVersion = "1.0.1";

    internal new ManualLogSource Logger;

    private Harmony harmony;

    public static TestPlugin Instance;

    public AssetManager assetMan = new();

    private GenericHallBuilder debugCannedTest, debugBubblerum, debugTestyDar, debugVase;

    public void Awake()
    {
        if (Instance == null || !Instance)
            Instance = this;

        harmony = new(modGUID);
        harmony.PatchAllConditionals();

        LoadingEvents.RegisterOnAssetsLoaded(Info, OnLoad(), LoadingEventOrder.Start);
        LoadingEvents.RegisterOnAssetsLoaded(Info, SetupPost(), LoadingEventOrder.Post);
        GeneratorManagement.Register(this, GenerationModType.Override, AddNPCsFrameDelay);

        Logger = base.Logger;
        Logger.LogInfo("The Tests Are Plentiful");

        harmony.PatchAllConditionals();

        AssetLoader.LocalizationFromMod(this);
    }

    private void AddNPCsFrameDelay(string s, int i, SceneObject c)
    {
        StartCoroutine(Adder(s, i, c));
    }

    private IEnumerator Adder(string s, int i, SceneObject c)
    {
        yield return new WaitForSeconds(Time.deltaTime);
        AddNPCs(s, i, c);
        yield break;
    }

    private IEnumerator SetupPost()
    {
        yield return 2;
        yield return "Adding shop items...";

        // Construct item list
        List<WeightedItemObject> weightedItemObjects =
        [
            new WeightedItemObject
            {
                selection = assetMan.Get<ItemObject>("Testiporter"),
                weight = 100
            },
            new WeightedItemObject
            {
                selection = assetMan.Get<ItemObject>("Grebstermote"),
                weight = 120
            },
            new WeightedItemObject
            {
                selection = assetMan.Get<ItemObject>("Headplum"),
                weight = 130
            },
            new WeightedItemObject
            {
                selection = assetMan.Get<ItemObject>("Bubblerum"),
                weight = 130
            },
            new WeightedItemObject
            {
                selection = assetMan.Get<ItemObject>("CannedTest"),
                weight = 140
            },
            new WeightedItemObject
            {
                selection = assetMan.Get<ItemObject>("GoodSoup"),
                weight = 130
            },
            new WeightedItemObject
            {
                selection = assetMan.Get<ItemObject>("TestyDar"),
                weight = 130
            },
            new WeightedItemObject
            {
                selection = assetMan.Get<ItemObject>("Potion"),
                weight = 140
            }
        ];

        // Configure scene objects
        string[] sceneObjectNames = ["MainLevel_1", "MainLevel_2", "MainLevel_3", "MainLevel_4", "MainLevel_5"];
        Array.ForEach(sceneObjectNames, sceneObjectName =>
        {
            SceneObject sceneObject = FindResourceOfName<SceneObject>(sceneObjectName, null);
            sceneObject.shopItems = CollectionExtensions.AddRangeToArray(sceneObject.shopItems, [.. weightedItemObjects]);
            sceneObject.MarkAsNeverUnload();
        });

        // Configure Pick-Quick-Nic
        Sprite bubblerumSpriteCamping = CreateSprite("Bubblerum_Soda_Camping", Path.Combine(AssetLoader.GetModPath(this), "Textures"), 0.5f, 50f);
        PicnicGroupGroup[] scoringGroup = FindResourceOfName<Minigame_Picnic>("Minigame_PicnicPanic", null).scoringGroup;
        WeightedPicnicGroupObject[] groupObjects = [.. scoringGroup.SelectMany(scoringGroup => scoringGroup.groupObject)];
        Array.ForEach(groupObjects, groupObject => groupObject.selection.sprite = [.. CollectionExtensions.AddItem(groupObject.selection.sprite, bubblerumSpriteCamping)]);
        Minigame_Picnic pickQuickNic = FindResourceOfName<Minigame_Picnic>("Minigame_PicnicPanic", null);
        pickQuickNic.scoringGroup = scoringGroup;
        pickQuickNic.MarkAsNeverUnload();

        yield break;
    }

    private IEnumerator OnLoad()
    {
        yield return 1;
        yield return "Creating Test Variants...";

        string texturePath = Path.Combine(AssetLoader.GetModPath(this), "Textures");
        string audioPath = Path.Combine(AssetLoader.GetModPath(this), "Audio");

        #region Items

        #region Testiporter
        Logger.LogInfo("Loading Testiporter");

        Sprite testporterSprite = CreateSprite("Testiporter_Large", texturePath, 0.5f, 50f);
        Sprite testporterSpriteSmall = CreateSprite("Testiporter_Small", texturePath, 0.5f, 25f);

        SoundMetadata testiporterSoundData = new(SoundType.Effect, Color.white);
        SoundObject testiporterSound = CreateSound("Testiport", audioPath, testiporterSoundData, "ITM_Testiporter_Use");
        testiporterSound.subtitle = true;

        ItemObject testiporterObject = new ItemBuilder(Info).SetNameAndDescription("ITM_Testiporter", "ITM_Testiporter_Desc").SetSprites(testporterSpriteSmall, testporterSprite).SetEnum("Testiporter")
            .SetShopPrice(400)
            .SetGeneratorCost(60)
            .SetItemComponent<ITM_Testimote>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();
        assetMan.Add("Testiport", testiporterSound);
        assetMan.Add("Testiporter", testiporterObject);
        #endregion

        #region Grebstermote
        Logger.LogInfo("Loading Grebstermote");

        Sprite grebstermoteSprite = CreateSprite("Grebstermote_Icon_Large", texturePath, 0.5f, 50f);
        Sprite grebstermoteSpriteSmall = CreateSprite("Grebstermote_Icon_Small", texturePath, 0.5f, 25f);
        Sprite grebstermoteSpriteZap = CreateSprite("Grebstermote_Zap", texturePath, 0.5f, 50f);

        SoundObject grebstermoteSound = CreateSound("Grebstermote_Use", audioPath, testiporterSoundData, "ITM_Grebstermote_Use");
        grebstermoteSound.subtitle = true;

        ItemObject grebstermoteObject = new ItemBuilder(Info).SetNameAndDescription("ITM_Grebstermote", "ITM_Grebstermote_Desc").SetSprites(grebstermoteSpriteSmall, grebstermoteSprite).SetEnum("Grebstermote")
            .SetShopPrice(150)
            .SetGeneratorCost(50)
            .SetItemComponent<ITM_GrebsterMote>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();

        assetMan.Add("Grebstermote_Use", grebstermoteSound);
        assetMan.Add("Grebstermote", grebstermoteObject);
        assetMan.Add("ZAP", grebstermoteSpriteZap);
        #endregion

        #region Headplum
        Logger.LogInfo("Loading Headplum");

        Sprite headplumSprite = CreateSprite("Headplum_Icon_large", texturePath, 0.5f, 50f);
        Sprite headplumSpriteSmall = CreateSprite("Headplum_Icon_small", texturePath, 0.5f, 25f);
        Sprite headplumSpriteObject = CreateSprite("Headplum_Entity_Thrown", texturePath, 0.5f, 47f);
        Sprite headplumSplatBig = CreateSprite("Headplum_Splat", texturePath, 0.5f, 25f);
        Sprite headplumSplatSmall = CreateSprite("Headplum_Splat_small", texturePath, 0.5f, 25f);
        Sprite headplumTree = CreateSprite("The_headplum_tree", texturePath, 0.5f, 25f);

        SoundObject headplumThrowSound = CreateSound("Headplum_Throw", audioPath, testiporterSoundData, "ITM_Headplum_Throw");
        headplumThrowSound.subtitle = true;
        SoundObject headplumBounceSound = CreateSound("Headplum_Bounce", audioPath, testiporterSoundData, "ITM_Headplum_Bounce");
        headplumBounceSound.subtitle = true;
        SoundObject headplumSplatSound = CreateSound("Headplum_Splat", audioPath, testiporterSoundData, "ITM_Headplum_Splat");
        headplumSplatSound.subtitle = true;
        SoundObject headplumStickSound = CreateSound("Headplum_Stick", audioPath, testiporterSoundData, "ITM_Headplum_Stick");
        headplumStickSound.subtitle = true;

        Entity gumt = FindResourceOfName<Entity>("Gum", null);
        assetMan.Add("GumEntity", gumt);

        ItemObject headplumObject = new ItemBuilder(Info).SetNameAndDescription("ITM_Headplum", "ITM_Headplum_Desc").SetSprites(headplumSpriteSmall, headplumSprite).SetEnum("Headplum")
            .SetShopPrice(450)
            .SetGeneratorCost(80)
            .SetItemComponent<ITM_Headplum>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();

        assetMan.Add("Headplum", headplumObject);
        assetMan.Add("HeadplumObject", headplumSpriteObject);
        assetMan.Add("HeadplumSplatBig", headplumSplatBig);
        assetMan.Add("HeadplumSplatSmall", headplumSplatSmall);
        assetMan.Add("HeadplumTree", headplumTree);
        assetMan.Add("Headplum_Throw", headplumThrowSound);
        assetMan.Add("Headplum_Bounce", headplumBounceSound);
        assetMan.Add("Headplum_Splat", headplumSplatSound);
        assetMan.Add("Headplum_Stick", headplumStickSound);
        #endregion

        #region Bubblerum Soda
        Logger.LogInfo("Loading Bubblerum Soda");

        Sprite bubblerumSprite = CreateSprite("Bubblerum_Soda_Icon_Large", texturePath, 0.5f, 50f);
        Sprite bubblerumSpriteEvil = CreateSprite("ReallyEvilBubblerum", texturePath, 0.5f, 5f);
        Sprite bubblerumSpriteSmall = CreateSprite("Bubblerum_Soda_Icon_Small", texturePath, 0.5f, 25f);
        Sprite bubblerumSplatA = CreateSprite("Bubblerum_Soda_Spill_3", texturePath, 0.5f, 12f);
        Sprite bubblerumSplatB = CreateSprite("Bubblerum_Soda_Spill_2", texturePath, 0.5f, 12f);
        Sprite bubblerumSplatC = CreateSprite("Bubblerum_Soda_Spill_1", texturePath, 0.5f, 12f);
        Sprite bubblerumLiquid = CreateSprite("Bubblerum_Soda_Liquid", texturePath, 0.5f, 75f);
        Sprite bubblerumParticle = CreateSprite("Bubblerum_Soda_Liquid_particle", texturePath, 0.5f, 24f);

        SoundObject bubblerumThrowSound = CreateSound("Bubblerum_Soda_Throw", audioPath, testiporterSoundData, "ITM_Bubblerum_Throw");
        bubblerumThrowSound.subtitle = true;
        SoundObject bubblerumExplodeSound = CreateSound("Bubblerum_Soda_Explosion", audioPath, testiporterSoundData, "ITM_Bubblerum_Explode");
        bubblerumExplodeSound.subtitle = true;

        ItemObject bubblerumObject = new ItemBuilder(Info).SetNameAndDescription("ITM_Bubblerum", "ITM_Bubblerum_Desc").SetSprites(bubblerumSpriteSmall, bubblerumSprite).SetEnum("Bubblerum")
            .SetShopPrice(450)
            .SetGeneratorCost(60)
            .SetItemComponent<ITM_BubblerumSoda>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();

        assetMan.Add("Bubblerum", bubblerumObject);
        assetMan.Add("BubblerumSprite", bubblerumSprite);
        assetMan.Add("ReallyEvilBubblerum", bubblerumSpriteEvil);
        assetMan.Add("BubblerumSplat3", bubblerumSplatA);
        assetMan.Add("BubblerumSplat2", bubblerumSplatB);
        assetMan.Add("BubblerumSplat1", bubblerumSplatC);
        assetMan.Add("BubblerumLiquid", bubblerumLiquid);
        assetMan.Add("BubblerumParticle", bubblerumParticle);
        assetMan.Add("Bubblerum_Throw", bubblerumThrowSound);
        assetMan.Add("Bubblerum_Explode", bubblerumExplodeSound);

        SodaMachine bubblerumMachine = MakeSodaMachine(AssetLoader.TextureFromFile(Path.Combine(texturePath, "BubblerumSoda_Machine.png")), AssetLoader.TextureFromFile(Path.Combine(texturePath, "BubblerumSoda_Machine_Out.png")));
        bubblerumMachine.item = bubblerumObject;
        debugBubblerum = Instantiate(FindResourceOfName<GenericHallBuilder>("BsodaHallBuilder", null));
        Logger.LogInfo(debugBubblerum);
        ObjectPlacer myObjectPlacer = debugBubblerum.objectPlacer;
        myObjectPlacer.prefab = bubblerumMachine.gameObject;
        debugBubblerum.objectPlacer = myObjectPlacer;
        debugBubblerum.MarkAsNeverUnload();
        debugBubblerum.gameObject.ConvertToPrefab(setActive: true);
        assetMan.Add("BubblerumMachine", debugBubblerum);

        #endregion

        #region Luckiest Coin
        Logger.LogInfo("Loading Luckiest Coin");

        Sprite luckSprite = CreateSprite("LuckiestCoin_Icon_Large", texturePath, 0.5f, 50f);
        Sprite luckSpriteSmall = CreateSprite("LuckiestCoin_Icon_Small", texturePath, 0.5f, 25f);

        SoundObject luckSound = CreateSound("Lucky", audioPath, testiporterSoundData, "ITM_LuckiestCoin_Use");
        luckSound.subtitle = true;

        ItemObject luckiestCoinObject = new ItemBuilder(Info).SetNameAndDescription("ITM_LuckiestCoin", "ITM_LuckiestCoin_Desc").SetSprites(luckSpriteSmall, luckSprite).SetEnum("LuckiestCoin")
            .SetShopPrice(99)
            .SetGeneratorCost(60)
            .SetItemComponent<ITM_LuckiestCoin>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();
        assetMan.Add("Lucky", luckSound);
        assetMan.Add("LuckiestCoin", luckiestCoinObject);
        #endregion

        #region Canned Test
        Logger.LogInfo("Loading Canned Test");

        Sprite canSprite = CreateSprite("CannedTest_Large", texturePath, 0.5f, 50f);
        Sprite canSpriteSmall = CreateSprite("CannedTest_Small", texturePath, 0.5f, 25f);

        SoundObject canSoundA = CreateSound("CannedTestDispense", audioPath, testiporterSoundData, "ITM_CannedTest_Dispense");
        canSoundA.subtitle = true;
        SoundObject canSoundB = CreateSound("CannedTestPush", audioPath, testiporterSoundData, "ITM_CannedTest_Push");
        canSoundB.subtitle = true;

        ItemObject cannedTestObject = new ItemBuilder(Info).SetNameAndDescription("ITM_CannedTest", "ITM_CannedTest_Desc").SetSprites(canSpriteSmall, canSprite).SetEnum("CannedTest")
            .SetShopPrice(450)
            .SetGeneratorCost(60)
            .SetItemComponent<ITM_CannedTest>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();
        assetMan.Add("CannedTestDispense", canSoundA);
        assetMan.Add("CannedTestPush", canSoundB);
        assetMan.Add("CannedTest", cannedTestObject);

        SodaMachine testMachine = MakeSodaMachine((Texture)(object)AssetLoader.TextureFromFile(texturePath + "/CannedTestMachine.png"), (Texture)(object)AssetLoader.TextureFromFile(texturePath + "/CannedTestMachine_Out.png"));
        testMachine.item = cannedTestObject;
        debugCannedTest = Instantiate<GenericHallBuilder>(FindResourceOfName<GenericHallBuilder>("BsodaHallBuilder", null));
        myObjectPlacer = debugCannedTest.objectPlacer;
        myObjectPlacer.prefab = testMachine.gameObject;
        debugCannedTest.objectPlacer = myObjectPlacer;
        debugCannedTest.MarkAsNeverUnload();
        debugCannedTest.gameObject.ConvertToPrefab(setActive: true);

        assetMan.Add("CannedTestMachine", debugCannedTest);
        Sprite cannysprite = CreateSprite("CannedTestNPC", texturePath, 0.5f, 256f);
        Sprite cannyspriteb = CreateSprite("CannedTestNPCSplat", texturePath, 0.5f, 256f);
        assetMan.Add("CannedTestNPC", cannysprite);
        assetMan.Add("CannedTestNPCSplat", cannyspriteb);

        PosterObject cannyCharacterPoster = ObjectCreators.CreateCharacterPoster(AssetLoader.TextureFromFile(Path.Combine(texturePath, "pri_dasketch.png")), "PST_DaSketch_Name", "PST_DaSketch_Desc");

        Canny canny = new NPCBuilder<Canny>(Info).SetName("Canny").SetEnum("Canny")
            .AddSpawnableRoomCategories([RoomCategory.Hall, RoomCategory.Class, RoomCategory.Faculty, RoomCategory.Special])
            .SetPoster(cannyCharacterPoster)
            .Build();
        assetMan.Add("Canny", canny);
        #endregion

        #region Good Soup
        Logger.LogInfo("Loading Good Soup");

        Sprite soupSprite = CreateSprite("GoodSoup", texturePath, 0.5f, 50f);
        Sprite soupSpriteSmall = CreateSprite("GoodSoupSmall", texturePath, 0.5f, 25f);

        SoundObject soupSound = CreateSound("GoodSoupEat", audioPath, testiporterSoundData, "ITM_GoodSoup_Use");
        soupSound.subtitle = true;

        ItemObject goodSoupObject = new ItemBuilder(Info).SetNameAndDescription("ITM_GoodSoup", "ITM_GoodSoup_Desc").SetSprites(soupSpriteSmall, soupSprite).SetEnum("GoodSoup")
            .SetShopPrice(325)
            .SetGeneratorCost(60)
            .SetItemComponent<ITM_GoodSoup>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();

        assetMan.Add("GoodSoupEat", soupSound);
        assetMan.Add("GoodSoup", goodSoupObject);
        #endregion

        #region Testy Dar
        Logger.LogInfo("Loading Testy Dar");

        Sprite darSprite = CreateSprite("TestyDarIcon_Large", texturePath, 0.5f, 50f);
        Sprite darSpriteSmall = CreateSprite("TestyDarIcon_Small", texturePath, 0.5f, 25f);

        SoundObject darSound = CreateSound("TestyDarEat", audioPath, testiporterSoundData, "ITM_TestyDar_Use");
        darSound.subtitle = true;

        ItemObject testyDarObject = new ItemBuilder(Info).SetNameAndDescription("ITM_TestyDar", "ITM_TestyDar_Desc").SetSprites(darSpriteSmall, darSprite).SetEnum("TestyDar")
            .SetShopPrice(550)
            .SetGeneratorCost(60)
            .SetItemComponent<ITM_TestyDar>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();

        assetMan.Add("TestyDarEat", darSound);
        assetMan.Add("TestyDar", testyDarObject);

        SodaMachine testyDachine = MakeSodaMachine((Texture)(object)AssetLoader.TextureFromFile(texturePath + "/TestyDachine.png"), (Texture)(object)AssetLoader.TextureFromFile(texturePath + "/TestyDachine_Out.png"));
        testyDachine.item = testyDarObject;
        debugTestyDar = Object.Instantiate<GenericHallBuilder>(FindResourceOfName<GenericHallBuilder>("BsodaHallBuilder", (AssetManager)null));
        myObjectPlacer = debugTestyDar.objectPlacer;
        myObjectPlacer.prefab = testyDachine.gameObject;
        debugTestyDar.objectPlacer = myObjectPlacer;
        debugTestyDar.MarkAsNeverUnload();
        debugTestyDar.gameObject.ConvertToPrefab(setActive: true);
        assetMan.Add("TestyDachine", debugTestyDar);
        #endregion

        #region Test YTPs
        Logger.LogInfo("Loading Test YTPs");

        Sprite ytpSprite = CreateSprite("YTP_Test", texturePath, 0.5f, 50f);
        Sprite ytpSpriteEVIL = CreateSprite("YTP_TestAngry", texturePath, 0.5f, 50f);

        SoundObject ytpSound = CreateSound("TestYTP", audioPath, testiporterSoundData, "ITM_TestYTP_Use");
        ytpSound.subtitle = false;
        SoundObject ytpSoundEVIL = CreateSound("TestYTPEVIL", audioPath, testiporterSoundData, "ITM_TestYTPEVIL_Use");
        ytpSoundEVIL.subtitle = false;

        ItemObject testYTPObject = new ItemBuilder(Info).SetNameAndDescription("ITM_TestYTP", "ITM_TestYTP_Desc").SetSprites(ytpSprite, ytpSprite).SetEnum("TestYTP")
            .SetShopPrice(300)
            .SetGeneratorCost(60)
            .SetItemComponent<ITM_TestYTP>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();
        testYTPObject.audPickupOverride = ytpSound;
        testYTPObject.addToInventory = false;

        ItemObject evilTestYTPObject = new ItemBuilder(Info).SetNameAndDescription("ITM_TestYTPEVIL", "ITM_TestYTPEVIL_Desc").SetSprites(ytpSpriteEVIL, ytpSpriteEVIL).SetEnum("TestYTPEVIL")
            .SetShopPrice(600)
            .SetGeneratorCost(60)
            .SetItemComponent<ITM_TestYTPEVIL>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();
        evilTestYTPObject.audPickupOverride = ytpSoundEVIL;
        evilTestYTPObject.addToInventory = false;

        SoundObject ytpLoopSound = CreateSound("TestYTPLoop", audioPath, testiporterSoundData, "ITM_TestYTP_Loop");
        ytpLoopSound.subtitle = false;
        assetMan.Add("TestYTPLoop", ytpLoopSound);

        SoundObject ytpLoopSoundEVIL = CreateSound("TestYTPEVILLoop", audioPath, testiporterSoundData, "ITM_TestYTPEVIL_Loop");
        ytpLoopSoundEVIL.subtitle = false;
        assetMan.Add("TestYTPEVILLoop", ytpLoopSoundEVIL);

        assetMan.Add("TestYTP", testYTPObject);
        assetMan.Add("TestYTPEVIL", evilTestYTPObject);
        #endregion

        #region Eye Sharpening Potion
        Logger.LogInfo("Loading Eye Sharpening Potion");

        Sprite eyeSprite = CreateSprite("PotionBig", texturePath, 0.5f, 50f);
        Sprite eyeSpriteSmall = CreateSprite("PotionSmall", texturePath, 0.5f, 25f);

        SoundObject eyeSound = CreateSound("Drink", audioPath, testiporterSoundData, "ITM_Potion_Use");
        eyeSound.subtitle = true;

        ItemObject eyePotionObject = new ItemBuilder(Info).SetNameAndDescription("ITM_Potion", "ITM_Potion_Desc").SetSprites(eyeSpriteSmall, eyeSprite).SetEnum("Potion")
            .SetShopPrice(500)
            .SetGeneratorCost(60)
            .SetItemComponent<ITM_Potion>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();

        assetMan.Add("Drink", eyeSound);
        assetMan.Add("Potion", eyePotionObject);
        #endregion

        #region Laptop
        Logger.LogInfo("Loading Laptop");

        Sprite laptopSprite = CreateSprite("Testerly_Laptop_Entity", texturePath, 0.5f, 100f);
        Sprite laptopSpriteSmall = CreateSprite("Testerly_Laptop_Entity", texturePath, 0.5f, 50f);

        ItemObject laptopObject = new ItemBuilder(Info).SetNameAndDescription("ITM_Laptop", "ITM_Laptop_Desc").SetSprites(laptopSpriteSmall, laptopSprite).SetEnum("Laptop")
            .SetShopPrice(99)
            .SetGeneratorCost(60)
            .SetItemComponent<ITM_Laptop>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();

        assetMan.Add("Laptop", laptopObject);
        #endregion

        #region Test Folder
        Logger.LogInfo("Loading Test Folder");

        List<string> folderTextureNames =
        [
            "FolderSpin0000",
            "FolderSpin0001",
            "FolderSpin0002",
            "FolderSpin0003",
            "FolderSpin0004",
            "FolderSpin0005",
            "FolderSpin0006",
            "FolderSpin0007",
            "FolderTalk0000",
            "FolderTalk0001",
            "FolderTalk0002",
            "FolderTalk0003",
            "TestFolderPoster"
        ];

        for (int i = 0; i < folderTextureNames.Count; i++)
        {
            Sprite sprite = CreateSprite(folderTextureNames[i], texturePath, 0.5f, 256f);
            assetMan.Add(folderTextureNames[i], sprite);
        }

        List<string> folderSoundNames =
        [
            "TestFolder_Use",
            "TestFolder_Bounce",
            "TestFolder_Despawn",
            "TestFolder_Loop" // No subtitles
        ];

        for (int s = 0; s < folderSoundNames.Count; s++)
        {
            SoundObject sound = CreateSound(folderSoundNames[s], audioPath, testiporterSoundData, "ITM_" + folderSoundNames[s]);
            sound.subtitle = s <= 2;
            assetMan.Add(folderSoundNames[s], sound);
        }

        Sprite folSprite = CreateSprite("FolderTest_Large", texturePath, 0.5f, 50f);
        Sprite folSpriteSmall = CreateSprite("FolderTest_Small", texturePath, 0.5f, 25f);

        ItemObject testFolderObject = new ItemBuilder(Info).SetNameAndDescription("ITM_TestFolder", "ITM_TestFolder_Desc").SetSprites(folSpriteSmall, folSprite).SetEnum("TestFolder")
            .SetShopPrice(300)
            .SetGeneratorCost(60)
            .SetItemComponent<ITM_TestFolder>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();

        assetMan.Add("TestFolder", testFolderObject);
        #endregion

        #region Gummin Chunk
        Logger.LogInfo("Loading Gummin Chunk");

        Sprite chunkSprite = CreateSprite("GumminChunk_Icon_Large", texturePath, 0.5f, 50f);
        Sprite chunkSpriteSmall = CreateSprite("GumminChunk_Icon_Small", texturePath, 0.5f, 25f);

        ItemObject gumminChunkObject = new ItemBuilder(Info).SetNameAndDescription("ITM_GumminChunk", "ITM_GumminChunk_Desc").SetSprites(chunkSpriteSmall, chunkSprite).SetEnum("GumminChunk")
            .SetShopPrice(99)
            .SetGeneratorCost(60)
            .SetItemComponent<ITM_GumminChunk>()
            .SetMeta(ItemFlags.Persists, [])
            .Build();
        assetMan.Add("GumminChunk", gumminChunkObject);
        #endregion

        #region Test Vase
        Logger.LogInfo("Loading Test Vase");

        debugVase = Instantiate(FindResourceOfName<GenericHallBuilder>("PlantBuilder", null));
        myObjectPlacer = debugVase.objectPlacer;

        GameObject vase = Instantiate(myObjectPlacer.prefab);
        assetMan.Add("vase", CreateSprite("Test_Vase", texturePath, 0f, 15f));
        vase.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = CreateSprite("Test_Vase", texturePath, 0f, 15f);
        vase.ConvertToPrefab(setActive: true);
        myObjectPlacer.prefab = vase;
        debugVase.objectPlacer = myObjectPlacer;
        debugVase.MarkAsNeverUnload();
        debugVase.gameObject.ConvertToPrefab(setActive: true);
        assetMan.Add("TestVase", debugVase);
        #endregion

        #endregion

        #region Characters

        #region Bluxam

        Logger.LogInfo("Loading Bluxam");
        SoundMetadata bluxamSoundData = new(SoundType.Voice, Color.blue);

        assetMan.Add("Bluxam", CreateSprite("Bluxam", texturePath, 0.5f, 256f));

        List<string> characterSoundNames =
        [
            "Bluxam_ThankLater",
            "Bluxam_HereWeAre",
            "Bluxam_OhOk",
            "Bluxam_Nevermind",
            "Bluxam_MindIfHelp",
            "Bluxam_GonnaBeLate", 
            // Subtitles
            "NPC_Bluxam_ThankLater",
            "NPC_Bluxam_HereWeAre",
            "NPC_Bluxam_OhOk",
            "NPC_Bluxam_Nevermind",
            "NPC_Bluxam_MindIfHelp",
            "NPC_Bluxam_GonnaBeLate"
        ];

        for (int i = 0; i < 6; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, bluxamSoundData, characterSoundNames[i + 6]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        Texture2D characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_bluxam.png");
        PosterObject characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_Bluxam_Name", "PST_Bluxam_Desc");

        Bluxam bluxam = new NPCBuilder<Bluxam>(Info).SetName("Bluxam").SetEnum("Bluxam").AddLooker().AddTrigger()
            .AddSpawnableRoomCategories([RoomCategory.Faculty, RoomCategory.Class])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("Bluxam", bluxam);
        #endregion

        #region Orlan 7
        Logger.LogInfo("Loading Orlan 7");

        assetMan.Add("Orlan", CreateSprite("Orlan_7", texturePath, 0.5f, 256f));

        SoundMetadata orlanSoundData = new(SoundType.Voice, new Color(1f, 0.5f, 0f, 1f));
        characterSoundNames = ["Orlan7_Snatch", "Orlan7_Laugh", "Orlan7_AintGettingBack", "Orlan7_BelongToMe", "Orlan7_Poor", "NPC_Orlan_Snatch", "NPC_Orlan_Laugh", "NPC_Orlan_AintGettingBack", "NPC_Orlan_BelongToMe", "NPC_Orlan_Poor"];

        for (int i = 0; i < 5; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, orlanSoundData, characterSoundNames[i + 5]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_orlan.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_Orlan_Name", "PST_Orlan_Desc");

        Orlan orlan = new NPCBuilder<Orlan>(Info).SetName("Orlan 7").SetEnum("Orlan").AddLooker().AddTrigger()
            .AddSpawnableRoomCategories([RoomCategory.Hall])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("Orlan", orlan);
        #endregion

        #region Plit
        Logger.LogInfo("Loading Plit");

        assetMan.Add("PlitSpriteMain", CreateSprite("Plit", texturePath, 0.5f, 256f));
        assetMan.Add("PlitSpriteLunge", CreateSprite("Plit_Lunge", texturePath, 0.5f, 256f));
        assetMan.Add("PlitSpriteStun", CreateSprite("Plit_Stunned", texturePath, 0.5f, 256f));

        SoundMetadata plitSoundData = new(SoundType.Voice, new Color(1f, 0.25f, 1f, 1f));
        characterSoundNames =
        [
            "Plit_Idle1", "Plit_Idle2", "Plit_Idle3", "Plit_Idle4", "Plit_Lunge1", "Plit_Lunge2", "Plit_Lunge3", "Plit_Attack_Loop", "Plit_Stunned", "Bang",
            "NPC_Plit_Speech", "NPC_Plit_Speech", "NPC_Plit_Speech", "NPC_Plit_Speech", "NPC_Plit_Speech", "NPC_Plit_Speech", "NPC_Plit_Speech", "NPC_Plit_Fight", "NPC_Plit_Stun", "NPC_Plit_Bang"
        ];

        for (int i = 0; i < 10; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, plitSoundData, characterSoundNames[i + 10]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_plit.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_Plit_Name", "PST_Plit_Desc");

        Plit plit = new NPCBuilder<Plit>(Info).SetName("Plit").SetEnum("Plit").AddLooker()
            .AddSpawnableRoomCategories([RoomCategory.Hall, RoomCategory.Class, RoomCategory.Faculty])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("Plit", plit);
        #endregion

        #region Question Shelfman
        Logger.LogInfo("Loading Question Shelfman");

        assetMan.Add("Shelfman", CreateSprite("Question_Shelfman", texturePath, 0.5f, 200f));

        SoundMetadata shelfmanSoundData = new(SoundType.Voice, new Color(1f, 0.5f, 0f, 1f));
        characterSoundNames =
        [
            "Shelfman_Question", "Shelfman_Huh1", "Shelfman_Huh2", "Shelfman_Huh3", "Shelfman_Huh4", "Shelfman_Huh5", "Shelfman_Testholder",
            "NPC_Shelfman_Ask", "NPC_Shelfman_Huh", "NPC_Shelfman_Huh", "NPC_Shelfman_Huh", "NPC_Shelfman_Huh", "NPC_Shelfman_Huh", "NPC_Shelfman_Testholder"
        ];

        for (int i = 0; i < 7; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, shelfmanSoundData, characterSoundNames[i + 7]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_shelfman.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_Shelfman_Name", "PST_Shelfman_Desc");

        Shelfman shelfman = new NPCBuilder<Shelfman>(Info).SetName("Question Shelfman").SetEnum("Shelfman").AddLooker().AddTrigger()
            .AddSpawnableRoomCategories([RoomCategory.Hall, RoomCategory.Class, RoomCategory.Faculty])
            .SetPoster(characterPosterObject)
            .SetAirborne()
            //.IgnoreBelts()
            .IgnorePlayerOnSpawn()
            .Build();
        assetMan.Add("Shelfman", shelfman);
        #endregion

        #region Yestholomew
        Logger.LogInfo("Loading Yestholomew");

        Sprite yesholomewScreamSprite = CreateSprite("YestholomewScream2", texturePath, 0.5f, 350f);
        assetMan.Add("YestholomewIdle", CreateSprite("YestholomewIdle", texturePath, 0.5f, 350f));
        assetMan.Add("YestholomewScream1", CreateSprite("YestholomewScream1", texturePath, 0.5f, 350f));
        assetMan.Add("YestholomewScream2", yesholomewScreamSprite);
        assetMan.Add("YestholomewScream3", CreateSprite("YestholomewScream3", texturePath, 0.5f, 350f));
        assetMan.Add("YestholomewScream4", yesholomewScreamSprite);

        SoundMetadata yestholomewSoundData = new(SoundType.Voice, Color.green);
        characterSoundNames =
        [
            "Yestholomew_LockedInRoom", "Yestholomew_Scream1", "Yestholomew_Scream2", "Yestholomew_Scream3", "Yestholomew_Scream4", "Yestholomew_Scream5", "NPC_Yestholomew_Cower", "NPC_Yestholomew_Scream", "NPC_Yestholomew_Scream", "NPC_Yestholomew_Scream",
            "NPC_Yestholomew_Scream", "NPC_Yestholomew_Scream"
        ];

        for (int i = 0; i < 6; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, yestholomewSoundData, characterSoundNames[i + 6]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_yestholomew.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_Yestholomew_Name", "PST_Yestholomew_Desc");

        Yestholemew yestholomew = new NPCBuilder<Yestholemew>(Info).SetName("Yestholomew").SetEnum("Yestholomew").AddLooker()
            .AddSpawnableRoomCategories([RoomCategory.Class])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("Yestholomew", yestholomew);
        #endregion

        #region Testholder9
        Logger.LogInfo("Loading Testholder9");

        assetMan.Add("TestholderMain", CreateSprite("testholder9", texturePath, 0.5f, 512f));
        assetMan.Add("TestholderFrown", CreateSprite("testholder9Frown", texturePath, 0.5f, 512f));

        SoundMetadata testholderSoundData = new(SoundType.Voice, new Color(0.5f, 1f, 0.5f));
        characterSoundNames = ["Testholder9_Idle1", "Testholder9_Idle2", "Testholder9_Idle3", "Testholder9_Idle4", "Testholder9_Idle5", "NPC_Testholder_Talk", "NPC_Testholder_Talk", "NPC_Testholder_Talk", "NPC_Testholder_Talk", "NPC_Testholder_Talk"];

        for (int i = 0; i < 5; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, testholderSoundData, characterSoundNames[i + 5]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_testholder9.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_Testholder_Name", "PST_Testholder_Desc");

        Testholder testholder = new NPCBuilder<Testholder>(Info).SetName("Testholder9").SetEnum("Testholder")
            .AddSpawnableRoomCategories([RoomCategory.Class])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("Testholder", testholder);
        #endregion

        #region Cren
        Logger.LogInfo("Loading Cren");

        assetMan.Add("CrenInbetween", CreateSprite("CrenInbetween", texturePath, 0.5f, 256f));
        assetMan.Add("CrenWalk1", CreateSprite("CrenWalk1", texturePath, 0.5f, 256f));
        assetMan.Add("CrenWalk2", CreateSprite("CrenWalk2", texturePath, 0.5f, 256f));
        assetMan.Add("CrenSpitPrepare1", CreateSprite("CrenSpitPrepare1", texturePath, 0.5f, 256f));
        assetMan.Add("CrenSpitPrepare2", CreateSprite("CrenSpitPrepare2", texturePath, 0.5f, 256f));
        assetMan.Add("CrenSpitOut1", CreateSprite("CrenSpitOut1", texturePath, 0.5f, 256f));
        assetMan.Add("CrenSpitOut2", CreateSprite("CrenSpitOut2", texturePath, 0.5f, 256f));

        SoundMetadata crenSoundData = new(SoundType.Voice, Color.green);
        characterSoundNames =
        [
            "Cren_Walk1",
            "Cren_Walk2",
            "Cren_Spit",
            "Cren_RunAway",
            "Cren_DashSfx", 
            // Subtitles
            "NPC_Cren_Step",
            "NPC_Cren_Step",
            "NPC_Cren_Spit",
            "NPC_Cren_RunAway",
            "NPC_Cren_DashSfx"
        ];

        for (int i = 0; i < 5; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, crenSoundData, characterSoundNames[i + 5]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_cren.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_Cren_Name", "PST_Cren_Desc");

        Cren cren = new NPCBuilder<Cren>(Info).SetName("Cren").SetEnum("Cren").AddLooker()
            .AddSpawnableRoomCategories([RoomCategory.Class])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("Cren", cren);
        #endregion

        #region The Best One
        Logger.LogInfo("Loading The Best One");

        assetMan.Add("TheBestOne", CreateSprite("The_Best_One", texturePath, 0.5f, 400f));
        assetMan.Add("Urine", CreateSprite("Urine", texturePath, 0.5f, 32f));
        assetMan.Add("UrineRed", CreateSprite("UrineRed", texturePath, 0.5f, 32f));

        SoundMetadata thebestoneSoundData = new(SoundType.Voice, Color.green);
        characterSoundNames =
        [
            "TheBestOne_Yellowing",
            "TheBestOne_Yellow_1",
            "TheBestOne_Yellow_2",
            "TheBestOne_Yellow_3",
            "TheBestOne_Yellow_4",
            "TheBestOne_Yellow_5",
            // Subtitles
            "NPC_TheBestOne_Yellowing",
            "NPC_TheBestOne_Yellow_1",
            "NPC_TheBestOne_Yellow_2",
            "NPC_TheBestOne_Yellow_3",
            "NPC_TheBestOne_Yellow_4",
            "NPC_TheBestOne_Yellow_5"
        ];

        for (int i = 0; i < 6; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, thebestoneSoundData, characterSoundNames[i + 6]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_thebestone.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_TheBestOne_Name", "PST_TheBestOne_Desc");

        TheBestOne thebestone = new NPCBuilder<TheBestOne>(Info).SetName("The Best One").SetEnum("TheBestOne").AddLooker()
            .AddSpawnableRoomCategories([RoomCategory.Hall, RoomCategory.Class, RoomCategory.Faculty, RoomCategory.Office, RoomCategory.Special])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("TheBestOne", thebestone);
        #endregion

        #region AJOPI9
        Logger.LogInfo("Loading AJOPI9");

        assetMan.Add("AJOPI9Calm", CreateSprite("AJOPI9Calm", texturePath, 0.5f, 350f));
        assetMan.Add("AJOPI9Start1", CreateSprite("AJOPI9Start1", texturePath, 0.5f, 350f));
        assetMan.Add("AJOPI9Start2", CreateSprite("AJOPI9Start2", texturePath, 0.5f, 350f));
        assetMan.Add("AJOPI9Start3", CreateSprite("AJOPI9Start3", texturePath, 0.5f, 350f));
        assetMan.Add("AJOPI9Scream", CreateSprite("AJOPI9Scream", texturePath, 0.5f, 350f));
        assetMan.Add("AJOPI9Red", CreateSprite("AJOPI9Red", texturePath, 0.5f, 350f));

        SoundMetadata ajopi9SoundData = new(SoundType.Voice, new Color(0f, 0.25f, 0f));
        characterSoundNames =
        [
            "ajopi9Begin",
            "ajopi9Loop",
            "ajopi9Blind",
            // Subtitles
            "NPC_AJOPI9_Scream",
            "NPC_AJOPI9_Scream",
            "NPC_AJOPI9_Blind"
        ];

        for (int i = 0; i < 3; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, ajopi9SoundData, characterSoundNames[i + 3]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_AJOPI9.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_AJOPI9_Name", "PST_AJOPI9_Desc");

        AJOPI ajopi = new NPCBuilder<AJOPI>(Info).SetName("AJOPI9").SetEnum("AJOPI").AddLooker()
            .AddSpawnableRoomCategories([RoomCategory.Hall])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("AJOPI9", ajopi);
        #endregion

        #region Me Mest
        Logger.LogInfo("Loading Me Mest");

        assetMan.Add("MeMestMain", CreateSprite("MeMestIdle", texturePath, 0.5f, 128f));
        assetMan.Add("MeMestStop", CreateSprite("MeMestStop", texturePath, 0.5f, 128f));
        assetMan.Add("MeMestShove", CreateSprite("MeMestShove", texturePath, 0.5f, 128f));
        assetMan.Add("MeMestWrench", CreateSprite("MeMestWrench", texturePath, 0.5f, 128f));

        SoundMetadata meMestSoundData = new(SoundType.Voice, Color.cyan);
        characterSoundNames = ["MeMest_CannotCross", "MeMest_Pushed", "MeMest_Stop", "MeMest_WTH", "MeMest_Scream", "NPC_MeMest_CannotCross", "NPC_MeMest_Pushed", "NPC_MeMest_Stop", "NPC_MeMest_WTH", "NPC_MeMest_Scream"];

        for (int i = 0; i < 5; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, meMestSoundData, characterSoundNames[i + 5]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_memest.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_MeMest_Name", "PST_MeMest_Desc");

        MeMest memest = new NPCBuilder<MeMest>(Info).SetName("Me Mest").SetEnum("MeMest")
            .AddSpawnableRoomCategories([RoomCategory.Hall])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("MeMest", memest);
        #endregion

        #region PalbyFan1
        Logger.LogInfo("Loading PalbyFan1");

        List<string> characterTextureNames =
        [
            "_Mad0",
            "_Mad1",
            "_Punch0",
            "_Punch1",
            "_Run0",
            "_Run1",
            "_Show0",
            "_Show1",
            "_Show2"
        ];

        for (int i = 0; i < characterTextureNames.Count; i++)
        {
            Sprite sprite = CreateSprite(string.Concat("PalbyFan1", characterTextureNames[i]), texturePath, 0.5f, 256f);
            assetMan.Add(string.Concat("PalbyFan", characterTextureNames[i]), sprite);
        }

        SoundMetadata palbySoundData = new(SoundType.Voice, Color.magenta);
        characterSoundNames =
        [
            "PalbyFan_Look",
            "PalbyFan_UhOh",
            "PalbyFan_Punch",
            "PalbyFan_AfterPunch",
            "NPC_PalbyFan_Look",
            "NPC_PalbyFan_UhOh",
            "NPC_PalbyFan_Punch",
            "NPC_PalbyFan_AfterPunch"
        ];

        for (int i = 0; i < 4; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, palbySoundData, characterSoundNames[i + 4]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_palbyfan.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_PalbyFan_Name", "PST_PalbyFan_Desc");

        PalbyFan palbyfan = new NPCBuilder<PalbyFan>(Info).SetName("PalbyFan1").SetEnum("PalbyFan").AddLooker()
            .AddSpawnableRoomCategories([RoomCategory.Hall, RoomCategory.Faculty])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("PalbyFan", palbyfan);
        #endregion

        #region Da Sketch
        Logger.LogInfo("Loading Da Sketch");

        assetMan.Add("DaSketch_Alert2", CreateSprite("DaSketch_Alert2", texturePath, 0.5f, 256f));
        assetMan.Add("DaSketch_Alert1", CreateSprite("DaSketch_Alert1", texturePath, 0.5f, 256f));
        assetMan.Add("DaSketch_Alert0", CreateSprite("DaSketch_Alert0", texturePath, 0.5f, 256f));
        assetMan.Add("DaSketch_Idle", CreateSprite("DaSketch_Idle", texturePath, 0.5f, 256f));
        assetMan.Add("DaSketch_PaperSpawn_0", CreateSprite("DaSketch_PaperSpawn_0", texturePath, 0f, 50f));

        SoundMetadata dasketchSoundData = new(SoundType.Voice, Color.gray);
        characterSoundNames =
        [
            "DaSketch_Alert",
            "DaSketch_Spawn",
            // Subtitles
            "NPC_DaSketch_Alert",
            "NPC_DaSketch_Spawn"
        ];

        for (int i = 0; i < 2; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, dasketchSoundData, characterSoundNames[i + 2]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_dasketch.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_DaSketch_Name", "PST_DaSketch_Desc");

        DaSketch dasketch = new NPCBuilder<DaSketch>(Info).SetName("Da Sketch").SetEnum("DaSketch")
            .AddSpawnableRoomCategories([RoomCategory.Class])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("DaSketch", dasketch);
        #endregion

        #region Qrid
        Logger.LogInfo("Loading Qrid");

        assetMan.Add("Qrid_Docile", CreateSprite("Qrid_Docile", texturePath, 0.5f, 495f));
        assetMan.Add("Qrid_Hostile", CreateSprite("Qrid_Hostile", texturePath, 0.5f, 495f));

        SoundMetadata qridSoundNames = new(SoundType.Voice, Color.red);
        characterSoundNames =
        [
            "Qrid_Intro",
            "Qrid_Loop",
            "Qrid_Friend1",
            "Qrid_Friend2",
            "Qrid_Anger1",
            "Qrid_Anger2",
            "Qrid_Anger3",
            "Qrid_Leave1",
            "Qrid_Leave2",
            "Qrid_Leave3",
            "Qrid_NPC1",
            "Qrid_NPC2",
            "Qrid_NPC3",
            "Qrid_NPCLeave1",
            "Qrid_NPCLeave2",
            "Qrid_NPCLeave3",
            // Subtitles
            "NPC_Qrid_Intro",
            "NPC_Qrid_Loop",
            "NPC_Qrid_Friend1",
            "NPC_Qrid_Friend2",
            "NPC_Qrid_Anger1",
            "NPC_Qrid_Anger2",
            "NPC_Qrid_Anger3",
            "NPC_Qrid_Leave1",
            "NPC_Qrid_Leave2",
            "NPC_Qrid_Leave3",
            "NPC_Qrid_NPC1",
            "NPC_Qrid_NPC2",
            "NPC_Qrid_NPC3",
            "NPC_Qrid_NPCLeave1",
            "NPC_Qrid_NPCLeave2",
            "NPC_Qrid_NPCLeave3"
        ];

        for (int i = 0; i < 16; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, qridSoundNames, characterSoundNames[i + 16]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_qrid.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_Qrid_Name", "PST_Qrid_Desc");

        Qrid qrid = new NPCBuilder<Qrid>(Info).SetName("Qrid").SetEnum("Qrid").AddLooker()
            .AddSpawnableRoomCategories([RoomCategory.Class, RoomCategory.Faculty, RoomCategory.Hall, RoomCategory.Special])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("Qrid", qrid);
        #endregion

        #region The Gummin
        Logger.LogInfo("Loading The Gummin");

        assetMan.Add("Gummin_Idle", CreateSprite("Gummin_Idle", texturePath, 0.5f, 256f));
        assetMan.Add("Gummin_Run1", CreateSprite("Gummin_Run1", texturePath, 0.5f, 256f));
        assetMan.Add("Gummin_Run2", CreateSprite("Gummin_Run2", texturePath, 0.5f, 256f));
        assetMan.Add("Gummin_Give1", CreateSprite("Gummin_Give1", texturePath, 0.5f, 256f));
        assetMan.Add("Gummin_Give2", CreateSprite("Gummin_Give2", texturePath, 0.5f, 256f));

        SoundMetadata gumminSoundData = new(SoundType.Voice, Color.green);
        characterSoundNames =
        [
            "Gummin_Wantsome",
            "Gummin_HereGo",
            "Gummin_TakingandGiving",
            "GumminChunk_Splat",
            "NPC_Gummin_See",
            "NPC_Gummin_Give",
            "NPC_Gummin_Replace",
            "NPC_Gummin_Splat"
        ];

        for (int i = 0; i < 4; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, gumminSoundData, characterSoundNames[i + 4]);
            sound.subtitle = true;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_gummin.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_Gummin_Name", "PST_Gummin_Desc");

        Gummin gummin = new NPCBuilder<Gummin>(Info).SetName("Gummin").SetEnum("Gummin").AddLooker().AddTrigger()
            .AddSpawnableRoomCategories([RoomCategory.Class])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("Gummin", gummin);
        #endregion

        #region xXThroneTestXx 
        Logger.LogInfo("Loading ThroneTest");

        characterTextureNames =
        [
            // Idle
            "ThroneTest_0",
            "ThroneTest_1",
            "ThroneTest_2",
            "ThroneTest_3",
            "ThroneTest_4",
            "ThroneTest_5",
            "ThroneTest_6",
            "ThroneTest_7", 
            // Hit marker
            "ThroneTest_Hitmarker"
        ];

        for (int i = 0; i < characterTextureNames.Count; i++)
        {
            Sprite sprite = CreateSprite(characterTextureNames[i], texturePath, 0.5f, 256f);
            assetMan.Add(characterTextureNames[i], sprite);
        }

        SoundMetadata thronetestSoundData = new(SoundType.Voice, new Color(0.96f, 0.96f, 0.87f));
        characterSoundNames =
        [
            "ThroneTest_Idle_1",
            "ThroneTest_Idle_2",
            "ThroneTest_Idle_3",
            "ThroneTest_Idle_4",
            "ThroneTest_RunOverRandom_1",
            "ThroneTest_RunOverRandom_2",
            "ThroneTest_RunOverRandom_3",
            "ThroneTest_RunOverRandom_4",
            "ThroneTest_Hit",
            "ThroneTest_Bang", // Subtitles are disabled for sounds after this
            "ThroneTest_Combo1",
            "ThroneTest_Combo2",
            "ThroneTest_Combo3",
            "ThroneTest_Combo4",
            "ThroneTest_Combo5Buildup",
            "ThroneTest_Combo5Successful",
            "ThroneTest_ComboLost"
        ];

        for (int i = 0; i < characterSoundNames.Count; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, thronetestSoundData, "NPC_" + characterSoundNames[i]);
            sound.subtitle = i <= 9;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_thronetest.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_ThroneTest_Name", "PST_ThroneTest_Desc");

        ThroneTest thronetest = new NPCBuilder<ThroneTest>(Info).SetName("xXThroneTestXx").SetEnum("ThroneTest").AddTrigger()
            .AddSpawnableRoomCategories([RoomCategory.Hall])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("ThroneTest", thronetest);
        #endregion

        #region Testerly
        Logger.LogInfo("Loading Testerly");

        characterTextureNames =
        [
            // Idle
            "Testerly_Idle_0",
            "Testerly_Idle_1",
            "Testerly_Idle_2",
            "Testerly_Idle_3",
            "Testerly_Idle_4",
            "Testerly_Idle_Peak",
            // Drop - single frame of Testerly in place after her laptop is dropped
            "Testerly_Drop", 
            // Laptops (intact and broken)
            "Testerly_Laptop_Entity",
            "Testerly_Laptop_Entity_Broken",
            // Distraught - Testerly on her knees as she lingers over her broken laptop
            "Testerly_Distraught_0",
            "Testerly_Distraught_1",
            "Testerly_Distraught_2",
            "Testerly_Distraught_3",
            "Testerly_Distraught_4",
            "Testerly_Distraught_5",
            "Testerly_Distraught_6",
            "Testerly_Distraught_7",
            // Run
            "TesterlyRunCycle1_0",
            "TesterlyRunCycle1_1",
            "TesterlyRunCycle1_2",
            "TesterlyRunCycle1_3",
            "TesterlyRunCycle1_4",
            "TesterlyRunCycle1_5",
            "TesterlyRunCycle1_6",
            "TesterlyRunCycle1_7",
            "TesterlyRunCycle2_0",
            "TesterlyRunCycle2_1",
            "TesterlyRunCycle2_2",
            "TesterlyRunCycle2_3",
            "TesterlyRunCycle2_4",
            "TesterlyRunCycle2_5",
            "TesterlyRunCycle2_6",
            "TesterlyRunCycle2_7",
            "TesterlyRunCycle3_0",
            "TesterlyRunCycle3_1",
            "TesterlyRunCycle3_2",
            "TesterlyRunCycle3_3",
            "TesterlyRunCycle3_4",
            "TesterlyRunCycle3_5",
            "TesterlyRunCycle3_6",
            "TesterlyRunCycle3_7",
            // Alert - Testerly waving her arms beside the Principal as she alerts him of her broken laptop
            "Testerly_Alert_0",
            "Testerly_Alert_1",
            // New laptop - Testerly inexplicably pulls out a new laptop from behind her back
            "Testerly_NewLaptop_0",
            "Testerly_NewLaptop_1",
            "Testerly_NewLaptop_2",
            "Testerly_NewLaptop_3",
            "Testerly_NewLaptop_4",
            "Testerly_NewLaptop_5"
        ];

        for (int i = 0; i < characterTextureNames.Count; i++)
        {
            Sprite sprite = CreateSprite(characterTextureNames[i], texturePath, 0.5f, 256f);
            assetMan.Add(characterTextureNames[i], sprite);
        }

        SoundMetadata testerlySoundData = new(SoundType.Voice, Color.yellow);
        characterSoundNames =
        [
            "Testerly_Gasp",
            "Testerly_MyLaptopBroken",
            "Testerly_GonnaRegret",
            "Testerly_Snitching",
            "Testerly_SeekRandom1",
            "Testerly_SeekRandom2",
            "Testerly_AlertPrincipal",
            "TesterlyLaptop_Break",
            "Testerly_Keyboard",
            "Testerly_Run1",
            "Testerly_Run2",
            "Testerly_Bump"
        ];

        for (int i = 0; i < characterSoundNames.Count; i++)
        {
            SoundObject sound = CreateSound(characterSoundNames[i], audioPath, testerlySoundData, "NPC_" + characterSoundNames[i]);
            sound.subtitle = i <= 8;
            assetMan.Add(characterSoundNames[i], sound);
        }

        characterPosterTex = AssetLoader.TextureFromFile(texturePath + "/pri_testerly.png");
        characterPosterObject = ObjectCreators.CreateCharacterPoster(characterPosterTex, "PST_Testerly_Name", "PST_Testerly_Desc");

        Testerly testerly = new NPCBuilder<Testerly>(Info).SetName("Testerly").SetEnum("Testerly").AddTrigger()
            .AddSpawnableRoomCategories([RoomCategory.Hall])
            .SetPoster(characterPosterObject)
            .Build();
        assetMan.Add("Testerly", testerly);
        #endregion

        #endregion
    }

    private void AddNPCs(string floorName, int floorNumber, SceneObject sceneObject)
    {
        if ((sceneObject.levelObject == null || !sceneObject.levelObject) && (sceneObject.randomizedLevelObject == null || sceneObject.randomizedLevelObject.Length == 0))
        {
            Logger.LogWarning($"SceneObject {sceneObject.name} lacks any form of a LevelObject (singular and randomized)");
            Logger.LogInfo("Therefore, Truckload O' Tests cannot apply any of it's changes to the specified SceneObject");
            return;
        }

        if (sceneObject.levelObject != null && sceneObject.levelObject)
        {
            AddNPCs(floorName, floorNumber, sceneObject.levelObject, sceneObject);
        }

        if (sceneObject.randomizedLevelObject != null && sceneObject.randomizedLevelObject.Length > 0)
        {
            foreach (WeightedLevelObject weighedLevelObject in sceneObject.randomizedLevelObject)
            {
                if (weighedLevelObject == null || weighedLevelObject.selection == null || !weighedLevelObject.selection) continue;
                AddNPCs(floorName, floorNumber, weighedLevelObject.selection, sceneObject);
            }
        }
    }

    private void AddNPCs(string floorName, int floorNumber, LevelObject levelObject, SceneObject sceneObject)
    {
        Logger.LogInfo($"{floorNumber}: {floorName} - {levelObject.name} via. {sceneObject.name}");

        string texturePath = Path.Combine(AssetLoader.GetModPath(this), "Textures");

        // Poster implementation

        WeightedPosterObject weightedMeetingArtPosters = new()
        {
            selection = ObjectCreators.CreatePosterObject(
            [
                AssetLoader.TextureFromFile(texturePath + "/TestMeetingArt_B.png"),
                AssetLoader.TextureFromFile(texturePath + "/TestMeetingArt_W.png"),
                AssetLoader.TextureFromFile(texturePath + "/TestMeetingArt_WB.png")
            ]),
            weight = 90
        };

        WeightedPosterObject weightedPresidentPosters = new()
        {
            selection = ObjectCreators.CreatePosterObject(
            [
                AssetLoader.TextureFromFile(texturePath + "/GumminForPresident.png"),
                AssetLoader.TextureFromFile(texturePath + "/TestmanForPresident.png")
            ]),
            weight = 90
        };

        List<WeightedPosterObject> posters = [];
        posters.Add(weightedMeetingArtPosters);
        posters.Add(weightedPresidentPosters);
        posters.Add(CreateWeightedPoster("Talbulicious_Cart_Ride_Poster", 90));
        posters.Add(CreateWeightedPoster("RetroTestPoster", 90));
        posters.Add(CreateWeightedPoster("Porridge_Poster", 90));
        posters.Add(CreateWeightedPoster("OWNED_poster", 90));
        posters.Add(CreateWeightedPoster("Grebstermote_Poster", 90));
        posters.Add(CreateWeightedPoster("BURGE_Poseter", 90));
        posters.Add(CreateWeightedPoster("orlan7tookmywallet", 90));
        posters.Add(CreateWeightedPoster("TOBEREPLACEREDFTW", 90));
        posters.Add(CreateWeightedPoster("Tst_2007", 30));
        posters.Add(CreateWeightedPoster("test_varients_poster", 90));
        posters.Add(CreateWeightedPoster("TepGep_Poster", 90));
        posters.Add(CreateWeightedPoster("PALBY_Poster", 90));
        posters.Add(CreateWeightedPoster("GlowTest_Poster", 90));
        posters.Add(CreateWeightedPoster("CloseUpGret_Poster", 90));
        posters.Add(CreateWeightedPoster("JAKE", 90));
        posters.Add(CreateWeightedPoster("ThatTestdayTeeling_Poster", 90));
        posters.Add(CreateWeightedPoster("IINK_Poster", 90));
        posters.Add(CreateWeightedPoster("Hest_Ough", 90));
        posters.Add(CreateWeightedPoster("Zhe_Meeting", 90));
        posters.Add(CreateWeightedPoster("Hey_Juys_Render", 90));
        posters.Add(CreateWeightedPoster("Helping_hand_v2", 90));
        posters.Add(CreateWeightedPoster("Gret_Drawing_1", 90));
        posters.Add(CreateWeightedPoster("DIMBITE_Foilt", 90));
        posters.Add(CreateWeightedPoster("Bluxam_Drawing", 90));
        posters.Add(CreateWeightedPoster("Plit_Drawing", 90));
        posters.Add(CreateWeightedPoster("Cren_Drawing", 90));
        posters.Add(CreateWeightedPoster("Testholder9_Drawing", 90));
        posters.Add(CreateWeightedPoster("Porridge_Fan_Test_Drawing", 90));
        posters.Add(CreateWeightedPoster("Testember2024_Poster", 90));
        posters.Add(CreateWeightedPoster("YayTestDraw_Poster", 90));
        posters.Add(CreateWeightedPoster("TestOnTheHill_Poster", 90));
        posters.Add(CreateWeightedPoster("TestHeadAlsoMSPaint_Poster", 90));
        posters.Add(CreateWeightedPoster("YeDestFuture", 90));
        posters.Add(CreateWeightedPoster("TheCoolTests", 90));
        posters.Add(CreateWeightedPoster("DeliciousPorridgeAd", 90));
        posters.Add(CreateWeightedPoster("WhereYTP_Poster", 90));
        posters.Add(CreateWeightedPoster("TestigenTextigenTexturePoster", 90));
        posters.Add(CreateWeightedPoster("TestFolderSmilePoster", 90));
        posters.Add(CreateWeightedPoster("BluxamArt", 90));
        posters.Add(CreateWeightedPoster("CrenDrivebyPoster", 90));
        posters.Add(CreateWeightedPoster("DitherBrine", 90));
        posters.Add(CreateWeightedPoster("TestSpeechBubble", 90));
        posters.Add(CreateWeightedPoster("TestSpeechBubble2", 90));
        posters.Add(CreateWeightedPoster("Reference", 90));
        posters.Add(CreateWeightedPoster("BaldiSoulTesty", 90));
        posters.Add(CreateWeightedPoster("TheAjopiComic", 30));
        posters.Add(CreateWeightedPoster("KillerTestRoblox", 90));
        posters.Add(CreateWeightedPoster("TheTestsons", 90));
        posters.Add(CreateWeightedPoster("GumminForPresident", 90));
        posters.Add(CreateWeightedPoster("TestmanForPresident", 90));
        posters.Add(CreateWeightedPoster("TepGepsProblem", 90));
        posters.Add(CreateWeightedPoster("IMG_4615", 90));
        posters.Add(CreateWeightedPoster("TestRecRoomCanvas_Poster", 90));
        posters.Add(CreateWeightedPoster("TestloonDrawingPage_Poster", 90));
        posters.Add(CreateWeightedPoster("TESTISBESTpage_Poster", 90));
        posters.Add(CreateWeightedPoster("TestFaceRippedPage_Poster", 90));
        posters.Add(CreateWeightedPoster("TestFacePage_Poster", 90));
        posters.Add(CreateWeightedPoster("OLDTESTYONTHEWALL_Poster", 90));
        posters.Add(CreateWeightedPoster("ITBURNSpage_Poster", 90));
        posters.Add(CreateWeightedPoster("GrebstermoteDrawing_Poster", 90));
        posters.Add(CreateWeightedPoster("CantStopSeeingPage_Poster", 90));
        posters.Add(CreateWeightedPoster("CartoonTesty", 90));
        posters.Add(CreateWeightedPoster("WRONGWRONGWRONG", 90));
        posters.Add(CreateWeightedPoster("ProtoGretSmiling_Poster", 90));
        posters.Add(CreateWeightedPoster("HeadplumTreeAndGroundplumShrooms", 90));
        posters.Add(CreateWeightedPoster("SestSweeperAndPaulm", 90));
        posters.Add(CreateWeightedPoster("TestBlithered_and_Gigadithered_in_the_water", 90));
        posters.Add(CreateWeightedPoster("theqriddy", 90));
        levelObject.posters = ConcatArray(posters, levelObject.posters);

        // Custom object implementation

        levelObject.forcedStructures = CollectionExtensions.AddToArray(levelObject.forcedStructures, new()
        {
            prefab = FindResourceObjects<Structure_EnvironmentObjectPlacer>().FirstOrDefault(builder => builder.randomlySelectWeightedPrefab) ?? new Structure_EnvironmentObjectPlacer()
            {
                generatedStructureData = [],
                randomlySelectWeightedPrefab = true,
                useWallDirection = true,
                _eligibleDirections = []
            },
            parameters = new()
            {
                chance = [0],
                minMax = [new(1, 3)],
                prefab =
                [
                    new WeightedGameObject()
                    {
                        selection = ((GenericHallBuilder)assetMan.Get<ObjectBuilder>("BubblerumMachine")).objectPlacer.prefab,
                        weight = 175
                    },
                    new WeightedGameObject()
                    {
                        selection = ((GenericHallBuilder)assetMan.Get<ObjectBuilder>("CannedTestMachine")).objectPlacer.prefab,
                        weight = 175
                    },
                    new WeightedGameObject()
                    {
                        selection = ((GenericHallBuilder)assetMan.Get<ObjectBuilder>("TestyDachine")).objectPlacer.prefab,
                        weight = 175
                    }
                ]
            }
        });

        levelObject.forcedStructures = CollectionExtensions.AddToArray(levelObject.forcedStructures, new()
        {
            prefab = FindResourceObjects<Structure_EnvironmentObjectPlacer>().FirstOrDefault(builder => builder.randomlySelectWeightedPrefab) ?? new Structure_EnvironmentObjectPlacer()
            {
                generatedStructureData = [],
                randomlySelectWeightedPrefab = true,
                useWallDirection = true,
                _eligibleDirections = []
            },
            parameters = new()
            {
                chance = [0],
                minMax = [new(12, 24)],
                prefab =
                [
                    new WeightedGameObject()
                    {
                        selection = ((GenericHallBuilder)assetMan.Get<ObjectBuilder>("TestVase")).objectPlacer.prefab,
                        weight = 150
                    }
                ]
            }
        });

        /*
        levelObject.specialHallBuilders = [.. CollectionExtensions.AddToArray<WeightedObjectBuilder>(levelObject.specialHallBuilders, new WeightedObjectBuilder
        {
            selection = assetMan.Get<ObjectBuilder>("BubblerumMachine"),
            weight = 175
        })];

        levelObject.specialHallBuilders = [.. CollectionExtensions.AddToArray<WeightedObjectBuilder>(levelObject.specialHallBuilders, new WeightedObjectBuilder
        {
            selection = assetMan.Get<ObjectBuilder>("CannedTestMachine"),
            weight = 165
        })];

        levelObject.specialHallBuilders = [.. CollectionExtensions.AddToArray<WeightedObjectBuilder>(levelObject.specialHallBuilders, new WeightedObjectBuilder
        {
            selection = assetMan.Get<ObjectBuilder>("TestyDachine"),
            weight = 115
        })];

        levelObject.specialHallBuilders = [.. CollectionExtensions.AddToArray<WeightedObjectBuilder>(levelObject.specialHallBuilders, new WeightedObjectBuilder
        {
            selection = assetMan.Get<ObjectBuilder>("TestVase"),
            weight = 150
        })];
        */

        // Item implementation

        levelObject.potentialItems = [.. CollectionExtensions.AddItem<WeightedItemObject>(levelObject.potentialItems, new WeightedItemObject
        {
            selection = assetMan.Get<ItemObject>("Testiporter"),
            weight = 100
        })];

        levelObject.potentialItems = [.. CollectionExtensions.AddItem<WeightedItemObject>(levelObject.potentialItems, new WeightedItemObject
        {
            selection = assetMan.Get<ItemObject>("Grebstermote"),
            weight = 120
        })];

        levelObject.potentialItems = [.. CollectionExtensions.AddItem<WeightedItemObject>(levelObject.potentialItems, new WeightedItemObject
        {
            selection = assetMan.Get<ItemObject>("Headplum"),
            weight = 130
        })];

        levelObject.potentialItems = [.. CollectionExtensions.AddItem<WeightedItemObject>(levelObject.potentialItems, new WeightedItemObject
        {
            selection = assetMan.Get<ItemObject>("Bubblerum"),
            weight = 130
        })];

        levelObject.potentialItems = [.. CollectionExtensions.AddItem<WeightedItemObject>(levelObject.potentialItems, new WeightedItemObject
        {
            selection = assetMan.Get<ItemObject>("GoodSoup"),
            weight = 130
        })];

        levelObject.potentialItems = [.. CollectionExtensions.AddItem<WeightedItemObject>(levelObject.potentialItems, new WeightedItemObject
        {
            selection = assetMan.Get<ItemObject>("CannedTest"),
            weight = 120
        })];

        levelObject.potentialItems = [.. CollectionExtensions.AddItem<WeightedItemObject>(levelObject.potentialItems, new WeightedItemObject
        {
            selection = assetMan.Get<ItemObject>("LuckiestCoin"),
            weight = 95
        })];

        levelObject.potentialItems = [.. CollectionExtensions.AddItem<WeightedItemObject>(levelObject.potentialItems, new WeightedItemObject
        {
            selection = assetMan.Get<ItemObject>("TestyDar"),
            weight = 90
        })];

        levelObject.potentialItems = [.. CollectionExtensions.AddItem<WeightedItemObject>(levelObject.potentialItems, new WeightedItemObject
        {
            selection = assetMan.Get<ItemObject>("Potion"),
            weight = 120
        })];

        levelObject.potentialItems = [.. CollectionExtensions.AddItem<WeightedItemObject>(levelObject.potentialItems, new WeightedItemObject
        {
            selection = assetMan.Get<ItemObject>("TestYTP"),
            weight = 50
        })];

        levelObject.potentialItems = [.. CollectionExtensions.AddItem<WeightedItemObject>(levelObject.potentialItems, new WeightedItemObject
        {
            selection = assetMan.Get<ItemObject>("TestYTPEVIL"),
            weight = 25
        })];

        // Character implementation

        sceneObject.potentialNPCs.Add(new WeightedNPC
        {
            selection = assetMan.Get<Bluxam>("Bluxam"),
            weight = 140
        });

        sceneObject.potentialNPCs.Add(new WeightedNPC
        {
            selection = assetMan.Get<Orlan>("Orlan"),
            weight = 140
        });

        if (floorName != "F1") // Floor one beyond exclusive characters
        {
            sceneObject.potentialNPCs.Add(new WeightedNPC
            {
                selection = assetMan.Get<Plit>("Plit"),
                weight = 140
            });

            sceneObject.potentialNPCs.Add(new WeightedNPC
            {
                selection = assetMan.Get<Yestholemew>("Yestholomew"),
                weight = 140
            });

            sceneObject.potentialNPCs.Add(new WeightedNPC
            {
                selection = assetMan.Get<AJOPI>("AJOPI9"),
                weight = 15
            });

            sceneObject.potentialNPCs.Add(new WeightedNPC
            {
                selection = assetMan.Get<PalbyFan>("PalbyFan"),
                weight = 140
            });

            sceneObject.potentialNPCs.Add(new WeightedNPC
            {
                selection = assetMan.Get<Qrid>("Qrid"),
                weight = 140
            });

            sceneObject.potentialNPCs.Add(new WeightedNPC
            {
                selection = assetMan.Get<Gummin>("Gummin"),
                weight = 140
            });

            sceneObject.potentialNPCs.Add(new WeightedNPC
            {
                selection = assetMan.Get<ThroneTest>("ThroneTest"),
                weight = 140
            });
        }

        sceneObject.potentialNPCs.Add(new WeightedNPC
        {
            selection = assetMan.Get<Testerly>("Testerly"),
            weight = 140
        });

        sceneObject.potentialNPCs.Add(new WeightedNPC
        {
            selection = assetMan.Get<Shelfman>("Shelfman"),
            weight = 140
        });

        sceneObject.potentialNPCs.Add(new WeightedNPC
        {
            selection = assetMan.Get<DaSketch>("DaSketch"),
            weight = 140
        });

        sceneObject.potentialNPCs.Add(new WeightedNPC
        {
            selection = assetMan.Get<Testholder>("Testholder"),
            weight = 140
        });

        sceneObject.potentialNPCs.Add(new WeightedNPC
        {
            selection = assetMan.Get<Cren>("Cren"),
            weight = 140
        });

        sceneObject.potentialNPCs.Add(new WeightedNPC
        {
            selection = assetMan.Get<MeMest>("MeMest"),
            weight = 140
        });

        sceneObject.potentialNPCs.Add(new WeightedNPC
        {
            selection = assetMan.Get<TheBestOne>("TheBestOne"),
            weight = (floorName == "F1" || floorName == "F2") ? 5 : 30
        });

        List<string> testWallNames =
        [
            "TestWall", "Testwall2", "TXVHubWall", "TXVHubWall2", "TestWall4", "NoEscape", "TestWall5", "TestWall6", "TestWall7", "TestWall8",
            "TestWall9", "TestWall10", "TestWall11", "TestWall12", "TestHeadWall", "TXVHubWall3"
        ];

        List<string> testFloorNames =
        [
            "TXVHubFloor", "TXVHubFloor2", "TXVHubFloor3", "TestFloor", "TestFloor2", "TestFloor3", "TestFloor4", "TestFloor5", "TestFloor6", "TestFloor7",
            "TestFloor8", "TestFloor9", "TXVHubFloor4"
        ];

        List<string> testCeilingNames = ["TXVHubCeiling", "TXVHubCeiling2", "TestCeiling", "TestCeiling2", "TestCeiling3", "TXVHubCeiling3"];

        List<WeightedTexture2D> testRoomFloorTextures = [];
        List<WeightedTexture2D> testHallFloorTextures = [];
        List<WeightedTexture2D> testWallTextures = [];
        List<WeightedTexture2D> testCeilingTextures = [];

        foreach (string textureName in testWallNames)
        {
            if (textureName != "TestWall7" && textureName != "NoEscape")
            {
                testWallTextures.Add(CreateWeightedTexture(textureName, texturePath));
                continue;
            }

            WeightedTexture2D weightedTexture2D = CreateWeightedTexture(textureName, texturePath);
            weightedTexture2D.weight = 4;
            testWallTextures.Add(weightedTexture2D);
        }

        foreach (string textureName in testFloorNames)
        {
            testRoomFloorTextures.Add(CreateWeightedTexture(textureName, texturePath));
            if (textureName != "TestFloor2")
            {
                testHallFloorTextures.Add(CreateWeightedTexture(textureName, texturePath));
            }
        }

        foreach (string textureName in testCeilingNames)
        {
            testCeilingTextures.Add(CreateWeightedTexture(textureName, texturePath));
        }

        levelObject.hallFloorTexs = ConcatTextures(testHallFloorTextures, levelObject.hallFloorTexs);
        levelObject.hallWallTexs = ConcatTextures(testWallTextures, levelObject.hallWallTexs);
        levelObject.hallCeilingTexs = levelObject.type == LevelType.Factory ? levelObject.hallCeilingTexs : ConcatTextures(testCeilingTextures, levelObject.hallCeilingTexs);

        foreach (RoomGroup roomGroup in levelObject.roomGroup)
        {
            roomGroup.floorTexture = ConcatTextures(testRoomFloorTextures, roomGroup.floorTexture);
            roomGroup.wallTexture = ConcatTextures(testWallTextures, roomGroup.wallTexture);
            roomGroup.ceilingTexture = ConcatTextures(testCeilingTextures, roomGroup.ceilingTexture);
        }

        sceneObject.MarkAsNeverUnload();
    }

    // Asset utils

    public static T[] FindResourceObjects<T>() where T : Object
    {
        List<T> list = [.. from x in Resources.FindObjectsOfTypeAll<T>() where x.GetInstanceID() > 0 select x];
        return [.. list];
    }

    public static T FindResourceOfName<T>(string name, AssetManager assetManager = null) where T : Object
    {
        T[] array = Resources.FindObjectsOfTypeAll<T>();

        foreach (T t in array)
        {
            if (t.name == name) return t;
        }

        if (assetManager != null)
        {
            try
            {
                return assetManager.Get<T>(name);
            }
            catch (Exception)
            {
                throw new NotImplementedException("YOU DONT HAVE THAT YET");
            }
        }

        return default;
    }

    // Creation utils - textures, sounds, and more

    public Sprite CreateSprite(string name, string directory, float offset, float ppu)
    {
        string path = Path.Combine(directory, $"{name}.png");
        Texture2D texture2D = AssetLoader.TextureFromFile(path);
        return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, offset), ppu);
    }

    public SoundObject CreateSound(string name, string directory, SoundMetadata metadata, string Subtitle, string format = "wav")
    {
        string path = Path.Combine(directory, $"{name}.{format}");
        AudioClip audioClip = AssetLoader.AudioClipFromFile(path);
        return ObjectCreators.CreateSoundObject(audioClip, Subtitle, metadata.type, metadata.subcolor, -1f);
    }

    public WeightedTexture2D CreateWeightedTexture(string name, string directory)
    {
        string path = Path.Combine(directory, $"{name}.png");
        Texture2D texture2D = AssetLoader.TextureFromFile(path);
        return new WeightedTexture2D
        {
            selection = texture2D,
            weight = 65
        };
    }

    private WeightedPosterObject CreateWeightedPoster(string name, int weight)
    {
        string directory = Path.Combine(AssetLoader.GetModPath(this), "Textures");
        PosterObject posterObject = ObjectCreators.CreatePosterObject([AssetLoader.TextureFromFile(Path.Combine(directory, $"{name}.png"))]);
        return new WeightedPosterObject
        {
            selection = posterObject,
            weight = weight
        };
    }

    public static SodaMachine MakeSodaMachine(Texture sodaTex, Texture sodaOutTex)
    {
        SodaMachine sodaMachine = Instantiate(FindResourceOfName<GameObject>("SodaMachine", null).GetComponent<SodaMachine>());
        MeshRenderer meshRenderer = sodaMachine.meshRenderer;
        meshRenderer.materials[1].mainTexture = sodaTex;
        sodaMachine.outOfStockMat = new Material(sodaMachine.outOfStockMat)
        {
            mainTexture = sodaOutTex
        };
        sodaMachine.gameObject.ConvertToPrefab(true);
        Instance.Logger.LogInfo(sodaMachine);

        return sodaMachine;
    }

    // Concatenation utils

    private WeightedTexture2D[] ConcatTextures(List<WeightedTexture2D> gaye, WeightedTexture2D[] og) // Props to big thinker for such brilliant parameter names
    {
        List<WeightedTexture2D> list = [];
        for (int i = 0; i < og.Length; i++)
        {
            list.Add(og[i]);
        }
        for (int j = 0; j < gaye.Count; j++)
        {
            list.Add(gaye[j]);
        }
        return [.. list];
    }

    public static T[] ConcatArray<T>(List<T> ge, T[] og) // Big thinker, you're killing me here, and keep it up mind you
    {
        List<T> list = [];
        for (int i = 0; i < og.Length; i++)
        {
            list.Add(og[i]);
        }
        for (int j = 0; j < ge.Count; j++)
        {
            list.Add(ge[j]);
        }
        return [.. list];
    }
}
