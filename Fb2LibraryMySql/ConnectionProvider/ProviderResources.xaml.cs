using System.ComponentModel.Composition;
using System.Windows;

namespace Fb2Library.MySql.ConnectionProvider
{
    /// <summary>
    /// Interaction logic for ProviderResources.xaml
    /// </summary>
    public partial class ProviderResources : ResourceDictionary
    {
        public ProviderResources()
        {
            InitializeComponent();
        }
    
        [Export("RecentItemsTabControl.DataTemplates")]
        public DataTemplate RecentItemContentTemplate
        {
            get
            {
                return (DataTemplate)this["RecentItemContentTemplate"];
            }
        }
    }
}