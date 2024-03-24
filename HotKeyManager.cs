using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MyUnits
{
    public class HotKeyManager
    {
        private List<int> registeredHotKeys;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        private IntPtr hWnd;

        public event EventHandler<int> HotKeyPressed;

        public HotKeyManager(IntPtr hWnd)
        {
            this.hWnd = hWnd;
            this.registeredHotKeys = new List<int>();
        }

        public void RegisterHotKey(int id, uint fsModifiers, uint vk)
        {
            if (!RegisterHotKey(hWnd, id, fsModifiers, vk))
            {
                throw new ApplicationException("Failed to register hotkey.");
            }
            else {
                registeredHotKeys.Add(id);
            }
        }

        public void UnregisterHotKey(int id)
        {
            UnregisterHotKey(hWnd, id);
            registeredHotKeys.Remove(id);
        }
        public void UnregisterAllHotKeys()
        {
            foreach (var id in registeredHotKeys.ToList())
            {
                UnregisterHotKey(id);
            }
        }

        public void ProcessHotKey(int hotkeyId)
        {
            HotKeyPressed?.Invoke(this, hotkeyId);
        }
        public void ProcessHotKeyMessage(Message m)
        {
            if (m.Msg == 0x0312)
            {
                int hotkeyId = m.WParam.ToInt32();
                HotKeyPressed?.Invoke(this, hotkeyId);
            }
        }
    }

    public static class HotKeyModifiers
    {
        public const uint MOD_NONE = 0x0000; // 无
        public const uint MOD_ALT = 0x0001; // Alt 键
        public const uint MOD_CONTROL = 0x0002; // Ctrl 键
        public const uint MOD_SHIFT = 0x0004; // Shift 键
        public const uint MOD_WIN = 0x0008; // Windows 键
    }
}


/* 使用
 
private HotKeyManager hotKeyManager;




private void InitializeHotKeys()
        {
            // 实例化 HotKeyManager，传入窗体的句柄
            hotKeyManager = new HotKeyManager(this.Handle);

            // 注册热键 F2，使用 CTRL 修饰键
            hotKeyManager.RegisterHotKey(1, HotKeyModifiers.MOD_ALT, (uint)Keys.F2);

            // 注册热键 F6，不使用修饰键
            hotKeyManager.RegisterHotKey(2, 0, (uint)Keys.F6);

            // 订阅 HotKeyPressed 事件
            hotKeyManager.HotKeyPressed += HotKeyManager_HotKeyPressed;


            this.Activated += (s, e) => hotKeyManager.RegisterHotKey(3, HotKeyModifiers.MOD_CONTROL, (uint)Keys.A);
            this.Deactivate += (s, e) => hotKeyManager.UnregisterHotKey(3);
        }

        private void HotKeyManager_HotKeyPressed(object sender, int hotkeyId)
        {
            Console.WriteLine("bbb");
            // 处理热键事件
            if (hotkeyId == 1)
            {
                // 处理按下了 CTRL + F2 的情况
                MessageBox.Show("CTRL + F2 pressed");
            }
            else if (hotkeyId == 2)
            {
                // 处理按下了 F6 的情况
                MessageBox.Show("F6 pressed");
            }
        }

        protected override void WndProc(ref Message m)
        {
            // 调用基类的 WndProc 方法来处理其他消息
            base.WndProc(ref m);

            // 在消息处理方法中调用 HotKeyManager 的 ProcessHotKeyMessage 方法
            if (hotKeyManager != null)
            {
                hotKeyManager.ProcessHotKeyMessage(m);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 在窗体关闭时取消注册热键
            hotKeyManager.UnregisterHotKey(1);
            hotKeyManager.UnregisterHotKey(2);
        }


*/