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

        public void SetUnitsInQueue(int unitsAmount)
        {
            queueText.text = unitsAmount.ToString();
        }
    }
}