using Combat;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField]
        private Image healthBar;
        [SerializeField]
        private GameObject parent;
        [SerializeField]
        private Damageable damageable;

        private void Awake()
        {
            damageable.OnClientHealthUpdated += UpdateUI;
            damageable.OnClientFocused += ShowUI;
            damageable.OnClientUnfocused += HideUI;
        }

        private void Start()
        {
            HideUI();
        }

        private void OnDestroy()
        {
            damageable.OnClientHealthUpdated -= UpdateUI;
            damageable.OnClientFocused -= ShowUI;
            damageable.OnClientUnfocused -= HideUI;
        }

        private void UpdateUI(float value, float maxValue)
        {
            healthBar.fillAmount = value / maxValue;
        }

        private void ShowUI()
        {
            parent.SetActive(true);
        }

        private void HideUI()
        {
            parent.SetActive(false);
        }
    }
}