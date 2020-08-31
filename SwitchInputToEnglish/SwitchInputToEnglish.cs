using KeePass.Plugins;
using KeePass.Util;

using System;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Linq;
using System.Globalization;


namespace SwitchInputToEnglish
{
    public sealed class SwitchInputToEnglishExt : Plugin
    {
       
        [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);
        [DllImport("user32.dll")] public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private IPluginHost m_host = null;

        public override bool Initialize(IPluginHost host)
        {
            m_host = host;

            AutoType.FilterCompilePre += OnAutoType;

            return true;
        }

        private void OnAutoType(object sender, AutoTypeEventArgs autoTypeEventArgs)
        {
            switchInputMethod();
        }

        private void switchInputMethod()
        {
            IntPtr hwnd = GetForegroundWindow();
            PostMessage(hwnd, 0x0050, IntPtr.Zero, (IntPtr)1033);
        }

        public override void Terminate()
        {
            AutoType.FilterCompilePre -= OnAutoType;
            m_host = null;
        }
    }
}
