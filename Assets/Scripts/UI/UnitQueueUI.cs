using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitQueueUI : MonoBehaviour
    {
        [SerializeField] private Image queueProgressImage;
        [SerializeField] private TMP_Text queueText;
        
        private float progressVelocity;

        public void SetUnitsInQueue(int unitsAmount)
        {
            queueText.text = unitsAmount.ToString();
        }

        public void UpdateProgress(float newValue, float maxValue)
        {
            float progress = newValue / maxValue;
            if (progress < queueProgressImage.fillAmount)
            {
                queueProgressImage.fillAmount = progress;
            }
            else
            {
                queueProgressImage.fillAmount = Mathf.SmoothDamp(
                    queueProgressImage.fillAmount,
                    progress,
                    ref progressVelocity,
                    0.1f);
            }
        }
    }
}