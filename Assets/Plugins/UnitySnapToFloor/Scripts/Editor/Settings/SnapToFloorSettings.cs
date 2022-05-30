using System;
using System.Collections.Generic;
using UnityEngine;

namespace NKStudio.Snap2Floor
{
    public class SnapToFloorSettings : ScriptableObject
    {
        public static List<string> GetLanguageScript(KLanguage kLanguage)
        {
            switch (kLanguage)
            {
                case KLanguage.English:
                    return new List<string>()
                    {
                        "Always",
                        "Never",
                    };
                case KLanguage.한국어:
                    return new List<string>()
                    {
                        "항상",
                        "끄기",
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(kLanguage), kLanguage, null);
            }
        }

        public enum KSnapMode
        {
            Mode3D,
            Mode2D,
        }

        public enum KLanguage
        {
            English,
            한국어,
        }

        public enum KStartAtShow
        {
            Always,
            Never,
        }

        public KSnapMode Mode;

        public KLanguage Language;

        public KStartAtShow StartAtShow;

        private void OnValidate()
        {
            //현재 열려있는 SnapToFloorEditor를 모두 찾습니다.
            var snapToFloorEditors = Resources.FindObjectsOfTypeAll<SnapToFloorEditor>();

            //없으면 리턴
            if (snapToFloorEditors.Length < 1)
                return;

            //세팅 필드를 비웁니다.
            foreach (SnapToFloorEditor snap2Editor in snapToFloorEditors)
            {
                try
                {
                    snap2Editor.LanguageDropDown.value = Language;
                    snap2Editor.ModeDropDown.index = (int)Mode;
                    snap2Editor.StartAtShowDropDown.index = (int)StartAtShow;
                }
                catch (NullReferenceException)
                {
                    
                }
            }
        }
    }
}