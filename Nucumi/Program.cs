using Nucumi;

namespace Nucumi
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            using GameWindow game = new();
            game.Run();
        }
    }
}
