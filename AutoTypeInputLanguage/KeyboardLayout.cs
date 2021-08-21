using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace AutoTypeInputLanguage
{
    public class KeyboardLayout
    {

        public UInt32 Id { get; private set; }

        public UInt16 LanguageId { get; set; }
        public UInt16 KeyboardId { get; set; }

        public string LanguageName { get; set; }
        public string KeyboardName { get; set; }

        public KeyboardLayout(UInt32 id, UInt16 languageId, UInt16 keyboardId, String languageName, String keyboardName)
        {
            this.Id = id;
            this.LanguageId = languageId;
            this.KeyboardId = keyboardId;
            this.LanguageName = languageName;
            this.KeyboardName = keyboardName;
        }

       
    }

    public static class KeyboardLayoutUtility
    {
        public const UInt32 EnglishKeyboardLayoutID = 0x04090409;
        private const UInt32 KLF_SETFORPROCESS = 0x00000100;
        [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);
        [DllImport("user32.dll")] public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")] private static extern IntPtr GetKeyboardLayout(UInt32 idThread);
        [DllImport("user32.dll")] private static extern UInt32 GetKeyboardLayoutList(Int32 nBuff, IntPtr[] lpList);

        private static KeyboardLayout CreateKeyboardLayout(UInt32 keyboardLayoutId)
        {
            var languageId = (UInt16)(keyboardLayoutId & 0xFFFF);
            var keyboardId = (UInt16)(keyboardLayoutId >> 16);

            return new KeyboardLayout(keyboardLayoutId, languageId, keyboardId, GetCultureInfoName(languageId), GetCultureInfoName(keyboardId));
        }

        private static string GetCultureInfoName(UInt16 cultureId)
        {
            return CultureInfo.GetCultureInfo(cultureId).DisplayName;
        }

        public static KeyboardLayout CreateEnglishKeyboardLayout()
        {
            var keyboardLayoutId = EnglishKeyboardLayoutID;
            KeyboardLayout layout = CreateKeyboardLayout(keyboardLayoutId);
            return layout;
        }

        public static void switchForegroundWindowKeyboardLayout(KeyboardLayout layout)
        {
            IntPtr hwnd = GetForegroundWindow();
            PostMessage(hwnd, 0x0050, IntPtr.Zero, (IntPtr)layout.KeyboardId);
        }

        public static KeyboardLayout[] GetSystemKeyboardLayouts()
        {
            var keyboardLayouts = new List<KeyboardLayout>();

            var count = GetKeyboardLayoutList(0, null);

            var keyboardLayoutIds = new IntPtr[count];
            UInt32 layoutCount = GetKeyboardLayoutList(keyboardLayoutIds.Length, keyboardLayoutIds);

            foreach (var keyboardLayoutId in keyboardLayoutIds)
            {
                var keyboardLayout = CreateKeyboardLayout((UInt32)keyboardLayoutId);
                keyboardLayouts.Add(keyboardLayout);
            }

            return keyboardLayouts.ToArray();
        }
    }
    
}
