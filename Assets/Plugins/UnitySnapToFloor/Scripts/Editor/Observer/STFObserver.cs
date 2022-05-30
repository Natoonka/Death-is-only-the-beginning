using System.IO;
using UnityEditor;
using UnityEngine;

namespace NKStudio.Snap2Floor
{
    public class STFObserver : AssetPostprocessor
    {
        private const string KSettingsFileInstallPath = "Assets/Settings/STFAsset.asset";

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            //변경된 파일이 없다면 리턴
            if (deletedAssets.Length <= 0) return;
            
            bool hasSettingFile = File.Exists(KSettingsFileInstallPath);

            if (hasSettingFile) return;
            
            //현재 열려있는 SnapToFloorEditor를 모두 찾습니다.
            var snapToFloorEditors = Resources.FindObjectsOfTypeAll<SnapToFloorEditor>();
            
            //세팅 필드를 비웁니다.
            foreach (SnapToFloorEditor snapToFloorEditor in snapToFloorEditors)
                snapToFloorEditor.SettingField.value = null;
        }
    }
}