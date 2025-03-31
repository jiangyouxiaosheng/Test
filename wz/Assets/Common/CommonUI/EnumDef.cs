using Sirenix.OdinInspector;
using System.Collections.Generic;
using OPS.Obfuscator.Attribute;

[DoNotRename]
public enum EUIPageType : int
{
    Void = -1,
    None = 0,
    TestUI = 1,
    HowPlayPage = 2,
    GameOverPage = 3,
    GameWinPage =4,
    NoThanksPage = 5,
    BattlePage = 6,
    WaitMaskPage = 7,
    PopWindowPage = 8,
    TlWindowPage =9,
}


// -------- Comparer ---------
#region Enum Comparer

public class UIPageTypeComparer : IEqualityComparer<EUIPageType>
{
    public bool Equals(EUIPageType x, EUIPageType y)
    {
        return GetHashCode(x) == GetHashCode(y);
    }

    public int GetHashCode(EUIPageType obj)
    {
        return (int)obj;
    }
}

#endregion

public enum EPlayerDataLoadType
{
    Local,
    HttpServer,
}

public enum EGameModeType : int
{
    None,
    Loading,
    MainMenu,
    GamePlay,
}

public enum EGameModeState : int
{
    // --- Common --- from 0 to 99
    None = 0,
    Init,
    Pause,
    Fail,
    // --- Launch ---
    LaunchInit = 100,
    LaunchUpdateCatalog,
    LaunchUpdateAsset,
    LaunchLoginServer,
    LaunchLoadPlayerData,
    LaunchEnterGame,
    // --- Main --- from 200 to 299

    /// <summary>
    /// 主流程初始化
    /// </summary>
    GameMainInit = 200,
    /// <summary>
    /// 怪物阶段战斗
    /// </summary>
    GameMainBattle,
    /// <summary>
    /// 循环刷怪阶段战斗
    /// </summary>
    GameMainCyclicBattle,
    /// <summary>
    /// Boss挑战战斗
    /// </summary>
    GameMainBossBattle,
    /// <summary>
    /// 切换关卡
    /// </summary>
    GameMainChangeLevel,
    /// <summary>
    /// 钻石关
    /// </summary>
    GameMainDiamondDungeon,
    /// <summary>
    /// 金币关
    /// </summary>
    GameMainGoldDungeon,
    // --- Develop --- from 900 to 999
    DevelopInit = 900,
    DevelopSkillEditor,

}

public enum EPlayerPropertyType
{

}

public enum EMainBattleState : int
{
    None = 0,
    BattleWin,
    BattleFail,
}

public enum EUISoundEffectEvent
{
    [LabelText("展示时")]
    OnEnable,
    [LabelText("点击时")]
    OnClick,
}

#region Skill

public enum EBulletCollisionType
{
    /// <summary>
    /// 贯穿子弹
    /// </summary>
    Normal,
    /// <summary>
    /// 碰撞消失
    /// </summary>
    StopOnCollision,
}

public enum ESkillActionType
{
    None,
    Animation,
    Damage,
    Sound,
    Gun,
    SpawnObject,
    Buff,
    CameraShake,
    SpawnSummoned,
    ForceMove,
    Movement,
}

public enum EShape
{
    /// <summary>
    /// 方形
    /// </summary>
    [LabelText("方形")]
    Box,

    /// <summary>
    /// 圆形
    /// </summary>
    [LabelText("圆形")]
    Circle,

    /// <summary>
    /// 全屏
    /// </summary>
    [LabelText("全屏")]
    All,
}

public enum ESpawnObjectType
{
    Object,
    Summoned,
}

public enum ESpawnObjectSpace
{
    [LabelText("挂在自己身上")]
    HookOnOwner,
    [LabelText("敌人")]
    Enemy,
    [LabelText("当前屏幕")]
    ViewSpace,
}

public enum EViewspaceType
{
    [LabelText("绝对位置")]
    FixedSpace = 0,
    [LabelText("最多的敌人区域")]
    MostTarget = 1,
}

public enum EForceMoveType
{
    None,
    [LabelText("推")]
    Push,
    [LabelText("拉")]
    Pull,
}

public enum EMovementActionType
{
    [LabelText("不移动")]
    None,
    [LabelText("延方向移动")]
    MoveByDirection,
    [LabelText("移动到最多目标")]
    MoveToMostEnemy,
    [LabelText("移动到最近目标")]
    MoveToNearestEnemy,
}

public enum ESummonedMovementType
{
    [LabelText("不移动")]
    None,
    [LabelText("延方向移动")]
    MoveDirection,
    [LabelText("追踪目标")]
    TrackTarget,
}

#endregion

public enum EPawnType
{
    None,
    Player,
    Partner,
    Monster,
    Boss,
    Summoned,
}

public enum EMonsterState
{
    Idle,
    Vigilance,
    Hurt,
    Dead,
    Escape,
}

/// <summary>
/// 小怪类型，主要与掉落，AI相关
/// </summary>
public enum EMonsterType
{
    [LabelText("普通怪")]
    Normal,
    [LabelText("钻石关Boss")]
    DiamondBoss,
    [LabelText("金币关Boss")]
    GoldenBoss,
}

public enum EJewelryType
{
    Ring,
    Necklace,
}

public enum EQuality
{
    E = 1,
    D = 2,
    C = 3,
    B = 4,
    A = 5,
    S = 6,
    SS = 7
}


public enum EStarColor
{
    Gray = 0,
    Green = 1,
    Blue = 2,
    Purple = 3,
    Orange = 4
}

