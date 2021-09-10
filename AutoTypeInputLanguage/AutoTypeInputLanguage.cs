using KeePass.Plugins;
using KeePass.Util;
using KeePass.UI;

using System;
using System.Windows.Forms;
using System.Globalization;
using System.Runtime.InteropServices;

namespace AutoTypeInputLanguage
{
    public sealed class AutoTypeInputLanguageExt : Plugin
    {
        [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const string AutoTypeInputLanguage_InputLanguage = "AutoTypeInputLanguage_InputLanguage";
        private const string PluginMenuTitle_English = "Auto Type Input Language";
        private const string PluginMenuTitle_Chinese = "自動輸入時的語言";

        private InputLanguageCollection installedLanguages = InputLanguage.InstalledInputLanguages;
        private InputLanguage currentInputLanguage = InputLanguage.CurrentInputLanguage;

        private IPluginHost m_host = null;

        #region PluginOverride

        public override bool Initialize(IPluginHost host)
        {
            m_host = host;
            AutoType.FilterSendPre += OnAutoType;

            return true;
        }

        private void OnAutoType(object sender, AutoTypeEventArgs autoTypeEventArgs)
        {
            switchInputLanguage();
        }

        public override string UpdateUrl
        {
            get {
                return "https://raw.githubusercontent.com/NightDream2016/AutoTypeInputLanguage/master/VersionInfo";
            }
        }

        public override void Terminate()
        {
            AutoType.FilterSendPre -= OnAutoType;
            
            m_host = null;
        }

        #endregion

        #region UI

        public override ToolStripMenuItem GetMenuItem(PluginMenuType type)
        {
            // Our menu item below is intended for the main location(s),
            // not for other locations like the group or entry menus
            if (type != PluginMenuType.Main)
            {
                return null;
            }

            // To make the item list sync with system language setting.
            setupInputLanguages();

            string menuTitle = isAppChinese() ? PluginMenuTitle_Chinese : PluginMenuTitle_English;
            ToolStripMenuItem stripMenuItem = new ToolStripMenuItem(menuTitle);

            foreach (InputLanguage language in installedLanguages)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                if (isWindowsVersionHigherThanEight())
                {
                    menuItem.Text = language.Culture.DisplayName;
                }
                else
                {
                    menuItem.Text = language.LayoutName;
                }
                
                menuItem.Tag = language.Handle.ToString();
                menuItem.Click += this.OnMenuItemClicked;
                stripMenuItem.DropDownItems.Add(menuItem);
            }


            stripMenuItem.DropDownOpening += delegate (object sender, EventArgs e)
            {
                foreach (ToolStripMenuItem menuItem in stripMenuItem.DropDownItems)
                {
                    String languageHandleFromItem = (String)menuItem.Tag;
                    if (languageHandleFromItem == currentInputLanguage.Handle.ToString())
                    {
                        UIUtil.SetChecked(menuItem, true);
                    }
                    else
                    {
                        UIUtil.SetChecked(menuItem, false);
                    }
                }
            };

            return stripMenuItem;
        }

        private void OnMenuItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            var selectedLanguageHandle = (String)item.Tag;

            foreach (InputLanguage language in installedLanguages)
            {
                if (language.Handle.ToString() == selectedLanguageHandle)
                {
                    currentInputLanguage = language;
                    break;
                }
            }

            var handle = currentInputLanguage.Handle;
            setInputLanguageHandleToConfig(handle.ToString());
        }

        #endregion

        #region Switch Input Langauge Flow

        private void switchInputLanguage()
        {
            if (currentInputLanguage != null)
            {
                switchForegroundWindowInputLangauge(currentInputLanguage);
            }
        }

        #endregion

        #region Setup

        private InputLanguage defaultInputLanguage()
        {
            var defaultLayout = installedLanguages[0];

            foreach (InputLanguage language in installedLanguages)
            {
                if (language.Culture.Name == "en-US")
                {
                    defaultLayout = language;
                    break;
                }
            }

            return defaultLayout;
        }

        private void setupInputLanguages()
        {
            var languageHandle = configInputLanguageHandle();
 
            foreach (InputLanguage language in installedLanguages)
            {
                if (language.Handle.ToString() == languageHandle)
                {
                    currentInputLanguage = language;
                    break;
                }
            }

            var handle = currentInputLanguage.Handle;
            setInputLanguageHandleToConfig(handle.ToString());
        }

        #endregion

        #region ConfigSetting

        private String configInputLanguageHandle()
        {
            String handleId = m_host.CustomConfig.GetString(AutoTypeInputLanguage_InputLanguage, "");
            return handleId;
        }

        private void setInputLanguageHandleToConfig(String languageHandle)
        {
            m_host.CustomConfig.SetString(AutoTypeInputLanguage_InputLanguage, languageHandle);
        }

        #endregion

        #region Utility

        private bool isAppChinese()
        {
            if (AppLanguage() == "zh-tw")
            {
                return true;
            }

            return false;
        }

        private string AppLanguage()
        {
            var languageCode = KeePass.Program.Translation.Properties.Iso6391Code;
            if (string.IsNullOrEmpty(languageCode))
            {
                languageCode = "en";
            }

            return languageCode;
        }

        public bool isWindowsVersionHigherThanEight()
        {
            var version = System.Environment.OSVersion.Version;
            if (version.Major >= 6 && version.Minor >= 2)
            {
                return true;
            }
            return false;
        }

        public static void switchForegroundWindowInputLangauge(InputLanguage language)
        {
            IntPtr hwnd = GetForegroundWindow();
            PostMessage(hwnd, 0x0050, IntPtr.Zero, language.Handle);
        }


        #endregion


    }
}
