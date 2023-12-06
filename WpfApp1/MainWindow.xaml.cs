using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinUpdStop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ////WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Service' AND TargetInstance.Name = 'wuauserv'");
            //WqlEventQuery query = new WqlEventQuery("__InstanceCreationEvent",
            //    new TimeSpan(0, 0, 1), "TargetInstance isa 'Win32_Service' and TargetInstance.Name = 'wuauserv'");
            //ManagementEventWatcher watcher = new ManagementEventWatcher(query);
            //watcher.EventArrived += new EventArrivedEventHandler(UpdateServiceStarted);
            //watcher.Start();

            //this.WindowState = WindowState.Minimized;
            //this.Hide();

            AddMessageToTextBox("Приложение запущено!");

            try { StopUp(); }
            catch { }

            PauseClose();
        }

        private async void PauseClose()
        {
            AddMessageToTextBox("Закрываюсь...");
            await Task.Delay(3000);
            this.Close();
        }

        private void UpdateServiceStarted(object sender, EventArrivedEventArgs e)
        {
            AddMessageToTextBox("Windows Update service has started. Disabling it...");

            ServiceController sc = new ServiceController("wuauserv");

            if (sc.Status == ServiceControllerStatus.Running)
            {
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped);
            }

            ManagementObject wmiService = new ManagementObject("Win32_Service.Name='wuauserv'");
            wmiService.InvokeMethod("ChangeStartMode", new object[] { "Disabled" });

            AddMessageToTextBox("Windows Update service has been disabled.");
        }

        private void StopUp()
        {
            using (ServiceController sc = new ServiceController("wuauserv"))
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    AddMessageToTextBox("Обновления Остановлены!");
                }

                ManagementObject wmiService = new ManagementObject("Win32_Service.Name='wuauserv'");
                wmiService.InvokeMethod("ChangeStartMode", new object[] { "Disabled" });
            }
        }
        private void AddMessageToTextBox(string message)
        {
            Dispatcher.Invoke(() =>
            {
                TB.Text += message + Environment.NewLine;
            });
        }
    }
}
