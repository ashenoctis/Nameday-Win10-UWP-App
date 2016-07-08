using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Microsoft.Xaml.Interactivity;

namespace NameDay
{
    [TypeConstraint(typeof(ListViewBase))]
    public class AutoScrollToSelectedItemBehavior : DependencyObject, IBehavior
    {      
        private ListViewBase AssociatedListView => AssociatedObject as ListView;

        public DependencyObject AssociatedObject { get; private set; }

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;

            if (AssociatedListView != null)
                AssociatedListView.SelectionChanged += ListViewOnSelectionChanged;
        }

        public void Detach()
        {
            if (AssociatedListView != null)
                AssociatedListView.SelectionChanged -= ListViewOnSelectionChanged;
        }

        private void ListViewOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (AssociatedListView.SelectedItem == null)
                return;

            AssociatedListView.ScrollIntoView(AssociatedListView.SelectedItem);
        }
    }
}
