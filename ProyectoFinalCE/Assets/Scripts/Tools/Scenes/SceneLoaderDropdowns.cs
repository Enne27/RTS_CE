using UnityEditor;
public partial class SceneLoader
{
#if UNITY_EDITOR
        [MenuItem("Scenes/SampleScene")]
        public static void LoadSampleScene() { OpenScene("Assets/scenes/SampleScene.unity"); }
        [MenuItem("Scenes/FinalScenes/CreativeModeGameScene")]
        public static void LoadCreativeModeGameScene() { OpenScene("Assets/scenes/FinalScenes/CreativeModeGameScene.unity"); }
        [MenuItem("Scenes/FinalScenes/CreditsScene")]
        public static void LoadCreditsScene() { OpenScene("Assets/scenes/FinalScenes/CreditsScene.unity"); }
        [MenuItem("Scenes/FinalScenes/MainMenuScene")]
        public static void LoadMainMenuScene() { OpenScene("Assets/scenes/FinalScenes/MainMenuScene.unity"); }
        [MenuItem("Scenes/FinalScenes/SingleplayerGameScene UI")]
        public static void LoadSingleplayerGameSceneUI() { OpenScene("Assets/scenes/FinalScenes/SingleplayerGameScene UI.unity"); }
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
        [MenuItem("Scenes/TestScenes/IA_Troop_Testing")]
        public static void LoadIA_Troop_Testing() { OpenScene("Assets/scenes/TestScenes/IA_Troop_Testing.unity"); }
        [MenuItem("Scenes/TestScenes/Map_Test")]
        public static void LoadMap_Test() { OpenScene("Assets/scenes/TestScenes/Map_Test.unity"); }
        [MenuItem("Scenes/TestScenes/Settings_Scenes")]
        public static void LoadSettings_Scenes() { OpenScene("Assets/scenes/TestScenes/Settings_Scenes.unity"); }
        [MenuItem("Scenes/TestScenes/SingleplayerGameSceneTEST")]
        public static void LoadSingleplayerGameSceneTEST() { OpenScene("Assets/scenes/TestScenes/SingleplayerGameSceneTEST.unity"); }
        [MenuItem("Scenes/TestScenes/SingleplayerGameSceneTEST_NAIARA")]
        public static void LoadSingleplayerGameSceneTEST_NAIARA() { OpenScene("Assets/scenes/TestScenes/SingleplayerGameSceneTEST_NAIARA.unity"); }
        [MenuItem("Scenes/TestScenes/Test_Skills_UI")]
        public static void LoadTest_Skills_UI() { OpenScene("Assets/scenes/TestScenes/Test_Skills_UI.unity"); }
        [MenuItem("Scenes/TestScenes/UI_Test")]
        public static void LoadUI_Test() { OpenScene("Assets/scenes/TestScenes/UI_Test.unity"); }
        [MenuItem("Scenes/TestScenes/War_Test")]
        public static void LoadWar_Test() { OpenScene("Assets/scenes/TestScenes/War_Test.unity"); }
        [MenuItem("Scenes/TestScenes/ZonaRecursos_Test")]
        public static void LoadZonaRecursos_Test() { OpenScene("Assets/scenes/TestScenes/ZonaRecursos_Test.unity"); }
        [MenuItem("Scenes/TestScenes/Art/DavidReyes")]
        public static void LoadDavidReyes() { OpenScene("Assets/scenes/TestScenes/Art/DavidReyes.unity"); }
        [MenuItem("Scenes/TestScenes/Art/PropsMap")]
        public static void LoadPropsMap() { OpenScene("Assets/scenes/TestScenes/Art/PropsMap.unity"); }
        [MenuItem("Scenes/TestScenes/PersonalTestScenes/BuildingsChambers_Test")]
        public static void LoadBuildingsChambers_Test() { OpenScene("Assets/scenes/TestScenes/PersonalTestScenes/BuildingsChambers_Test.unity"); }
        [MenuItem("Scenes/TestScenes/PersonalTestScenes/FogOfWar")]
        public static void LoadFogOfWar() { OpenScene("Assets/scenes/TestScenes/PersonalTestScenes/FogOfWar.unity"); }
        [MenuItem("Scenes/TestScenes/PersonalTestScenes/QualityConfigTestView")]
        public static void LoadQualityConfigTestView() { OpenScene("Assets/scenes/TestScenes/PersonalTestScenes/QualityConfigTestView.unity"); }
        [MenuItem("Scenes/TestScenes/PersonalTestScenes/SingleplayerGameSceneGUILLEMTEST")]
        public static void LoadSingleplayerGameSceneGUILLEMTEST() { OpenScene("Assets/scenes/TestScenes/PersonalTestScenes/SingleplayerGameSceneGUILLEMTEST.unity"); }
        [MenuItem("Scenes/TestScenes/PersonalTestScenes/Anuk Temp/Fog")]
        public static void LoadFog() { OpenScene("Assets/scenes/TestScenes/PersonalTestScenes/Anuk Temp/Fog.unity"); }
        [MenuItem("Scenes/TestScenes/PersonalTestScenes/Anuk Temp/SingleplayerGameSceneTemp")]
        public static void LoadSingleplayerGameSceneTemp() { OpenScene("Assets/scenes/TestScenes/PersonalTestScenes/Anuk Temp/SingleplayerGameSceneTemp.unity"); }
#endif
}