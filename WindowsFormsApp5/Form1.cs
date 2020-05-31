using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        Boolean isRecording;
        String RxString;
        public Form1()
        {
            InitializeComponent();
            GetSerialDevice();
        }
        SerialPort temperature = new SerialPort();
        public void GetSerialDevice()
        {

            // Get a list of serial port names.        
            String[] ports = SerialPort.GetPortNames();
            foreach (String portT in ports) { cbPort.Items.Add(portT); }
            cbPort.SelectedIndex = cbPort.Items.Count - 1;
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text == "Connect")
            {
                try
                {
                    String rx = cbPort.Text;
                    //Sets up serial port
                    SP.PortName = rx;
                    SP.BaudRate = 9600;
                    SP.Handshake = System.IO.Ports.Handshake.None;
                    SP.DataBits = 8;
                    SP.ReadTimeout = 200;
                    SP.WriteTimeout = 50;
                    SP.ReceivedBytesThreshold = 2;
                    SP.Open();

                    //Sets button State and Creates function call on data recieved
                    btnConnect.Text = "Disconnect";
                 }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                try // just in case serial port is not open could also be acheved using if(serial.IsOpen)
                {
                    SP.Close();
                    btnConnect.Text = "Connect";
                }
                catch
                {
                }
            }
        }

        

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            
            saveDialog.Filter = "CSV Files (*.csv)| *.csv";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                txtRecordFile.Text = saveDialog.FileName;
                isRecording = true;
            }
            
        }

        private void Serial_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                RxString = "";
               // SP.Encoding = Encoding.GetEncoding(28591);
                RxString = SP.ReadLine();
                this.Invoke(new EventHandler(Record));
            }
            catch {; }        
        }
        int dataLogCounter = 0;
        private void Record(object sender, EventArgs e)
        {
                if (isRecording == true)
                {
                    dataLogCounter++;
                    if (dataLogCounter == 1)
                    {
                        System.IO.File.WriteAllText(saveDialog.FileName, RxString);
                    }                    
                    else
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(saveDialog.FileName, true))
                        {                            
                            file.Write(DateTime.Now.ToString() + ',' + dataLabel.Text + ',' +   RxString  );
                        }
                    }
                }

                ParseData(RxString);
        }
        bool hasBeenAdd=false;
        int num_data;
        private void ParseData(String str)
        {
            try { 
            string[] data = str.Split(' ');
            num_data = data.Count();
                chart1.Series[0].BorderWidth = 2;
                chart1.Series[0].Points.Add(Convert.ToDouble(data[0]));            
            if (chart1.Series[0].Points.Count > 300)
            {
                chart1.Series[0].Points.RemoveAt(0);

            }

            if(num_data>1 && hasBeenAdd == false) { 
                for(int i = 1; i <= num_data; i++) { 
                chart1.Series.Add(i.ToString());
                        chart1.Series[i.ToString()].BorderWidth = 2;
                    }
            hasBeenAdd = true;
            }
            if (num_data > 1)
            {
                for (int i = 1; i <= num_data; i++)
                {
                    chart1.Series[i].Points.Add(Convert.ToDouble(data[i]));
                    if (chart1.Series[i].Points.Count > 20)
                    {
                        chart1.Series[i].Points.RemoveAt(0);
                    }
                }
            }
            }
            catch
            {
                ;
            }

        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            GetSerialDevice();
        }
    }

}
