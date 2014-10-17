using System;

namespace LaserHarp
{
    static class Program
    {
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            using (LaserHarpGame game = new LaserHarpGame())
            {
                game.Run();
            }
        }
    }
}

