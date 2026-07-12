using Nucumi;

namespace Nucumi
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            using GameRoot game = new();
            game.Run();
        }
    }
}
