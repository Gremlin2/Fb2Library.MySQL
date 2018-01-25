using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Fb2Library.MySql.ConnectionProvider
{
    public class MySqlConnectionSettingsViewModel : INotifyPropertyChanged
    {
        private readonly MySqlConnectionSettings connectionSettings;

        public event PropertyChangedEventHandler PropertyChanged;

        public MySqlConnectionSettingsViewModel(MySqlConnectionSettings connectionSettings)
        {
            if (connectionSettings == null)
            {
                throw new ArgumentNullException("connectionSettings");
            }

            Contract.EndContractBlock();

            this.connectionSettings = connectionSettings;
        }

        public string ServerName
        {
            get
            {
                return connectionSettings.ServerName;
            }
            set
            {
                if (connectionSettings.ServerName != value)
                {
                    connectionSettings.ServerName = value;
                    OnPropertyChanged("ServerName");
                }
            }
        }

        public string Database
        {
            get
            {
                return connectionSettings.Database;
            }
            set
            {
                if (connectionSettings.Database != value)
                {
                    connectionSettings.Database = value;
                    OnPropertyChanged("Database");
                }
            }
        }

        public string Username
        {
            get
            {
                return connectionSettings.Username;
            }
            set
            {
                if (connectionSettings.Username != value)
                {
                    connectionSettings.Username = value;
                    OnPropertyChanged("Username");
                }
            }
        }

        public bool RememberPassword
        {
            get
            {
                return connectionSettings.RememberPassword;
            }
            set
            {
                if (connectionSettings.RememberPassword != value)
                {
                    connectionSettings.RememberPassword = value;
                    OnPropertyChanged("RememberPassword");
                }
            }
        }

        public string PlainTextPassword
        {
            get
            {
                return connectionSettings.PlainTextPassword;
            }
            set
            {
                if (connectionSettings.PlainTextPassword != value)
                {
                    connectionSettings.PlainTextPassword = value;
                    OnPropertyChanged("PlainTextPassword");
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

