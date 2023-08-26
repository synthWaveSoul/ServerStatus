using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading;
using System.IO;
using System.Media;
using System.Diagnostics;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text.Json;

namespace ServerStatus
{
    public partial class Form1 : Form
    {
        MqttClient mqttClient;

        SoundPlayer playerHighTemp = new SoundPlayer();
        

        public Form1()
        {
            InitializeComponent();
        }

        string[] ip = new string[]
        {
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
            "localhost",     // your device name
        };

        string rep;
        string repConfirm;
        int confirmOffline = 0;
        string tmpStatus;

        private Control.ControlCollection GetControls()
        {
            return Controls;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < (ip.Count()+1); i++)
            {
                if ((ip.Count() - 1) < i)
                {
                    i = -1;
                }
                else
                {
                    Ping ping = new Ping();
                    PingReply reply = ping.Send(ip[i], 300);
                    rep = reply.Status.ToString();

                    if (rep != "Success")
                    {
                        tmpStatus = reply.Status.ToString();

                        for (int j = 0; j < 5; j++)
                        {
                            reply = ping.Send(ip[i], 300);
                            repConfirm = reply.Status.ToString();

                            if ((repConfirm == "Success"))
                            {
                                confirmOffline++;
                            }

                            Thread.Sleep(400);
                        }

                        if ((repConfirm == "Success"))
                        {
                            Controls["label" + (i + 1).ToString()].Text = reply.Status.ToString();
                            SetPicture(i + 1);
                            AddLog(i + 1, reply.Status.ToString());
                            Application.DoEvents();
                            Thread.Sleep(1000);

                            confirmOffline = 0;
                        }
                        else
                        {
                            if (confirmOffline > 2)
                            {
                                Controls["label" + (i + 1).ToString()].Text = "Success";
                                SetPicture(i + 1);
                                AddLog(i + 1, reply.Status.ToString());
                                Application.DoEvents();
                                Thread.Sleep(1000);

                                confirmOffline = 0;
                            }
                            else
                            {
                                Controls["label" + (i + 1).ToString()].Text = tmpStatus;
                                SetPicture(i + 1);
                                AddLog(i + 1, tmpStatus);
                                Application.DoEvents();
                                Thread.Sleep(1000);

                                confirmOffline = 0;
                            }
                        }

                    }
                    else
                    {
                        Controls["label" + (i + 1).ToString()].Text = rep; //reply.Status.ToString();
                        SetPicture(i + 1);
                        AddLog(i + 1, reply.Status.ToString());
                        Application.DoEvents();
                        Thread.Sleep(1000);
                    }

                }           

                if (labelBackgroundWorkStatus.Text == "stop")
                {
                    i = (ip.Count() + 5);       // + 5 to be sure that the loop will be terminated
                }

                label00.Text = i.ToString();
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == true)
            {
                MessageBox.Show("Process is in work already");
            }
            else
            {
                labelBackgroundWorkStatus.Text = "start";
                stopToolStripMenuItem.Enabled = true;
                startToolStripMenuItem.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == true)
            {
                backgroundWorker1.CancelAsync();
                labelBackgroundWorkStatus.Text = "stop";
                stopToolStripMenuItem.Enabled = false;
                startToolStripMenuItem.Enabled= true;
                SetLabelsUnknown();
                SetPicturesUnknown();
            }
            else
            {
                MessageBox.Show("Process is not yet running, please start it first");
            }
        }

        public void SetLabelsUnknown()
        {
            for (int i = 1; i <= ip.Count(); i++)
            {
                Controls["label" + i.ToString()].Text = "unknown";
            }
        }

        public void SetPicturesUnknown()
        {
            for (int i = 1; i <= ip.Count(); i++)
            {
                if (Controls["pictureBox" + i.ToString()] != null)
                {
                    Controls["pictureBox" + i.ToString()].BackgroundImage = Properties.Resources.server_unknown;
                }
                else
                {
                    Controls["labelName" + i.ToString()].ForeColor = Color.White;
                }
            }
        }

