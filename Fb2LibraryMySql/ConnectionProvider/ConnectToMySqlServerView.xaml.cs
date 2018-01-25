using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fb2Library.MySql.ConnectionProvider
{
    /// <summary>
    /// Interaction logic for ConnectToMySqlServerView.xaml
    /// </summary>
    public partial class ConnectToMySqlServerView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MySqlConnectionSettingsViewModel), typeof(ConnectToMySqlServerView),
            new PropertyMetadata(null));

        public MySqlConnectionSettingsViewModel ViewModel
        {
            get
            {
                return (MySqlConnectionSettingsViewModel) GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        public ConnectToMySqlServerView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MySqlConnectionSettings connectionSettings = e.NewValue as MySqlConnectionSettings;
            if (connectionSettings != null)
            {
                ViewModel = new MySqlConnectionSettingsViewModel(connectionSettings);
            }
            else
            {
                ViewModel = null;
            }
        }
    }
}
