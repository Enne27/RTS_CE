using UnityEditor;
public partial class SceneLoader
{
#if UNITY_EDITOR
        [MenuItem("Scenes/FinalScenes/CreativeModeGameScene")]
        public static void LoadCreativeModeGameScene() { OpenScene("Assets/scenes/FinalScenes/CreativeModeGameScene.unity"); }
        [MenuItem("Scenes/FinalScenes/CreditsScene")]
        public static void LoadCreditsScene() { OpenScene("Assets/scenes/FinalScenes/CreditsScene.unity"); }
        [MenuItem("Scenes/FinalScenes/MainMenuScene")]
        public static void LoadMainMenuScene() { OpenScene("Assets/scenes/FinalScenes/MainMenuScene.unity"); }
        [MenuItem("Scenes/FinalScenes/SingleplayerGameScene")]
        public static void LoadSingleplayerGameScene() { OpenScene("Assets/scenes/FinalScenes/SingleplayerGameScene.unity"); }
        [MenuItem("Scenes/TestScenes/Assets_3D_Test")]
        public static void LoadAssets_3D_Test() { OpenScene("Assets/scenes/TestScenes/Assets_3D_Test.unity"); }
        [MenuItem("Scenes/TestScenes/Buildings_Test")]
        public static void LoadBuildings_Test() { OpenScene("Assets/scenes/TestScenes/Buildings_Test.unity"); }
        [MenuItem("Scenes/TestScenes/Enemy_IA_Test")]
        public static void LoadEnemy_IA_Test() { OpenScene("Assets/scenes/TestScenes/Enemy_IA_Test.unity"); }
        [MenuItem("Scenes/TestScenes/General_Mecanics_Test")]
        public static void LoadGeneral_Mecanics_Test() { OpenScene("Assets/scenes/TestScenes/General_Mecanics_Test.unity"); }
        [MenuItem("Scenes/TestScenes/IA_Troops_Test")]
        public static void LoadIA_Troops_Test() { OpenScene("Assets/scenes/TestScenes/IA_Troops_Test.unity"); }
        [MenuItem("Scenes/TestScenes/Map_Test")]
        public static void LoadMap_Test() { OpenScene("Assets/scenes/TestScenes/Map_Test.unity"); }
        [MenuItem("Scenes/TestScenes/UI_Test")]
        public static void LoadUI_Test() { OpenScene("Assets/scenes/TestScenes/UI_Test.unity"); }
        [MenuItem("Scenes/TestScenes/War_Test")]
        public static void LoadWar_Test() { OpenScene("Assets/scenes/TestScenes/War_Test.unity"); }
#endif
}