using System;
using System.Collections.Generic;
using System.Text;
using VictimBot.Lib.Interfaces;

namespace VictimBot.Lib.Helpers
{
    public static class DialogHelper
    {
        public static string GetDialogId(this IVictimBotDialog dialog)
        {
            return dialog.GetType().CalcDialogId();
        }

        public static string CalcDialogId(this Type type)
        {
            return type.FullName;
        }

        public static string GetStepId(this IVictimBotWaterfallStep step)
        {
            return step.GetType().CalcStepId();
        }

        public static string CalcStepId(this Type type)
        {
            return type.FullName;
        }
    }
}
