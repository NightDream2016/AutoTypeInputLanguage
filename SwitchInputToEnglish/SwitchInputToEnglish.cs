using KeePass.Plugins;
using KeePass.Util;
using KeePass.UI;

using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace SwitchInputToEnglish
{

    public sealed class SwitchInputToEnglishExt : Plugin
    {

        private const string ConfigKey_AutoTypeInputMethod = "SwitchInputToEnglish_AutoTypeInputMethod";

        private const string PluginMenuTitle_English = "Auto Type Input Language";
        private const string PluginMenuTitle_Chinese = "輸入法自動切換";

        private Dictionary<UInt32, KeyboardLayout> keyboardLayoutDictionary = new Dictionary<UInt32, KeyboardLayout>();

        private IPluginHost m_host = null;

        #region PluginOverride

        public override bool Initialize(IPluginHost host)
        {
            m_host = host;

            KeyboardLayout[] layouts = KeyboardLayoutUtility.GetSystemKeyboardLayouts();
            foreach (KeyboardLayout layout in layouts)
            {
                keyboardLayoutDictionary.Add(layout.Id, layout);
            }

            AutoType.FilterCompilePre += OnAutoType;

            return true;
        }

        private void OnAutoType(object sender, AutoTypeEventArgs autoTypeEventArgs)
        {
            switchInputMethod();
        }

        public override void Terminate()
        {
            AutoType.FilterCompilePre -= OnAutoType;
            
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

            string menuTitle = isAppChinese() ? PluginMenuTitle_Chinese : PluginMenuTitle_English;
            ToolStripMenuItem stripMenuItem = new ToolStripMenuItem(menuTitle);

            foreach (KeyboardLayout layout in keyboardLayoutList())
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
                    if (layoutIdFromItem == autoTypeInputlayoutId())
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
            UInt32 layoutId = (UInt32)item.Tag;
            setAutoTypeInputLayoutId(layoutId);
        }

        #endregion

        #region Switch Input Method Flow

        private void switchInputMethod()
        {
            UInt32 layoutId = autoTypeInputlayoutId();
            KeyboardLayout layout = keyboardLayoutDictionary[layoutId];
            KeyboardLayoutUtility.switchForegroundWindowKeyboardLayout(layout);
        }

        #endregion

        #region ConfigSetting

        private UInt32 autoTypeInputlayoutId()
        {
            KeyboardLayout defaultLayout = defaultkeyboardLayout();
            UInt32 layoutId = (UInt32)m_host.CustomConfig.GetULong(ConfigKey_AutoTypeInputMethod, defaultLayout.Id);
            return layoutId;
        }

        private void setAutoTypeInputLayoutId(UInt32 layoutId)
        {
            m_host.CustomConfig.SetULong(ConfigKey_AutoTypeInputMethod, layoutId);
        }

        #endregion

        #region KeyboardLayoutManagement

        private KeyboardLayout defaultkeyboardLayout()
        {
            var layoutList = keyboardLayoutList();
            KeyboardLayout defaultLayout = layoutList[0];
            KeyboardLayout englishLayout = KeyboardLayoutUtility.CreateEnglishKeyboardLayout();

            if (keyboardLayoutDictionary.ContainsKey(englishLayout.Id))
            {
                defaultLayout = keyboardLayoutDictionary[englishLayout.Id];
            }

            return defaultLayout;
        }

        private KeyboardLayout[] keyboardLayoutList()
        {
            List<KeyboardLayout> layoutList = keyboardLayoutDictionary.Values.ToList();
            layoutList.Sort((layout1, layout2) => layout1.Id.CompareTo(layout2.Id));

            return layoutList.ToArray();
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