        public void SetPicture(int p)
        {
            if (Controls["pictureBox" + p.ToString()] != null)
            {
                if (Controls["label" + p.ToString()].Text == "Success")
                {
                    Controls["pictureBox" + p.ToString()].BackgroundImage = Properties.Resources.server_online;
                }
                else
                {
                    Controls["pictureBox" + p.ToString()].BackgroundImage = Properties.Resources.server_offline;                   
                }
            }
            else
            {
                if (Controls["label" + p.ToString()].Text == "Success")
                {
                    Controls["labelName" + p.ToString()].ForeColor = Color.LightGreen;
                }
                else
                {
                    Controls["labelName" + p.ToString()].ForeColor = Color.Red;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the application?", "Quit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                backgroundWorker1.CancelAsync();

                if (mqttClient.IsConnected)
                {
                    mqttClient.Disconnect();
                }

                Application.Exit();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //goes to full screen mode on start
            //TopMost = true;                 //always on top
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            //Connect to MQTT broker
            Task.Run(() =>
            {
                mqttClient = new MqttClient("localhost");   // IP of mqtt broker
                mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
                mqttClient.Subscribe(new string[] { "meraki/v1/mt/your-device-details" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                mqttClient.Subscribe(new string[] { "meraki/v1/mt/your-device-details" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                mqttClient.Subscribe(new string[] { "meraki/v1/mt/your-device-details" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                mqttClient.Subscribe(new string[] { "meraki/v1/mt/your-device-details" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                mqttClient.Subscribe(new string[] { "meraki/v1/mt/your-device-details" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                mqttClient.Subscribe(new string[] { "meraki/v1/mt/your-device-details" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                mqttClient.Connect(Guid.NewGuid().ToString(), "mqtt username", "password");  // client ID - name of program that connects to broker, can be found in broker logs
                if (mqttClient.IsConnected)
                {
                    labelCheckBrokerCon.Visible = false;
                }
            });
        }

        private TimeSpan GetTimePassed(string readValueDate)
        {
            // set current time to TimeSpan
            DateTime timeNow = DateTime.Now;
            string timeNowString = timeNow.ToLongTimeString();
            TimeSpan timeSpanNow = TimeSpan.Parse(timeNowString);

            // set read value time to TimeSpan
            string readValueTime = readValueDate.Substring(11, 8);
            TimeSpan timeSpanValueWrong = TimeSpan.Parse(readValueTime);
            TimeSpan timeSpanValueCorrected = timeSpanValueWrong.Add(new TimeSpan(1, 0, 0));    // add one hour due to wrong sensor time

            TimeSpan timeDiff = timeSpanNow - timeSpanValueCorrected;

            return timeDiff;
        }

        private void MqttClient_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            playerHighTemp.Stream = Properties.Resources.soundHighTemp;

            if (e.Topic.Contains("MAC-of-your-device/temperature"))  //Device name
            {
                serverTempChanged = DateTime.Now;

                SensorValue temp = JsonSerializer.Deserialize<SensorValue>(Encoding.UTF8.GetString(e.Message));

                if (labelServerTemp.Text == "00")
                {
                    labelServerTemp.Text = temp.celsius.ToString() + "°C";
                }

                TimeSpan timeDiff = GetTimePassed(temp.ts.ToString());

                //Meraki sensor is collecting data every 2 min but sending all readings at once every 20 min, it also have delays in sending values
                if (timeDiff < new TimeSpan(0, 2, 35))
                {
                    labelServerTemp.Text = temp.celsius.ToString() + "°C";

                    if (temp.celsius >= 25)
                    {
                        if (labelServerTemp.ForeColor == Color.Chartreuse)
                        {
                            playerHighTemp.Play();
                        }
                        labelServerTemp.ForeColor = Color.Red;
                    }
                    else
                    {
                        labelServerTemp.ForeColor = Color.Chartreuse;
                    }
                } 
            }
            else if (e.Topic.Contains("MAC-of-your-device/temperature")) //Device name
            {
                storageTempChanged = DateTime.Now;

                SensorValue temp = JsonSerializer.Deserialize<SensorValue>(Encoding.UTF8.GetString(e.Message));

                if (labelStorageTemp.Text == "00")
                {
                    labelStorageTemp.Text = temp.celsius.ToString() + "°C";
                }

                TimeSpan timeDiff = GetTimePassed(temp.ts.ToString());

                if (timeDiff < new TimeSpan(0, 2, 35))
                {
                    labelStorageTemp.Text = temp.celsius.ToString() + "°C";

                    if (temp.celsius >= 25)
                    {
                        if (labelStorageTemp.ForeColor == Color.Chartreuse)
                        {
                            playerHighTemp.Play();
                        }
                        labelStorageTemp.ForeColor = Color.Red;
                    }
                    else
                    {
                        labelStorageTemp.ForeColor = Color.Chartreuse;
                    }
                }                    
            }
            else if (e.Topic.Contains("MAC-of-your-device/temperature")) //Device name
            {
                wanTempChanged = DateTime.Now;

                SensorValue temp = JsonSerializer.Deserialize<SensorValue>(Encoding.UTF8.GetString(e.Message));

                if (labelWanTemp.Text == "00")
                {
                    labelWanTemp.Text = temp.celsius.ToString() + "°C";
                }

                TimeSpan timeDiff = GetTimePassed(temp.ts.ToString());

                if (timeDiff < new TimeSpan(0, 2, 35))
                {
                    labelWanTemp.Text = temp.celsius.ToString() + "°C";

                    if (temp.celsius >= 25)
                    {
                        if (labelWanTemp.ForeColor == Color.Chartreuse)
                        {
                            playerHighTemp.Play();
                        }
                        labelWanTemp.ForeColor = Color.Red;
                    }
                    else
                    {
                        labelWanTemp.ForeColor = Color.Chartreuse;
                    }
                } 
            }
            else if (e.Topic.Contains("MAC-of-your-device/batteryPercentage"))  //Device 1
            {
                SensorBattery battperc = JsonSerializer.Deserialize<SensorBattery>(Encoding.UTF8.GetString(e.Message));
                labelBatteryServer.Text = battperc.batteryPercentage.ToString() + "%";
                pictureBoxServerFore.Size = new Size((battperc.batteryPercentage)*2, 35);    //"*2" due to form scale and bigger bar on form than 100%
                if (battperc.batteryPercentage < 10) { pictureBoxServerFore.BackColor = Color.Red; } else { pictureBoxServerFore.BackColor = Color.LawnGreen; }
            }
            else if (e.Topic.Contains("MAC-of-your-device/batteryPercentage")) //Device 2
            {
                SensorBattery battperc = JsonSerializer.Deserialize<SensorBattery>(Encoding.UTF8.GetString(e.Message));
                labelBatteryStorage.Text = battperc.batteryPercentage.ToString() + "%";
                pictureBoxStorageFore.Size = new Size((battperc.batteryPercentage) * 2, 35);    //"*2" due to form scale and bigger bar on form than 100%
                if (battperc.batteryPercentage < 10) { pictureBoxStorageFore.BackColor = Color.Red; } else { pictureBoxStorageFore.BackColor = Color.LawnGreen; }
            }
            else if (e.Topic.Contains("MAC-of-your-device/batteryPercentage")) //Device 3
            {
                SensorBattery battperc = JsonSerializer.Deserialize<SensorBattery>(Encoding.UTF8.GetString(e.Message));
                labelBatteryWAN.Text = battperc.batteryPercentage.ToString() + "%";
                pictureBoxWanFore.Size = new Size((battperc.batteryPercentage) * 2, 35);    //"*2" due to form scale and bigger bar on form than 100%
                if (battperc.batteryPercentage < 10) { pictureBoxWanFore.BackColor = Color.Red; } else { pictureBoxWanFore.BackColor = Color.LawnGreen; }
            }
        }

        private void timerSetDateTime_Tick(object sender, EventArgs e)
        {
            //sets the size of the label so that it always fits on the screen

            labelDateTime.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy    HH:mm:ss");

            int i = 79;

            while (labelDateTime.Width > 1730)
            {
                i--;
                labelDateTime.Font = new Font("Arial Black", i);                                
            }
        }

        public int workNum = 1;

        private void label00_TextChanged(object sender, EventArgs e)
        {
            //a status bar showing whether pings are being executed 

            if (pictureBoxWork21.BackColor != Color.SpringGreen)
            {
                Controls["pictureBoxWork" + workNum.ToString()].BackColor = Color.SpringGreen;
            }
            else
            {
                Controls["pictureBoxWork" + workNum.ToString()].BackColor = Color.DodgerBlue;
            }
           
            if (workNum == 21)
            {
                workNum = 1;
            }
            else
            {
                workNum++;
            }
        }

        List<int> listStatus = new List<int>();

        string path = @"logs.txt";

        public void AddLog (int l, string status)
        {
            //adds logs when one of the devices goes offline.
            //It adds the device number to the list so that it is not added to the logs every time it pings until it goes online, then it is removed from the list

            StreamWriter sw;

            if (!File.Exists(path))
            {
                sw = File.CreateText(path);
            }
            else
            {
                sw = new StreamWriter(path, true);
            }

            var player = new SoundPlayer();
            player.Stream = Properties.Resources.soundSystemOffline;
            var playerOnline = new SoundPlayer();
            playerOnline.Stream = Properties.Resources.soundSystemOnline;

            int onList = listStatus.IndexOf(l);

            if (status != "Success")
            {                
                if (onList < 0)
                {
                    listStatus.Add(l);
                    sw.WriteLine("Device" + " $ " + Controls["labelName" + l.ToString()].Text + " $ " + "gone offline at" + " $ " + DateTime.Now.ToString("dddd $ dd/MM/yyyy $ HH:mm:ss") + " $ " + "Reply status: " + " $ " + status);
                    player.Play();
                }                                
            }
            else
            {
                if (onList > 0)
                {
                    listStatus.Remove(l);
                    sw.WriteLine("Device" + " $ " + Controls["labelName" + l.ToString()].Text + " $ " + "came online at" + " $ " + DateTime.Now.ToString("dddd $ dd/MM/yyyy $ HH:mm:ss") + " $ " + "Reply status: " + " $ " + status);
                    playerOnline.Play();
                }
            }
            
            sw.Close();
        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            if (File.Exists(path))
            {
                Form2 logWindow = new Form2();
                logWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("The log file does not exist \n\nPlease note that the log file is not created until the first time the Offline status occurs on one of the devices since the program was started ");
            }            
        }

        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!mqttClient.IsConnected)
            {
                try
                {
                    mqttClient.Connect(Guid.NewGuid().ToString(), "mqtt username", "password");
                    labelCheckBrokerCon.Visible = false;
                }
                catch (Exception ex)
                {
                    labelCheckBrokerCon.Text = ex.Message;
                    labelCheckBrokerCon.Visible = true;
                }
            }
            backgroundWorker3.CancelAsync();
        }

        private void timerCheckBrokerCon_Tick(object sender, EventArgs e)
        {
            backgroundWorker3.RunWorkerAsync();
        }

        DateTime serverTempChanged;
        DateTime storageTempChanged;
        DateTime wanTempChanged;
        
        private void timerCheckLastUpdate_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan difServer = now - serverTempChanged;
            TimeSpan difStorage = now - storageTempChanged;
            TimeSpan difWan = now - wanTempChanged;

            if (difServer.TotalMinutes > 50)
            {
                labelServerTemp.ForeColor = Color.Silver;
            }

            if (difStorage.TotalMinutes > 50)
            {
                labelStorageTemp.ForeColor = Color.Silver;
            }

            if (difWan.TotalMinutes > 50)
            {
                labelWanTemp.ForeColor = Color.Silver;
            }
        }
    }

    class SensorValue
    {
        public string ts { get; set; }
        public float fahrenheit { get; set; }
        public float celsius { get; set; }

    }

    class SensorBattery
    {
        public int batteryPercentage { get; set; }
    }
}