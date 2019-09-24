using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.CustomizationMenu
{
    public class CustomizationManager : MonoBehaviour
    {
        [SerializeField] ScrollView scrollView = default;
        [SerializeField] string[] NeededCoinsList;
        [SerializeField] Text selectedItemInfo = default;

        void Start()
        {
            var items = NeededCoinsList.Select(i => new ItemData($"${i}")).ToArray();

            //var items = Enumerable.Range(0, 20)
            //    .Select(i => new ItemData($"Cell {i}"))
            //    .ToArray();

            scrollView.OnSelectionChanged(OnSelectionChanged);
            scrollView.UpdateData(items);
            scrollView.SelectCell(0);
        }

        void OnSelectionChanged(int index)
        {
            selectedItemInfo.text = $"Selected item info: index {index}";
        }
    }
}
