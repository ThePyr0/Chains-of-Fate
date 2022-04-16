using System.Collections;
using TMPro;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public float outofboundsBounceForce = 10.0f;
        public float boundaryMinDistance = 10.0f;
        public float loadRange = 10.0f;
        public float unloadRange = 15.0f;
        public float boundaryRange = 3.0f;
        internal bool levelLoadingLock = false;
        
        public CombatUI combatUI;
        public GameObject levelLoadingIndicatorUI;
        public TextMeshProUGUI levelLoadingUIText;
        public float levelLoadingTime = 2.0f;

        private bool shownIndicator = false;

        public void ShowCombatUI()
        {
            combatUI.gameObject.SetActive(true);
        }
        
        public void HideCombatUI()
        {
            combatUI.gameObject.SetActive(false);
        }

        public void ShowLevelLoadingIndicator(string sceneName)
        {
            levelLoadingIndicatorUI.SetActive(true);
            levelLoadingUIText.text = sceneName + "...";
        }
        
        public void HideLevelLoadingIndicator()
        {
            if (!shownIndicator)
            {
                StartCoroutine(HideIndicator());
            }
        }

        IEnumerator HideIndicator()
        {
            shownIndicator = true;
            yield return new WaitForSeconds(levelLoadingTime);
            levelLoadingIndicatorUI.SetActive(false);
            shownIndicator = false;
        }
        
        public virtual void Awake()
        {
            Instance = this;
            
            levelLoadingIndicatorUI.SetActive(false);
        }
    }
}