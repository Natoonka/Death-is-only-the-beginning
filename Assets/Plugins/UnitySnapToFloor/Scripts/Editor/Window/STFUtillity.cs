using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKStudio.Snap2Floor
{
    public class STFUtillity 
    {
        /// <summary>
        /// 파일이 있는지 체크합니다.
        /// </summary>
        /// <returns></returns>
        public static bool HasSettingsFile()
        {
            string settingsGuid = EditorPrefs.GetString("STFSettingsGUID", string.Empty);

            if (string.IsNullOrEmpty(settingsGuid))
            {
                EditorPrefs.SetString("STFSettingsGUID", string.Empty);
                return false;
            }

            string settingsPath = AssetDatabase.GUIDToAssetPath(settingsGuid);
            if (string.IsNullOrEmpty(settingsPath))
            {
                EditorPrefs.SetString("STFSettingsGUID", string.Empty);
                return false;
            }
            
            bool hasFile = File.Exists(settingsPath);
            
            if (!hasFile) 
                EditorPrefs.SetString("STFSettingsGUID", string.Empty);

            return hasFile;
        }
    
        /// <summary>
        /// 시스템 언어가 한국어이면 true, 아니면 false을 반환한다.
        /// </summary>
        /// <returns></returns>
        public static bool IsSystemLanguageKorean()
        {
            return Application.systemLanguage == SystemLanguage.Korean;
        }

        /// <summary>
        /// 3D모드이면 0, 2D이면 1을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public static int GetEditorMode()
        {
            return (int) EditorSettings.defaultBehaviorMode;
        }

        /// <summary>
        /// 디파인을 추가합니다.
        /// </summary>
        /// <param name="modeIndex"></param>
        /// <param name="language"></param>
        public static void AddDefine(SnapToFloorSettings.KSnapMode modeIndex, SnapToFloorSettings.KLanguage language)
        {
            //현재 선택된 빌트 타겟에 처리합니다.
            var defines = PlayerSettings
                .GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';')
                .ToList();

            switch (modeIndex)
            {
                case SnapToFloorSettings.KSnapMode.Mode3D:

                    if (defines.Contains("SNAP2FLOOR_3D"))
                    {
                        Debug.LogWarning(language == SnapToFloorSettings.KLanguage.English
                            ? "It has already been applied."
                            : "이미 적용이 완료되었습니다.");
                        return;
                    }

                    defines.Add("SNAP2FLOOR_3D");
                    defines.Remove("SNAP2FLOOR_2D");
                    break;
                case SnapToFloorSettings.KSnapMode.Mode2D:
                    if (defines.Contains("SNAP2FLOOR_2D"))
                    {
                        Debug.LogWarning(language == SnapToFloorSettings.KLanguage.English
                            ? "It has already been applied."
                            : "이미 적용이 완료되었습니다.");
                        return;
                    }

                    defines.Add("SNAP2FLOOR_2D");
                    defines.Remove("SNAP2FLOOR_3D");
                    break;
            }

            //디파인 중복 제거
            defines = defines.Distinct().ToList();

            //문자열 다시 합친후 심볼(디파인) 적용 
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", defines.ToArray()));
        }
        
        /// <summary>
        /// 디파인을 제거합니다.
        /// </summary>
        public static void RemoveDefine()
        {
            //현재 선택된 빌트 타겟에 처리합니다.
            var defines = PlayerSettings
                .GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';')
                .ToList();

            //디파인에 없었다면,
            if (!(defines.Contains("SNAP2FLOOR_2D") || defines.Contains("SNAP2FLOOR_3D")))
                return;

            //제거
            defines.Remove("SNAP2FLOOR_2D");
            defines.Remove("SNAP2FLOOR_3D");

            //디파인 중복 제거
            defines = defines.Distinct().ToList();

            //문자열 다시 합친후 심볼(디파인) 적용 
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", defines.ToArray()));
        }
    }
}
