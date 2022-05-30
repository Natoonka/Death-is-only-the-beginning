using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NKStudio.Snap2Floor
{
    public class SnapToFloorEditor : EditorWindow
    {
        //uxml절대 경로
        private const string KResourcePath = "Assets/Plugins/UnitySnapToFloor/Scripts/Editor/Window/SnapToFloor.uxml";

        //설정 파일 절대 경로
        private const string KSettingsFileInstallPath = "Assets/Settings/STFAsset.asset";

        //세팅 파일 객체
        private SnapToFloorSettings _settings;

        public ObjectField SettingField;
        public DropdownField ModeDropDown;
        public EnumField LanguageDropDown;
        public DropdownField StartAtShowDropDown;

        [MenuItem("Window/SnapToFloor/settings")]
        public static void Title()
        {
            SnapToFloorEditor wnd = GetWindow<SnapToFloorEditor>();
            wnd.titleContent = new GUIContent("SnapToFloorSetting");
            wnd.minSize = new Vector2(280, 300);
            wnd.maxSize = new Vector2(400, 360);
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            //settings파일의 ID를 가져옵니다.
            string guid = EditorPrefs.GetString("STFSettingsGUID", string.Empty);
            string settingsPath = AssetDatabase.GUIDToAssetPath(guid);

            //파일이 있을 경우 파일 데이터 반영
            if (STFUtillity.HasSettingsFile())
            {
                SnapToFloorSettings instance = AssetDatabase.LoadAssetAtPath<SnapToFloorSettings>(settingsPath);

                //Always이면 true, 아니면 false
                bool showOnStartup = instance.StartAtShow == SnapToFloorSettings.KStartAtShow.Always;

                //항상으로 표시되어 있다면, 켜기
                if (showOnStartup)
                    EditorApplication.update += ShowAtStartup;
            }
            else
                //무조건 켜기
                EditorApplication.update += ShowAtStartup;
        }

        private void OnDestroy() => EditorApplication.update -= ShowAtStartup;

        private static void ShowAtStartup()
        {
            if (!Application.isPlaying)
                Title();

            EditorApplication.update -= ShowAtStartup;
        }

        public void CreateGUI()
        {
            #region 기본 설정

            // 각 편집기 창에는 루트 VisualElement 개체가 포함되어 있습니다.
            VisualElement root = rootVisualElement;

            //UXML 가져오기
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(KResourcePath);
            VisualElement container = visualTree.Instantiate();
            root.Add(container);

            #endregion

            #region DropdownField

            ModeDropDown = root.Q<DropdownField>("mode-DropDown");
            LanguageDropDown = root.Q<EnumField>("language-DropDown");
            StartAtShowDropDown = root.Q<DropdownField>("startAtShow-DropDown");

            #endregion

            #region ObjectField

            SettingField = container.Q<ObjectField>("setting-Filed");
            SettingField.objectType = typeof(SnapToFloorSettings);
            SettingField.allowSceneObjects = false;

            #endregion

            #region Button

            Button applyButton = root.Q<Button>("apply-Button");
            Button createButton = root.Q<Button>("createSettings-Button");

            #endregion

            #region Label

            Label howToUseLabel = root.Q<Label>("howToUse-Text");
            Label descriptionLabel = root.Q<Label>("description-Text");

            #endregion

            //초기화
            InitSetUp();

            //언어 설정
            InitLanguage();

            void InitSetUp()
            {
                //모드-드롭다운에 내용을 추가합니다.
                ModeDropDown.choices = Enum.GetNames(typeof(SnapToFloorSettings.KSnapMode)).ToList();

                //잘 가져왔다면,
                if (STFUtillity.HasSettingsFile())
                {
                    //ID를 가져와서 경로를 구합니다.
                    string guid = EditorPrefs.GetString("STFSettingsGUID", string.Empty);
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    //세팅파일 설정
                    _settings = AssetDatabase.LoadAssetAtPath<SnapToFloorSettings>(assetPath);
                    SettingField.value = _settings;
                    
                    //언어파일 설정
                    LanguageDropDown.Init(_settings.Language);

                    //항상 켜기/끄기 설정
                    StartAtShowDropDown.choices = SnapToFloorSettings.GetLanguageScript(_settings.Language);
                    StartAtShowDropDown.index = (int) _settings.StartAtShow;

                    //모드 설정
                    ModeDropDown.index = (int) _settings.Mode;

                    //기능 활성화
                    LanguageDropDown.SetEnabled(true);
                    StartAtShowDropDown.SetEnabled(true);
                    ModeDropDown.SetEnabled(true);
                    applyButton.SetEnabled(true);
                }
                else
                {
                    //비어있는 것으로 처리
                    EditorPrefs.SetString("STFSettingsGUID", string.Empty);

                    LanguageDropDown.Init(STFUtillity.IsSystemLanguageKorean()
                        ? SnapToFloorSettings.KLanguage.한국어
                        : SnapToFloorSettings.KLanguage.English);

                    //모드 설정
                    ModeDropDown.index = STFUtillity.GetEditorMode();

                    //언어 설정하기
                    SnapToFloorSettings.KLanguage language = STFUtillity.IsSystemLanguageKorean()
                        ? SnapToFloorSettings.KLanguage.한국어
                        : SnapToFloorSettings.KLanguage.English;

                    //언어설정
                    StartAtShowDropDown.choices = SnapToFloorSettings.GetLanguageScript(language);
                    StartAtShowDropDown.index = 0;

                    //기능 비활성화
                    LanguageDropDown.SetEnabled(false);
                    StartAtShowDropDown.SetEnabled(false);
                    ModeDropDown.SetEnabled(false);
                    applyButton.SetEnabled(false);
                }
            }

            void InitLanguage()
            {
                if (STFUtillity.HasSettingsFile())
                {
                    if (_settings.Language == SnapToFloorSettings.KLanguage.한국어)
                        SetKorean();
                    else
                        SetEnglish();
                }
                else
                {
                    if (STFUtillity.IsSystemLanguageKorean())
                        SetKorean();
                    else
                        SetEnglish();
                }

                void SetKorean()
                {
                    applyButton.text = "적용";
                    ModeDropDown.label = "사용하는 모드 ?";
                    howToUseLabel.text = "사용법 ?";
                    descriptionLabel.text = "자신이 사용하는 유니티 모드가 2D/3D에 맞춰 선택하고 적용을 누르세요.";
                    LanguageDropDown.label = "언어";
                    StartAtShowDropDown.label = "시작 시 표시";
                    createButton.text = "생성";

                    ModeDropDown.tooltip = "프로젝트에 맞춰서 모드를 선택합니다." +
                                           "2.5D를 만드는 경우, 2D프로젝트에서 3D를 선택하면 캐릭터는 스프라이트 렌더러를 사용하고 지형은 3D오브젝트를 사용할 수 있습니다.";
                    LanguageDropDown.tooltip = "SnapToFloor의 세팅창에 언어를 변경할 수 있습니다.";
                    StartAtShowDropDown.tooltip = "설정을 완료하면 이 옵션을 '끄기'로 변경하여 컴파일 이후 설정창이 생성되는 것을 끌 수 있습니다.";
                }

                void SetEnglish()
                {
                    applyButton.text = "Apply";
                    ModeDropDown.label = "Use Mode ?";
                    howToUseLabel.text = "How to use ?";
                    descriptionLabel.text = "Check whether the Unity mode is 2D or 3D.";
                    LanguageDropDown.label = "Language";
                    StartAtShowDropDown.label = "Show at StartUp";
                    createButton.text = "Create";

                    ModeDropDown.tooltip = "Choose a mode according to your project." +
                                           "If you're making 2.5D, if you choose 3D in a 2D project, the character will use the sprite renderer and the terrain will use the 3D object.";
                    LanguageDropDown.tooltip = "You can change the language in the settings window of SnapToFloor.";
                    StartAtShowDropDown.tooltip =
                        "After setting, change this option to 'Never' so that it will not be created after compilation.";
                }
            }

            #region CallBack

            createButton.RegisterCallback<MouseUpEvent>(_ =>
            {
                //파일 생성
                CreateSettingsFile();

                //언어 설정 초기화
                InitLanguage();
            });
            ModeDropDown.RegisterValueChangedCallback(_ => ChangeMode());
            StartAtShowDropDown.RegisterValueChangedCallback(_ => ChangeStartAtShow());
            LanguageDropDown.RegisterValueChangedCallback(_ => ChangeLanguage());
            SettingField.RegisterCallback<ChangeEvent<Object>>(SettingsFieldChangeListener);
            applyButton.RegisterCallback<MouseUpEvent>(_ => Apply());

            #endregion

            void CreateSettingsFile()
            {
                #region 파일 가지고 있는지 체크

                //파일을 가지고 있는지 체크
                bool hasSettingsFile = File.Exists(KSettingsFileInstallPath);

                if (hasSettingsFile)
                {
                    //파일을 가지고 있으면 에러를 표시합니다.
                    string message = STFUtillity.IsSystemLanguageKorean()
                        ? "CreateSettingsFile Error : Settings 파일을 가지고 있습니다."
                        : "CreateSettingsFile Error : I have a settings file in my project";

                    //경고
                    Debug.LogWarning(message);
                    return;
                }

                //폴더가 있으면 true, 없으면 false 
                bool hasSettingsDir = Directory.Exists($"{Application.dataPath}/Settings");

                //없으면 생성
                if (!hasSettingsDir)
                    Directory.CreateDirectory($"{Application.dataPath}/Settings");

                #endregion

                #region 파일 생성 & Settings 값 대입

                //인스턴스 생성
                _settings = CreateInstance<SnapToFloorSettings>();

                EditorUtility.SetDirty(_settings);
                
                //에셋 생성
                AssetDatabase.CreateAsset(_settings, KSettingsFileInstallPath);

                //세팅 값 대입
                SettingField.value = _settings;

                #endregion

                #region Settings 값 설정

                //값 설정
                _settings.Mode = (SnapToFloorSettings.KSnapMode) STFUtillity.GetEditorMode();

                //언어 설정
                _settings.Language = STFUtillity.IsSystemLanguageKorean()
                    ? SnapToFloorSettings.KLanguage.한국어
                    : SnapToFloorSettings.KLanguage.English;

                //항상 켜기 설정
                _settings.StartAtShow = SnapToFloorSettings.KStartAtShow.Always;

                #endregion

                #region 기능 활성화

                LanguageDropDown.SetEnabled(true);
                StartAtShowDropDown.SetEnabled(true);
                ModeDropDown.SetEnabled(true);
                applyButton.SetEnabled(true);

                #endregion

                #region GUI 값 대입

                //언어 설정
                LanguageDropDown.Init(_settings.Language);

                //언어 & 모드 초이스 글자 설정
                ModeDropDown.choices = Enum.GetNames(typeof(SnapToFloorSettings.KSnapMode)).ToList();
                StartAtShowDropDown.choices = SnapToFloorSettings.GetLanguageScript(_settings.Language);

                //인덱스 설정
                StartAtShowDropDown.index = (int) _settings.StartAtShow;
                ModeDropDown.index = (int) _settings.Mode;

                #endregion

                #region GUID 저장

                //인스턴스 GUID 저장 
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_settings.GetInstanceID(),
                        out string guid,
                        out long _))
                {
                    //인스턴스ID를 저장합니다.
                    EditorPrefs.SetString("STFSettingsGUID", guid);
                }

                #endregion

                //새로고침
                AssetDatabase.Refresh();
            }

            void ChangeMode()
            {
                //파일이 없으면 에러 표시
                AssertHasSettingsData();

                //모드를 변경합니다.
                _settings.Mode = (SnapToFloorSettings.KSnapMode) ModeDropDown.index;
                
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
            }

            void ChangeLanguage()
            {
                //없다면 에러 표시
                AssertHasSettingsData();

                if (STFUtillity.HasSettingsFile())
                {
                    _settings.Language = (SnapToFloorSettings.KLanguage) LanguageDropDown.value;
                    
                    //항상 켜기/끄기 설정
                    StartAtShowDropDown.choices = SnapToFloorSettings.GetLanguageScript(_settings.Language);
                    StartAtShowDropDown.index = (int) _settings.StartAtShow;
                    
                    if (_settings.Language == SnapToFloorSettings.KLanguage.한국어)
                        SetKorean();
                    else
                        SetEnglish();
                }
                
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
                
                void SetKorean()
                {
                    applyButton.text = "적용";
                    ModeDropDown.label = "사용하는 모드 ?";
                    howToUseLabel.text = "사용법 ?";
                    descriptionLabel.text = "자신이 사용하는 유니티 모드가 2D/3D에 맞춰 선택하고 적용을 누르세요.";
                    LanguageDropDown.label = "언어";
                    StartAtShowDropDown.label = "시작 시 표시";
                    createButton.text = "생성";

                    ModeDropDown.tooltip = "프로젝트에 맞춰서 모드를 선택합니다." +
                                           "2.5D를 만드는 경우, 2D프로젝트에서 3D를 선택하면 캐릭터는 스프라이트 렌더러를 사용하고 지형은 3D오브젝트를 사용할 수 있습니다.";
                    LanguageDropDown.tooltip = "SnapToFloor의 세팅창에 언어를 변경할 수 있습니다.";
                    StartAtShowDropDown.tooltip = "설정을 완료하면 이 옵션을 '끄기'로 변경하여 컴파일 이후 설정창이 생성되는 것을 끌 수 있습니다.";
                }

                void SetEnglish()
                {
                    applyButton.text = "Apply";
                    ModeDropDown.label = "Use Mode ?";
                    howToUseLabel.text = "How to use ?";
                    descriptionLabel.text = "Check whether the Unity mode is 2D or 3D.";
                    LanguageDropDown.label = "Language";
                    StartAtShowDropDown.label = "Show at StartUp";
                    createButton.text = "Create";

                    ModeDropDown.tooltip = "Choose a mode according to your project." +
                                           "If you're making 2.5D, if you choose 3D in a 2D project, the character will use the sprite renderer and the terrain will use the 3D object.";
                    LanguageDropDown.tooltip = "You can change the language in the settings window of SnapToFloor.";
                    StartAtShowDropDown.tooltip =
                        "After setting, change this option to 'Never' so that it will not be created after compilation.";
                }
            }

            void ChangeStartAtShow()
            {
                //파일이 없으면 에러 표기
                AssertHasSettingsData();

                //값을 적용
                _settings.StartAtShow = (SnapToFloorSettings.KStartAtShow) StartAtShowDropDown.index;
                
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
            }

            void SettingsFieldChangeListener(ChangeEvent<Object> changeEvent)
            {
                //파일을 가지고 있는지 체크
                bool hasSettingsFile = changeEvent.newValue != null;

                //따로 빼버렸다면 ID저장을 뺀다
                if (hasSettingsFile)
                {
                    //세팅 값을 넣습니다.
                    _settings = (SnapToFloorSettings) changeEvent.newValue;
                    SettingField.value = _settings;

                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_settings.GetInstanceID(),
                            out string guid,
                            out long _))
                    {
                        //인스턴스ID를 저장합니다.
                        EditorPrefs.SetString("STFSettingsGUID", guid);
                    }
                }
                else
                {
                    //인스턴스 ID를 제거합니다.
                    EditorPrefs.DeleteKey("STFSettingsGUID");
                    SettingField.value = null;

                    //기능 비활성화
                    LanguageDropDown.SetEnabled(false);
                    StartAtShowDropDown.SetEnabled(false);
                    ModeDropDown.SetEnabled(false);
                    applyButton.SetEnabled(false);
                }

                //초기화 합니다.
                InitSetUp();

                if (!hasSettingsFile)
                    STFUtillity.RemoveDefine();
            }

            void Apply()
            {
                //파일이 없으면 에러 표시
                AssertHasSettingsData();

                STFUtillity.AddDefine(_settings.Mode, _settings.Language);
            }

            bool HasSettingsData() => STFUtillity.HasSettingsFile() && SettingField.value != null && _settings != null;

            void AssertHasSettingsData()
            {
                string msg = STFUtillity.IsSystemLanguageKorean()
                    ? "Settings 필드가 Null입니다."
                    : "the settings field is null";

                //파일이 없으면 에러 표기
                Assert.IsTrue(HasSettingsData(), msg);
            }
        }
    }
}