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

        [Export("DatabaseInfoView.DataTemplates")]
        public DataTemplate DatabaseInfoTemplate
        {
            get
            {
                return (DataTemplate)this["DatabaseInfoTemplate"];
            }
        }

        [Export("DatabaseInfoView.Commands.DataTemplates")]
        public DataTemplate DatabaseInfoCommandsTemplate
        {
            get
            {
                return (DataTemplate)this["DatabaseInfoCommandsTemplate"];
            }
        }
    }
}