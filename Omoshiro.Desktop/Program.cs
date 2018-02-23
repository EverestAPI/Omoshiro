using System;
using Eto;
using Eto.Forms;

namespace Omoshiro.Desktop {
    public class Program {
        [STAThread]
        public static void Main(string[] args) {
            Platform platform = Platform.Detect;
            new Application(platform).Run(new MainForm());
        }
    }
}
