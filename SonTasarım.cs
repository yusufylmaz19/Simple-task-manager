using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Threading;

namespace simpleTaskManager
{
    public partial class SonTasarım : Form
    {
        public SonTasarım()
        {
            InitializeComponent();
        }
        static readonly string[] suffixes =
            { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        public static string FormatSize(Int64 bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }

        private void getProcessInformations()
        {
            dataGridView1.Rows.Clear();
            var searcher = new ManagementObjectSearcher("Select * From Win32_Process");
            var processList = searcher.Get();

            foreach (var process in processList)
            {
                var processName = process["Name"];
                var processID = process["ProcessId"];
                var processPath = process["ExecutablePath"];
                var parentProcessId = process["ParentProcessId"];
                var processMemory = Convert.ToInt64(process["WorkingSetSize"].ToString());

                if (processPath != null)
                {
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(processPath.ToString());
                    var processDescription = fileVersionInfo.FileDescription;
                    dataGridView1.Rows.Add(processName, processID, FormatSize(processMemory), processDescription, parentProcessId);
                    int rowCount = dataGridView1.Rows.Count;
                    lblcounprcs.Text = rowCount.ToString();
                }
            }
        }

        private void getBatteryİnfo()
        {


            ManagementObjectSearcher mos =
            new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Battery");
            ManagementClass wmi = new ManagementClass("Win32_Battery");
            var allBatteries = wmi.GetInstances();
            String estimatedChargeRemaining = String.Empty;

            foreach (var battery in allBatteries)
            {
                estimatedChargeRemaining = Convert.ToString(battery["EstimatedChargeRemaining"]);
            }

            lblremaininginfo.Text = estimatedChargeRemaining + "%";


            foreach (ManagementObject mo in mos.Get())
            {
                if (Convert.ToUInt16(mo["BatteryStatus"]) == 2)
                {

                    pictureBox1.Visible = true;
                    pictureBox2.Visible = false;
                }
                else if (Convert.ToUInt16(mo["BatteryStatus"]) == 1)
                {

                    pictureBox2.Visible = true;
                    pictureBox1.Visible = false;
                }

                lblbatarynamvalue.Text = Convert.ToString(mo["Name"]);
                label44.Text= Convert.ToString(mo["Description"]);
                
            }
        }


        static PerformanceCounter c, r;

        private void getCPUCounter()
        {

            c = new PerformanceCounter();
            c.CategoryName = "Processor";
            c.CounterName = "% Processor Time";
            c.InstanceName = "_Total";

            r = new PerformanceCounter();
            r.CategoryName = "Memory";
            r.CounterName = "% Committed Bytes In Use";



            float fcpu0 = c.NextValue();
            Thread.Sleep(1000);
            float fcpu = c.NextValue();

            float fram = r.NextValue();

            prgrbarCpu.Value = (int)fcpu;
            lblcpuinfo.Text = string.Format("{0:0.00}%", fcpu);


            prgsbarram.Value = (int)fram;
            lblraminfo.Text = string.Format("{0:0.00}%", fram);


            chart1.Series["CPU"].Points.AddY(fcpu);
            chart1.Series["RAM"].Points.AddY(fram);

        }
         
        private void getBiosInfo()
        {
            ManagementObjectSearcher finder = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS ");

            foreach (ManagementObject wmi in finder.Get())
            {
                try
                {
                    label15.Text=wmi["Manufacturer"].ToString();
                    label29.Text = wmi["Name"].ToString();
                    label33.Text = wmi["SerialNumber"].ToString();
                    label35.Text = wmi["version"].ToString();
                    label31.Text = wmi["ReleaseDate"].ToString();
                    label37.Text = wmi["Status"].ToString();
                   }

                catch { }

            }

        }

        private void getUserInfo()
        {
            ManagementObjectSearcher finder = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_UserAccount  ");

            foreach (ManagementObject wmi in finder.Get())
            {
                try
                {
                    label55.Text = wmi["Name"].ToString();
                    label57.Text = wmi["FullName"].ToString();
                    label53.Text = wmi["AccountType"].ToString();
                    label47.Text = wmi["SID"].ToString();
                    label49.Text = wmi["Status"].ToString();
                }

                catch { }

            }
        }

        private void SonTasarım_Load(object sender, EventArgs e)
        {

            timer1.Start();
            timer2.Start();
            getProcessInformations();
            getBiosInfo();
            getUserInfo();
            label1.Text = GetInfoHardware.GetHDDSerialNo();
            label2.Text = GetInfoHardware.GetMACAddress();
            label26.Text = GetInfoHardware.GetCdRomDrive();
            label7.Text = GetInfoHardware.GetPhysicalMemory();
            label5.Text = GetInfoHardware.GetCPUManufacturer();
            label6.Text = GetInfoHardware.GetCpuSpeedInGHz().ToString();
            label4.Text = GetInfoHardware.GetProcessorInformation();
            label8.Text = GetInfoHardware.GetOSInformation();
            label9.Text = GetInfoHardware.GetCurrentLanguage();
            label10.Text = GetInfoHardware.GetComputerName();
            label42.Text = GetInfoHardware.GetNoRamSlots();

            bool Systembit = Environment.Is64BitOperatingSystem;
            if (Systembit)
            {
                label11.Text = "64";
            }
            else
            {
                label11.Text = "32";

            }

            long totalsize = 0;
            foreach (System.IO.DriveInfo label in System.IO.DriveInfo.GetDrives())
            {

                if (label.IsReady)
                {
                    totalsize += label.TotalSize;
                }
            }
            label12.Text = Convert.ToString(totalsize);

            int Processor = Environment.ProcessorCount;
            label13.Text = Convert.ToString(Processor);

        }

        private void btnEndTask_Click(object sender, EventArgs e)
        {
            try
            {
                Process p = Process.GetProcessById(int.Parse(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()));
                getProcessInformations();
                p.Kill();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void runNewTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Form2 frm = new Form2())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    getProcessInformations();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            getProcessInformations();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void timer2_Tick(object sender, EventArgs e)
        {
            getBatteryİnfo();
            getCPUCounter();
        }

        }
}

public static class GetInfoHardware
{

