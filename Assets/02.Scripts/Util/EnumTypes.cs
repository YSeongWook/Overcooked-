using UnityEngine;

namespace EnumTypes
{
    public enum LookDirection { Up, Down, Left, Right }
    public enum PlayerState { Stand, Move, Run, Hold, Cook }
    public enum Layers
    {
        Default,
        TransparentFX,
        IgnoreRaycast,
        Reserved1,
        Water,
        UI,
        Reserved2,
        Reserved3,
        Player,
        World,
    }

    public enum ThrowableType
    {
        Ingredient,
    };

    public enum GlobalEvents
    {
        PlayerDead,
        PlayerSpawned,
        PlayerInactive,
        PlayerDamaged,
    }

    public enum SceneChangeEvent
    {
        WorldMapOpen,
        IntroMapOpen,
        BattleRoomOpen,
        TutoMapOpen,
        Stage1_4MapOpen,
        Stage2_5MapOpen,
        Stage3_3MapOpen,
        StageWizardMapOpen,
        StageMineMapOpen,
        TestStageMapOpen,
    }

    public enum SoundEvents
    {
        FadeIn,
        FadeOut,
        MineBgmPlay,
    }

    public enum UIEvents
    {
        EscUI,
        MobilePlayerController,
    }

    public enum UIType
    {
        OptionUI,
        StopUI,
        BattleUI,
        ExitLobbyUI,
        LoadingKeyUI,
        LoadingMapUI,
        LoadingFoodUI,
        BusMap,
        StageMapEscUI,
        RecipeUI,
        EscUI,
    }

    public enum GameEvent
    {
        FireOff,
    }


    public enum SceneType
    {
        Intro,
        BattleLobby,
        WorldMap,
        BattleMap,
        StageMap,
        // Canvas분기 처리를 위한 추가
        NetworkBattleMap,
    }

    public enum MapType
    {
        None,
        Tuto,
        Stage1_4,
        Stage2_5,
        Stage3_3,
        StageWizard,
        StageMine,
    }

    public class EnumTypes : MonoBehaviour { }
}