using Combat;
using UnityEngine;
using UnityEngine.EventSystems;
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
        }

        private void OnDestroy()
        {
            damageable.OnClientHealthUpdated -= UpdateUI;
        }

        private void UpdateUI(float value, float maxValue)
        {
            healthBar.fillAmount = value / maxValue;
        }
    }
}