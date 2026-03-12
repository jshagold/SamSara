using System;
using System.Reflection;
using Core.System;
using UnityEngine;

namespace App.Systems.Lifecycle
{
    /// <summary>
    /// IAppLifecycleService 구현. 에디터는 플레이 중지, 빌드는 Application.Quit.
    /// 리플렉션으로 UnityEditor 미참조 → Burst 해상 오류 방지.
    /// </summary>
    public class AppLifecycleService : IAppLifecycleService
    {
        public void Quit()
        {
#if UNITY_EDITOR
            var editorApp = Type.GetType("UnityEditor.EditorApplication, UnityEditor");
            if (editorApp != null)
            {
                var prop = editorApp.GetProperty("isPlaying", BindingFlags.Public | BindingFlags.Static);
                prop?.SetValue(null, false);
            }
            else
                Application.Quit();
#else
            Application.Quit();
#endif
        }
    }
}
