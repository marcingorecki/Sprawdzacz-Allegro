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
using System.Windows.Shapes;

namespace Lista_Przedmiotow
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {

        private DataContainer data;

        /// <summary>
        /// Initialize window using configuration data
        /// </summary>
        /// <param name="dataContainer"></param>
        public ConfigurationWindow(DataContainer dataContainer)
        {
            InitializeComponent();
            data = dataContainer;
            username.Text = data.userLogin;
            password.Text = data.userPassword;
            webapikey.Text = data.webapiKey;
            country.Text = data.countryCode.ToString();
            version.Text = data.localVersion.ToString();
        }

        /// <summary>
        /// Save configruation data using values from input boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            data.userLogin = username.Text;
            data.userPassword = password.Text;
            data.webapiKey = webapikey.Text;
            int.TryParse(country.Text, out data.countryCode);
            int.TryParse(version.Text, out data.localVersion);

            data.save();
            Close();
        }

        /// <summary>
        /// Close window without saving changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
