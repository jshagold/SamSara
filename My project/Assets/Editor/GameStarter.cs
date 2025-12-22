using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameStarter
{
    // 단축키 설정: F5 키로 설정함 (_F5)
    // % = Ctrl (Mac은 Cmd), # = Shift, & = Alt
    // 예: "%#r" = Ctrl + Shift + R
    [MenuItem("Tools/Play From Intro Scene _F5")]
    public static void PlayFromIntroScene()
    {
        // 1. 현재 작업 중인 씬이 수정되었다면 저장할지 물어봄 (날라가면 안되니까)
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            // 2. Build Settings에 등록된 0번째 씬(주로 Intro)의 경로를 가져옴
            // (만약 특정 이름을 쓰고 싶다면 path 대신 "Assets/Scenes/IntroScene.unity" 직접 입력)
            string firstScenePath = EditorBuildSettings.scenes[0].path;

            if (!string.IsNullOrEmpty(firstScenePath))
            {
                // 3. 씬 열기
                EditorSceneManager.OpenScene(firstScenePath);

                // 4. 플레이 모드 진입
                EditorApplication.isPlaying = true;
            }
            else
            {
                Debug.LogError("Build Settings에 씬이 등록되지 않았습니다! File -> Build Settings에서 IntroScene을 0번에 등록해주세요.");
            }
        }
    }
}
