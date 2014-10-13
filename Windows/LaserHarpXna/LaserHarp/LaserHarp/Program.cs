using System;

namespace LaserHarp
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (LaserHarpGame game = new LaserHarpGame())
            {
                game.Run();
            }
        }
    }
}

