using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Game.UI.Controllers.SettingsMenu
{
    public class VideoSettingsPanelController : BaseSettingsPanel
    {
        [Header("UI References")]
        [SerializeField] private Toggle fullscreenToggle;
        // [SerializeField] private TMP_Dropdown resolutionDropdown;
        // [SerializeField] private TMP_Dropdown qualityDropdown;

        // private List<Resolution> _resolutions = new();

        private bool _originalFullscreen;
        // private int _originalResolutionIndex;
        // private int _originalQualityIndex;

        // private void Awake()
        // {
        //     PopulateResolutionsDropdown();

        //     PopulateQualityDropdown();
        // }

        private void OnEnable()
        {
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
            // resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            // qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }

        private void OnDisable()
        {
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenChanged);
            // resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
            // qualityDropdown.onValueChanged.RemoveListener(OnQualityChanged);
        }

        // private void PopulateResolutionsDropdown()
        // {
        //     resolutionDropdown.ClearOptions();
        //     _resolutions.Clear();

        //     var uniqueResolutions = Screen.resolutions
        //         .GroupBy(r => new { r.width, r.height })
        //         .Select(group => group.First())
        //         .ToList();

        //     _resolutions.AddRange(uniqueResolutions);

        //     var options = new List<TMP_Dropdown.OptionData>();
        //     foreach (var resolution in _resolutions)
        //         options.Add(new TMP_Dropdown.OptionData($"{resolution.width} x {resolution.height}"));

        //     resolutionDropdown.AddOptions(options);
        // }

        // private void PopulateQualityDropdown()
        // {
        //     qualityDropdown.ClearOptions();

        //     string[] qualityNames = QualitySettings.names;

        //     var options = new List<TMP_Dropdown.OptionData>();
        //     foreach (var name in qualityNames)
        //         options.Add(new TMP_Dropdown.OptionData(name));

        //     qualityDropdown.AddOptions(options);
        // }

        public override void LoadSettings()
        {
            _originalFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            fullscreenToggle.SetIsOnWithoutNotify(_originalFullscreen);

            // int currentWidth = Screen.width;
            // int currentHeight = Screen.height;
            // _originalResolutionIndex = 0;

            // for (int i = 0; i < _resolutions.Count; i++)
            // {
            //     if (_resolutions[i].width == currentWidth && _resolutions[i].height == currentHeight)
            //     {
            //         _originalResolutionIndex = i;
            //         break;
            //     }
            // }

            // resolutionDropdown.SetValueWithoutNotify(_originalResolutionIndex);

            // _originalQualityIndex = QualitySettings.GetQualityLevel();
            // qualityDropdown.SetValueWithoutNotify(_originalQualityIndex);

            ResetChangeTracking();
        }

        public override void DiscardChanges()
        {
            fullscreenToggle.SetIsOnWithoutNotify(_originalFullscreen);
            // resolutionDropdown.SetValueWithoutNotify(_originalResolutionIndex);
            // qualityDropdown.SetValueWithoutNotify(_originalQualityIndex);

            ResetChangeTracking();
        }

        public override void ResetToDefaults()
        {
            fullscreenToggle.SetIsOnWithoutNotify(true);

            // int optimalIndex = _resolutions.Count - 1;
            // resolutionDropdown.SetValueWithoutNotify(optimalIndex);

            // int highQualityIndex = Mathf.Clamp(3, 0, QualitySettings.names.Length - 1);
            // qualityDropdown.SetValueWithoutNotify(highQualityIndex);

            CheckForChanges();
        }

        public override bool HasChanges()
        {
            return _hasChanges ||
                   fullscreenToggle.isOn != _originalFullscreen;
            //    resolutionDropdown.value != _originalResolutionIndex ||
            //    qualityDropdown.value != _originalQualityIndex;
        }

        private void CheckForChanges()
        {
            bool hasChanges = HasChanges();
            if (hasChanges != _hasChanges)
            {
                _hasChanges = hasChanges;
                NotifySettingsChanged();
            }
        }

        private void OnFullscreenChanged(bool isFullscreen)
        {
            CheckForChanges();
        }

        private void OnResolutionChanged(int index)
        {
            CheckForChanges();
        }

        private void OnQualityChanged(int index)
        {
            CheckForChanges();
        }


        public override void ApplyChanges()
        {
            // if (resolutionDropdown.value >= 0 && resolutionDropdown.value < _resolutions.Count)
            // {
            //     Resolution selectedResolution = _resolutions[resolutionDropdown.value];
            //     Screen.SetResolution(
            //         selectedResolution.width,
            //         selectedResolution.height,
            //         fullscreenToggle.isOn
            //     );
            // }

            Screen.fullScreen = fullscreenToggle.isOn;
            // QualitySettings.SetQualityLevel(qualityDropdown.value);


            PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
            // PlayerPrefs.SetInt("ResolutionWidth", Screen.width);
            // PlayerPrefs.SetInt("ResolutionHeight", Screen.height);
            // PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);

            PlayerPrefs.Save();

            ResetChangeTracking();
        }
    }
}