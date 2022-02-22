using System;
using System.Windows.Forms;
using Linearstar.Windows.RawInput;

namespace Assets
{
    class RawInputReceiverWindow : NativeWindow
    {
        public event EventHandler<RawInputEventArgs> Input;

        public RawInputReceiverWindow()
        {
            CreateHandle(new CreateParams
            {
                X = 0,
                Y = 0,
                Width = 0,
                Height = 0,
                Style = 0x800000,
            });
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_INPUT = 0x00FF;

            if (m.Msg == WM_INPUT)
            {
                var data = RawInputData.FromHandle(m.LParam);

                Input?.Invoke(this, new RawInputEventArgs(data));

                // You can identify the source device using Header.DeviceHandle or just Device.

                /*switch (data)
                {
                    case RawInputMouseData mouse:
                        Console.Write("WndProc ");
                        Console.WriteLine(mouse.Mouse);
                        break;
                    case RawInputKeyboardData keyboard:
                        Console.Write("WndProc ");
                        Console.WriteLine(keyboard.Keyboard);
                        break;
                    case RawInputHidData hid:
                        Console.Write("WndProc ");
                        Console.WriteLine(hid.Hid);
                        break;
                }*/
            }

            base.WndProc(ref m);
        }
    }
}
