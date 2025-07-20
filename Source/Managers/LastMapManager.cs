using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    internal static class LastMapManager
    {
        static internal int lastMapId = -1;

        internal static void ForceInit()
        {
            lastMapId = -1;
        }

        //internal static void SetLastMapId(int id)
        //{
        //    lastMapId = id;
        //}
    }
}
