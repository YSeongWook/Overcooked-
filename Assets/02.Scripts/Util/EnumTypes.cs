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

    public enum UIEvents
    {
        WorldMapOpen,
        IntroMapOpen,
        BattleRoomOpen,
        TutoMapOpen,
        stage1_4MapOpen,
        stage2_5MapOpen,
        stage3_3MapOpen,
        stageWizardMapOpen,
        stageMineMapOpen,
        TestStageMapOpen,
    }

    public enum SoundEvents
    {
        FadeIn,
        FadeOut
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
    }


    public enum SceneType
    {
        Intro,
        BattleLobby,
        BusMap,
        BattleMap,
        StageMap,
    }

    public enum MapType
    {
        None,
        Tuto,
        stage1_4,
        stage2_5,
        stage3_3,
        stageWizard,
        stageMine,
    }

    public class EnumTypes : MonoBehaviour { }
}