    public static String GetHDDSerialNo()
    {
        ManagementClass mangnmt = new ManagementClass("Win32_LogicalDisk");
        ManagementObjectCollection mcol = mangnmt.GetInstances();
        string result = "";
        foreach (ManagementObject strt in mcol)
        {
            result += Convert.ToString(strt["VolumeSerialNumber"]);
        }
        return result;
    }
    public static string GetMACAddress()
    {
        ManagementClass mancl = new ManagementClass("Win32_NetworkAdapterConfiguration");
        ManagementObjectCollection manageobcl = mancl.GetInstances();
        string MACAddress = String.Empty;
        foreach (ManagementObject myObj in manageobcl)
        {
            if (MACAddress == String.Empty)
            {
                if ((bool)myObj["IPEnabled"] == true) MACAddress = myObj["MacAddress"].ToString();
            }
            myObj.Dispose();
        }

        MACAddress = MACAddress.Replace(":", "");
        return MACAddress;
    }
    public static string GetCdRomDrive()
    {

        ManagementObjectSearcher finder = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_CDROMDrive");

        foreach (ManagementObject wmi in finder.Get())
        {
            try
            {
                return wmi.GetPropertyValue("Drive").ToString();

            }

            catch { }

        }

        return "Sorry CD ROM Drive Letter: Unknown";

    }
    public static string GetAccountName()
    {

        ManagementObjectSearcher finder = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_UserAccount");

        foreach (ManagementObject wmi in finder.Get())
        {
            try
            {

                return wmi.GetPropertyValue("Name").ToString();
            }
            catch { }
        }
        return "Sorry, User Account Name: Unknown";

    }
    public static string GetPhysicalMemory()
    {
        ManagementScope osmanagesys = new ManagementScope();
        ObjectQuery myobjQry = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
        ManagementObjectSearcher oFinder = new ManagementObjectSearcher(osmanagesys, myobjQry);
        ManagementObjectCollection oCollection = oFinder.Get();

        long liveMSize = 0;
        long livecap = 0;

        foreach (ManagementObject obj in oCollection)
        {
            livecap = Convert.ToInt64(obj["Capacity"]);
            liveMSize += livecap;
        }
        liveMSize = (liveMSize / 1024) / 1024;
        return liveMSize.ToString() + "MB";
    }
    public static string GetNoRamSlots()
    {

        int MemSlots = 0;
        ManagementScope osmanagesys = new ManagementScope();
        ObjectQuery oQuery2 = new ObjectQuery("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray");
        ManagementObjectSearcher oSearcher2 = new ManagementObjectSearcher(osmanagesys, oQuery2);
        ManagementObjectCollection oCollection2 = oSearcher2.Get();
        foreach (ManagementObject obj in oCollection2)
        {
            MemSlots = Convert.ToInt32(obj["MemoryDevices"]);

        }
        return MemSlots.ToString();
    }
    public static string GetCPUManufacturer()
    {
        string robotCpu = String.Empty;
        ManagementClass managemnt = new ManagementClass("Win32_Processor");
        ManagementObjectCollection objCol = managemnt.GetInstances();

        foreach (ManagementObject obj in objCol)
        {
            if (robotCpu == String.Empty)
            {
                robotCpu = obj.Properties["Manufacturer"].Value.ToString();
            }
        }
        return robotCpu;
    }
    public static double? GetCpuSpeedInGHz()
    {
        double? GHz = null;
        using (ManagementClass mancl = new ManagementClass("Win32_Processor"))
        {
            foreach (ManagementObject myObj in mancl.GetInstances())
            {
                GHz = 0.001 * (UInt32)myObj.Properties["CurrentClockSpeed"].Value;
                break;
            }
        }
        return GHz;
    }
    public static string GetCurrentLanguage()
    {

        ManagementObjectSearcher finder = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

        foreach (ManagementObject wmi in finder.Get())
        {
            try
            {
                return wmi.GetPropertyValue("CurrentLanguage").ToString();

            }

            catch { }

        }

        return "BIOS Maker: Unknown";

    }
    public static string GetOSInformation()
    {
        ManagementObjectSearcher finder = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
        foreach (ManagementObject wmi in finder.Get())
        {
            try
            {
                return ((string)wmi["Caption"]).Trim() + ", " + (string)wmi["Version"] + ", " + (string)wmi["OSArchitecture"];
            }
            catch { }
        }
        return "Sorry BIOS Maker: Unknown";
    }
    public static String GetProcessorInformation()
    {
        ManagementClass mancl = new ManagementClass("win32_processor");
        ManagementObjectCollection manageobcl = mancl.GetInstances();
        String info = String.Empty;
        foreach (ManagementObject myObj in manageobcl)
        {
            string name = (string)myObj["Name"];
            name = name.Replace("(TM)", "™").Replace("(tm)", "™").Replace("(R)", "®").Replace("(r)", "®").Replace("(C)", "©").Replace("(c)", "©").Replace("    ", " ").Replace("  ", " ");

            info = name + ", " + (string)myObj["Caption"] + ", " + (string)myObj["SocketDesignation"];

        }
        return info;
    }
    public static String GetComputerName()
    {
        ManagementClass mancl = new ManagementClass("Win32_ComputerSystem");
        ManagementObjectCollection manageobcl = mancl.GetInstances();
        String info = String.Empty;
        foreach (ManagementObject myObj in manageobcl)
        {
            info = (string)myObj["Name"];
            //myObj.Properties["Name"].Value.ToString();
            //break;
        }
        return info;
    }

}