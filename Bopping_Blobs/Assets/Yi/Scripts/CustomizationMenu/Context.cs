using System;

namespace FancyScrollView.CustomizationMenu
{
    public class Context
    {
        public int SelectedIndex = -1;
        public Action<int> OnCellClicked;
    }
}
