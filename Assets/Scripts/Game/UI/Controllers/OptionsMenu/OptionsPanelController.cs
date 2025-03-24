using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using Assets.Scripts.Game.UI.Controllers.SettingsMenu;

namespace Assets.Scripts.Game.UI.Controllers.OptionsMenu
{
    public class OptionsPanelController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Components")]
        [SerializeField] private RectTransform optionsPanelRectTransform;
        [SerializeField] private Button steamProfileButton;
        [SerializeField] private GameObject profileInfoGroup;
        [SerializeField] private TextMeshProUGUI profileNameText;
        [SerializeField] private TextMeshProUGUI profileLevelText;
        [SerializeField] private Button settingsButton;
        [SerializeField] private TextMeshProUGUI settingsText;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI quitText;

        [Header("Animation Settings")]
        [SerializeField] private float collapsedWidth = 100f;
        [SerializeField] private float animationDuration = 0.1f;
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("References")]
        [SerializeField] private SettingsPanelController settingsPanelController;

        private Coroutine animationCoroutine;
        private bool isExpanded = false;
        private bool isAnimating = false;

        private void Awake()
        {
            profileNameText.text = PlayerPrefs.GetString("PlayerName");
            profileLevelText.text = "Level " + PlayerPrefs.GetInt("PlayerLevel", 0); ;

            SetPanelWidth(collapsedWidth);
            SetButtonsCollapsedState();
        }

        private void OnEnable()
        {
            steamProfileButton.onClick.AddListener(OnSteamProfileClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnDisable()
        {
            steamProfileButton.onClick.RemoveListener(OnSteamProfileClicked);
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isAnimating) ExpandPanel();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isAnimating) CollapsePanel();
        }

        private void ExpandPanel()
        {
            if (isExpanded || isAnimating) return;

            isAnimating = true;

            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);

            animationCoroutine = StartCoroutine(AnimatePanelWidth(collapsedWidth, collapsedWidth * 4));
            isExpanded = true;
        }

        private void CollapsePanel()
        {
            if (!isExpanded || isAnimating) return;

            isAnimating = true;

            SetButtonsCollapsedState();

            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);

            animationCoroutine = StartCoroutine(AnimatePanelWidth(collapsedWidth * 4, collapsedWidth));
            isExpanded = false;
        }

        private IEnumerator AnimatePanelWidth(float startWidth, float targetWidth)
        {
            float time = 0;

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

            if (isExpanded)
                SetButtonsExpandedState();

            animationCoroutine = null;
            isAnimating = false;
        }

        private void SetButtonsCollapsedState()
        {
            profileInfoGroup.SetActive(false);
            settingsText.gameObject.SetActive(false);
            quitText.gameObject.SetActive(false);
        }

        private void SetButtonsExpandedState()
        {
            profileInfoGroup.SetActive(true);
            settingsText.gameObject.SetActive(true);
            quitText.gameObject.SetActive(true);
        }

        private void SetPanelWidth(float width)
        {
            optionsPanelRectTransform.sizeDelta = new Vector2(width, optionsPanelRectTransform.sizeDelta.y);
            optionsPanelRectTransform.anchoredPosition = new Vector2(width / 2, optionsPanelRectTransform.anchoredPosition.y);
        }

        private void OnSteamProfileClicked()
        {
            // open steam profile
            Debug.Log("Steam profile clicked");
        }

        private void OnSettingsClicked()
        {
            settingsPanelController.ShowPanel();
        }

        private void OnQuitClicked()
        {
            // show confirmation dialog and fix quit states to handle lobby situations
            Application.Quit();
        }
    }
}