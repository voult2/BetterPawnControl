using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    public static class Resources
    {
        public enum Type { animal, assign, restrict };
        public static readonly Texture2D Settings = 
            ContentFinder<Texture2D>.Get("UI/Buttons/Settings");
    }
}
