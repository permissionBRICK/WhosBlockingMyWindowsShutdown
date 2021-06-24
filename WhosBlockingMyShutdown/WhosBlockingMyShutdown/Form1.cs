using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HWND = System.IntPtr;

namespace GetOpenWindows
{
    public partial class Form1 : Form
    {
        private bool _allowClose = false;
        private readonly List<string> _windowsList;
        public Form1()
        {
            InitializeComponent();
            _windowsList = new List<string>();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowClose)
            {
                e.Cancel = true;
                MessageBox.Show("Use the Button in the App to Close it!");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _allowClose = true;
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var windows = OpenWindowGetter.GetOpenWindows();
            foreach (var window in windows)
            {
                if (_windowsList.Contains(window)) continue;
                _windowsList.Add(window);
                textBox1.Text += window + Environment.NewLine;
                File.AppendAllText("window-process.log", window + Environment.NewLine);
            }
        }
    }

    /// <summary>Contains functionality to get all the open windows.</summary>
    public static class OpenWindowGetter
    {
        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static IList<string> GetOpenWindows()
        {
            HWND shellWindow = GetShellWindow();
            Dictionary<HWND, string> windows = new Dictionary<HWND, string>();

            EnumWindows(delegate(HWND hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                //if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);
            List<string> WindowNames = new List<string>();
            Parallel.ForEach(windows, window =>
            {
                GetWindowThreadProcessId(window.Key, out var processid);
                var proc = Process.GetProcessById(Convert.ToInt32(processid));
                var name = ProcessGetPath(proc);
                if (name != null)
                {
                    lock (WindowNames)
                    {
                        WindowNames.Add(window.Value + " - " + name);
                    }
                }
            });

            return WindowNames;
        }

        private static string ProcessGetPath(Process process)
        {
            try
            {
                return process.MainModule?.FileName;
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return process.ProcessName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private delegate bool EnumWindowsProc(HWND hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    }
}
