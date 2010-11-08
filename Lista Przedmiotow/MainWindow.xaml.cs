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
using System.Threading;

namespace Lista_Przedmiotow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Controller controller = new Controller();
        private bool isRunning=false;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Start new computation thread if not already running
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pobierzDane_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                isRunning = true;
                Thread thread = new Thread(new ThreadStart(calculateAndReDraw));
                thread.Start();
            }
            else
            {
                logBox.Items.Insert(0, "Zaraz, zaraz, licze...");
            }
        }

        /// <summary>
        /// Sets up controller, runs calculation and finaly refreshes display
        /// </summary>
        private void calculateAndReDraw()
        {
            controller.logBox = logBox;
            controller.calculate();
            redrawDataForm();
            isRunning = false;
        }

        /// <summary>
        /// Refreshes data in main window
        /// </summary>
        private void redrawDataForm()
        {
            DataContainer data = controller.getDataContainer();

            addAll(missingItems, data.missing);
            addAll(duplicateItems, data.duplicates);
        }

        /// <summary>
        /// Adds all items from list to listbox items
        /// </summary>
        /// <param name="listbox"></param>
        /// <param name="list"></param>
        private void addAll(ListBox listbox, List<String> list)
        {
            logBox.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        listbox.Items.Clear();
                        foreach (String item in list)
                        {
                            listbox.Items.Add(item);
                        }
                    }
            ));
        }

        /// <summary>
        /// Open configuration window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void konfiguracjaButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow confWindow = new ConfigurationWindow(controller.getDataContainer());
            confWindow.ShowDialog();

        }

        private void ZamknijButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
