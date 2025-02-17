using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Framework
{
    public class Loading : MonoBehaviour
    {

        public void StartLoading(Button buttonClicked, LoadingBarAnimator loadingBar)
        {
            buttonClicked.interactable = false;
            loadingBar.StartLoading();
        }

        public void StopLoading(Button buttonClicked, LoadingBarAnimator loadingBar)
        {
            loadingBar.StopLoading();
            buttonClicked.interactable = true;
        }
    }
}