//
// 101	Item_101	1	1	金币	强化模块的升级材料
// 102	Item_102	5	1	钻石	可用于商城召唤各种道具等
// 103	Item_103	6	2	水晶	武器的升级材料
// 104	Item_104	6	2	神石	武器的升星材料
// 105	Item_105	3	3	蓝鱼干	普通天赋的升级材料
// 106	Item_106	4	14	紫鱼干	高级天赋的升级材料
// 107	Item_107	4	5	贝壳	藏品的发现/升级材料
// 108	Item_108	1	6	珍珠	契约模块刷新词条的材料
// 109	Item_109	1	15	1小时离线金币收益	立即获得1小时离线金币收益
// 110	Item_110	1	15	深海海域钥匙	用于进入深海海域副本（钻石副本）
// 111	Item_111	1	15	宝藏海域钥匙	用于进入宝藏海域副本（金币副本）
// 112	Item_112	1	15	漩涡海域钥匙	用于进入漩涡海域副本（贝壳副本）
// 113	Item_113	1	15	无尽海域钥匙	用于进入无尽海域副本（珍珠副本）
// 114	Item_114	1	4;13	铁镐	用于海底，消除选中地块的1点耐力
// 115	Item_115	3	13	钻头	用于海底，消除选中所在列，及相邻底部的地块
// 116	Item_116	3	13	炸弹	用于海底，消除选中选中地块及其上下左右的2个地块
// 117	Item_117	4	4;12	研究石	研究所模块的升级材料
// 118	Item_118	6	15	免广告券	免除广告


public enum ECurrencyNameId
{
    Gold = 101,
    Diamond = 102,
    Crystal = 103,
    GodStone = 104,
    BlueFish = 105,
    PurpleFish = 106,
    Shell = 107,
    Pearl = 108,
    HourGold = 109,
    DeepSeaKey = 110,
    TreasureSeaKey = 111,
    VortexSeaKey = 112,
    EndlessSeaKey = 113,
    Pickaxe = 114,
    Drill = 115,
    Bomb = 116,
    ResearchStone = 117,
    NoAd = 118,
    Indenture = 120,
}


// 1	挑战主线关卡到达某个进度
// 2	击溃X个敌人
// 3	贝壳副本通关到X层
// 4	钻石副本通关到X层
// 5	金币副本通关到X层
// 6	累计召唤X次装备
// 7	累计召唤X次技能
// 8	累计召唤X次宠物
// 9	强化攻击力到X级
// 10	强化生命到X级
// 11	强化攻击速度到X级
// 12	强化暴击伤害到X级
// 13	强化生命恢复到X级
// 14	强化暴击率到X级
// 15	强化连击到X级
// 16	强化三连击到X级
public enum EMainTaskType
{
    MainLevel = 1,

    KillMonster = 2,

    ShellDungeon = 3,

    DiamondDungeon = 4,

    GoldDungeon = 5,

    SummonEquip = 6,

    SummonSkill = 7,

    SummonPet = 8,

    StrengthenAttack = 9,

    StrengthenHp = 10,

    StrengthenAttackSpeed = 11,

    StrengthenCritDamage = 12,

    StrengthenHpRecover = 13,

    StrengthenCritRate = 14,

    StrengthenCombo = 15,

    StrengthenTripleCombo = 16,
}


// 1	击溃X个敌人
// 2	完成X个主线关卡
// 3	完成X次深海海域
// 4	完成X次宝藏海域
// 5	召唤X次技能/伙伴/装备
// 6	观看X次广告
// 7	完成X个每日任务
public enum EDailyTaskType
{
    KillMonster = 1,

    MainLevel = 2,

    DeepSea = 3,

    TreasureSea = 4,

    Summon = 5,

    WatchAd = 6,

    DailyTask = 7,
}

public enum ERewardDisplayType
{
    None,//不展示
    Gift, //礼包形式展示
    Lottery, //抽奖形式展示
}

public enum ESummonType
{
    Equip = 1,
    Skill = 2,
    Mate = 3,
}

public enum EAdBuff
{
    Gold = 1,
    SkillTime = 2,
    Atk = 3,
}
// -------- Comparer ---------
#region Enum Comparer

public class GameModeTypeComparer : IEqualityComparer<EGameModeType>
{
    public bool Equals(EGameModeType x, EGameModeType y)
    {
        return GetHashCode(x) == GetHashCode(y);
    }

    public int GetHashCode(EGameModeType obj)
    {
        return (int)obj;
    }
}

public class GameModeStateComparer : IEqualityComparer<EGameModeState>
{
    public bool Equals(EGameModeState x, EGameModeState y)
    {
        return GetHashCode(x) == GetHashCode(y);
    }

    public int GetHashCode(EGameModeState obj)
    {
        return (int)obj;
    }
}


public class SkillActionTypeComparer : IEqualityComparer<ESkillActionType>
{
    public bool Equals(ESkillActionType x, ESkillActionType y)
    {
        return GetHashCode(x) == GetHashCode(y);
    }

    public int GetHashCode(ESkillActionType obj)
    {
        return (int)obj;
    }
}



public class WeaponStarTypeComparer : IEqualityComparer<EStarColor>
{
    public bool Equals(EStarColor x, EStarColor y)
    {
        return GetHashCode(x) == GetHashCode(y);
    }

    public int GetHashCode(EStarColor obj)
    {
        return (int)obj;
    }
}
#endregion

// -------- 红点 ---------
public enum E_RedPoint
{
    None,
    Role_Root,

    AirplaneBag,        //新飞机
    AirplaneBag_List,

    Collection,         //图鉴
    Collection_List,

    Talent,             //天赋
    Talent_List,
}

public enum ERedPointState
{
    None,
    Show,
    Hide,
}

public enum EIndentureType
{
    Light = 1,
    Dark = 2,
}