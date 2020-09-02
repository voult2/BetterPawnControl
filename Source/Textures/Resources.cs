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
        public enum Type { work, restrict, assign, animal };
        public static readonly Texture2D Settings = 
            ContentFinder<Texture2D>.Get("UI/Buttons/Settings");
        public static readonly Texture2D EmergencyOn =
            ContentFinder<Texture2D>.Get("UI/Buttons/EmergencyOn");
        public static readonly Texture2D EmergencyOff =
            ContentFinder<Texture2D>.Get("UI/Buttons/EmergencyOff");
    }
}
