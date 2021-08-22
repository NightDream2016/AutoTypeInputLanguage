using KeePass.Plugins;
using KeePass.Util;
using KeePass.UI;

using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace AutoTypeInputLanguage
{
    public sealed class AutoTypeInputLanguageExt : Plugin
    {
   
        private const string AutoTypeInputLanguage_KeyboardLayoutId = "AutoTypeInputLanguage_KeyboardLayoutId";

        private const string PluginMenuTitle_English = "Auto Type Input Language";
        private const string PluginMenuTitle_Chinese = "自動輸入時的語言";

        private KeyboardLayout[] keyboardLayouts = KeyboardLayoutUtility.GetSystemKeyboardLayouts();
        private KeyboardLayout inputKeyboardLayout = null;

        private IPluginHost m_host = null;

        #region PluginOverride

        public override bool Initialize(IPluginHost host)
        {
            m_host = host;
            setupInputKeyboardLayout();
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
            reloadKeyboardLayouts();
            setupInputKeyboardLayout();

            string menuTitle = isAppChinese() ? PluginMenuTitle_Chinese : PluginMenuTitle_English;
            ToolStripMenuItem stripMenuItem = new ToolStripMenuItem(menuTitle);

            foreach (KeyboardLayout layout in keyboardLayouts)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Text = layout.LanguageName;
                menuItem.Tag = layout.Id;
                menuItem.Click += this.OnMenuItemClicked;
                stripMenuItem.DropDownItems.Add(menuItem);
            }

            stripMenuItem.DropDownOpening += delegate (object sender, EventArgs e)
            {
                foreach (ToolStripMenuItem menuItem in stripMenuItem.DropDownItems)
                {
                    UInt32 layoutIdFromItem = (UInt32)menuItem.Tag;
                    if (layoutIdFromItem == inputKeyboardLayout.Id)
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
            var selectedKeyboardLayoutId = (UInt32)item.Tag;

            foreach (KeyboardLayout layout in keyboardLayouts)
            {
                if (layout.Id == selectedKeyboardLayoutId)
                {
                    inputKeyboardLayout = layout;
                    break;
                }
            }

            setKeyboardLayoutIdToConfig(selectedKeyboardLayoutId);
        }

        #endregion

        #region Switch Input Langauge Flow

        private void switchInputLanguage()
        {
            if (inputKeyboardLayout != null)
            {
                KeyboardLayoutUtility.switchForegroundWindowKeyboardLayout(inputKeyboardLayout);
            }
        }

        #endregion

        #region Setup

        private KeyboardLayout defaultKeyboardLayout()
        {
            var defaultLayout = keyboardLayouts[0];

            foreach (KeyboardLayout layout in keyboardLayouts)
            {
                if (layout.Id == KeyboardLayoutUtility.EnglishKeyboardLayoutID)
                {
                    defaultLayout = layout;
                    break;
                }
            }

            return defaultLayout;
        }

        private void setupInputKeyboardLayout()
        {
            var keyboardLayoutId = configKeyboardLayoutId();
 
            foreach (KeyboardLayout layout in keyboardLayouts)
            {
                if (layout.Id == keyboardLayoutId)
                {
                    inputKeyboardLayout = layout;
                    break;
                }
            }

            if (inputKeyboardLayout == null)
            {
                inputKeyboardLayout = defaultKeyboardLayout();
                setKeyboardLayoutIdToConfig(inputKeyboardLayout.Id);
            }
        }

        private void reloadKeyboardLayouts()
        {
            keyboardLayouts = KeyboardLayoutUtility.GetSystemKeyboardLayouts();
        }

        #endregion

        #region ConfigSetting

        private UInt32 configKeyboardLayoutId()
        {
            UInt32 layoutId = (UInt32)m_host.CustomConfig.GetULong(AutoTypeInputLanguage_KeyboardLayoutId, default(ulong));
            return layoutId;
        }

        private void setKeyboardLayoutIdToConfig(UInt32 layoutId)
        {
            m_host.CustomConfig.SetULong(AutoTypeInputLanguage_KeyboardLayoutId, (ulong)layoutId);
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

        #endregion


    }
}
