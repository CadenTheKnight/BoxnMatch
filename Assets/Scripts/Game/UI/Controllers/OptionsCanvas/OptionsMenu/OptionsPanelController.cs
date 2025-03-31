using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Game.UI.Controllers.OptionsCanvas.SettingsMenu;

namespace Assets.Scripts.Game.UI.Controllers.OptionsCanvas.OptionsMenu
{
    public class OptionsPanelController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private RectTransform shadowRectTransform;
        [SerializeField] private RectTransform sidePanelRectTransform;
        [SerializeField] private Button profileButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Image profilePictureImage;
        [SerializeField] private Image settingsImage;
        [SerializeField] private Image quitImage;
        [SerializeField] private GameObject profileInfoGroup;
        [SerializeField] private TextMeshProUGUI profileNameText;
        [SerializeField] private TextMeshProUGUI profileLevelText;
        [SerializeField] private ProgressBar profileLevelProgressBar;
        [SerializeField] private TextMeshProUGUI settingsText;
        [SerializeField] private TextMeshProUGUI quitText;

        [Header("Animation Settings")]
        [SerializeField] private float collapsedWidth = 0.06f;
        [SerializeField] private float animationDuration = 0.2f;
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("References")]
        [SerializeField] private SettingsPanelController settingsPanelController;

        private Coroutine animationCoroutine;
        private bool isExpanded = false;
        private bool isAnimating = false;

        private void Awake()
        {
            SetCollapsed();

            profileNameText.text = AuthenticationManager.Instance.LocalPlayer.Profile.Name;
            profileLevelText.text = "Level " + AuthenticationManager.Instance.LocalPlayer.Data["Level"].Value;
            profileLevelProgressBar.SetProgress(float.Parse(AuthenticationManager.Instance.LocalPlayer.Data["Experience"].Value));

            SetFixedWidth(profilePictureImage.rectTransform, true);
            SetFixedWidth(settingsImage.rectTransform, true);
            SetFixedWidth(quitImage.rectTransform, true);
            SetFixedWidth(profileInfoGroup.GetComponent<RectTransform>(), false);
            SetFixedWidth(settingsText.rectTransform, false);
            SetFixedWidth(quitText.rectTransform, false);
        }

        private void OnEnable()
        {
            profileButton.onClick.AddListener(OnProfileClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnDisable()
        {
            profileButton.onClick.RemoveListener(OnProfileClicked);
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        public void ExpandPanel()
        {
            if (isExpanded || isAnimating) return;
            isAnimating = true;

            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimatePanelWidth(collapsedWidth, collapsedWidth * 3));

            isExpanded = true;
        }

        public void CollapsePanel()
        {
            if (!isExpanded || isAnimating) return;
            isAnimating = true;

            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimatePanelWidth(collapsedWidth * 3, collapsedWidth));

            isExpanded = false;
        }

        private IEnumerator AnimatePanelWidth(float startWidth, float targetWidth)
        {
            float time = 0;

            if (startWidth > targetWidth) SetCollapsed();

            while (time < animationDuration)
            {
                time += Time.deltaTime;
                float normalizedTime = time / animationDuration;
                float curvedTime = animationCurve.Evaluate(normalizedTime);
                float width = Mathf.Lerp(startWidth, targetWidth, curvedTime);

                SetPanelWidth(width);

                yield return null;
            }

            SetPanelWidth(targetWidth);

            if (startWidth < targetWidth) SetExpanded();

            animationCoroutine = null;
            isAnimating = false;
        }

        private void SetExpanded()
        {
            SetPanelWidth(collapsedWidth * 3);

            profileInfoGroup.SetActive(true);
            settingsText.gameObject.SetActive(true);
            quitText.gameObject.SetActive(true);
        }

        private void SetCollapsed()
        {
            SetPanelWidth(collapsedWidth);

            profileInfoGroup.SetActive(false);
            settingsText.gameObject.SetActive(false);
            quitText.gameObject.SetActive(false);
        }

        private void SetPanelWidth(float width)
        {
            sidePanelRectTransform.anchorMax = new Vector2(width, sidePanelRectTransform.anchorMax.y);
        }

        private void SetFixedWidth(RectTransform rectTransform, bool icon)
        {
            if (icon)
                rectTransform.anchorMax = new Vector2(collapsedWidth, rectTransform.anchorMax.y);
            else
            {
                rectTransform.anchorMin = new Vector2(collapsedWidth, rectTransform.anchorMin.y);
                rectTransform.anchorMax = new Vector2(collapsedWidth * 3, rectTransform.anchorMax.y);
            }
        }

        private void OnProfileClicked()
        {
            Debug.Log("Profile clicked");
            // Application.OpenURL("https://steamcommunity.com/profiles/" + SteamClient.SteamId);
        }

        private void OnSettingsClicked()
        {
            settingsPanelController.ShowPanel();
        }

        private void OnQuitClicked()
        {
            Application.Quit();
        }

        public void ShowPanel()
        {
            gameObject.SetActive(true);
        }

        public void HidePanel()
        {
            gameObject.SetActive(false);
        }
    }
}