using Epic.OnlineServices.P2P;
using System.Collections.Generic;
using System.Reflection;

namespace OuterRelics
{
    public class LogicTokenizer
    {
        public static bool TestConditions(List<LogicConditions> conditions)
        {
            bool conditionIsTrue = false;
            foreach (LogicConditions condition in conditions)
            {
                if (GetBoolFromString(condition.condition))
                {
                    conditionIsTrue = true;
                    break;
                }
            }

            return conditionIsTrue;
        }

        public static bool GetBoolFromString(List<string> condition)
        {
            LogicFunctions funcs = new LogicFunctions();
            foreach (string str in condition)
            {
                MethodInfo method = typeof(LogicFunctions).GetMethod(str);
                if (method == null)
                {
                    OuterRelics.Main.LogError("Was unable to find condiiton " + str + " in logic functions, returning true as a fallback");
                    return true;
                }
                if (!(bool)method.Invoke(funcs, null)) return false;
            }
            return true;
        }
    }
}