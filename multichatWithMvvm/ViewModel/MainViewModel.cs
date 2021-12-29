using multichatWithMvvm.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace multichatWithMvvm.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public RelayCommand StartCommand { get; set; }
        public RelayCommand ConnectCommand { get; set; }
        MainWindow window;
        public RelayCommand SendCommand { get; set; }
        TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string Receive;
        public string TextToSend;


        public readonly BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        public readonly BackgroundWorker backgroundWorker2 = new BackgroundWorker();
        public MainViewModel( MainWindow  mainwindow)
        {
            window = mainwindow;
            backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            backgroundWorker2.DoWork += BackgroundWorker2_DoWork;
          mainwindow .  ServerIpTextbox.Text = "192.168.100.12";
            //SelectedBook = new Book();
            StartCommand = new RelayCommand((sender) =>
            {

                TcpListener listener = new TcpListener(IPAddress.Parse(mainwindow.ServerIpTextbox.Text), int.Parse(mainwindow.ServerPortTextBox.Text));
                listener.Start();
                client = listener.AcceptTcpClient();
                STR = new StreamReader(client.GetStream());
                STW = new StreamWriter(client.GetStream());
                STW.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
            });
            SendCommand = new RelayCommand((sender) =>
            {
                if (mainwindow.MessageTextBox.Text != "")
                {
                    TextToSend = mainwindow.MessageTextBox .Text;
                    backgroundWorker2.RunWorkerAsync();
                }
                mainwindow.MessageTextBox .Text = "";
            });
           ConnectCommand = new RelayCommand((sender) =>
            {
                client = new TcpClient();
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(mainwindow.ClientIpTextbox.Text), int.Parse(mainwindow.ClientPortTextbox.Text));

                try
                {

                   mainwindow. ChatScreenTextbox.Text += "Connect To Server\n";
                    STW = new StreamWriter(client.GetStream());
                    STR = new StreamReader(client.GetStream());
                    STW.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                STW.WriteLine(TextToSend);
                App.Current.Dispatcher.Invoke(() =>
                {
                    window .ChatScreenTextbox.Text += "Me : " + TextToSend + "\n";

                });
            }
            else
            {
                MessageBox.Show("Sending Failed");
            }
            backgroundWorker2.CancelAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    Receive = STR.ReadLine();
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        window.ChatScreenTextbox.Text += "You : " + Receive + "\n";
                        Receive = "";
                    });
                }
                catch (Exception)
                {

                }
            }
        }
    }



    }
  

