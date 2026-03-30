using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MaintenanceScene.Presentation.Main
{
    public class ActionPointsView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ActionPointsView)}]";

        [SerializeField] private List<Image> _circles;

        public void SetActionPoints(int current, int max)
        {
            for (int i = 0; i < _circles.Count; i++)
            {
                if (i < max)
                {
                    _circles[i].gameObject.SetActive(true);
                    _circles[i].color = i < current ? Color.white : Color.gray;
                }
                else
                {
                    _circles[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
