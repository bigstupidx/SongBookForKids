using System;
using System.Text;
using System.Diagnostics;
using System.Reflection;

public static class D {

    private delegate void LogDelegate(object msg, UnityEngine.Object context = null);
    private static LogDelegate DebugLog = UnityEngine.Debug.Log;
    private static LogDelegate DebugLogWarning = UnityEngine.Debug.LogWarning;
    private static LogDelegate DebugLogError = UnityEngine.Debug.LogError;

    public static string Red(string str) {
        return "<color=red>"+str+"</color>";
    }

    public static string Orange(string str) {
        return "<color=orange>"+str+"</color>";
    }

    public static string Green(string str) {
        return "<color=green>"+str+"</color>";
    }

    #if DISABLE_DLOG
    [Conditional("__NEVER_DEFINED__")]
    #endif
    public static void Log(object msg) {
        WriteLog(new StackFrame(1), DebugLog, msg);
    }

    #if DISABLE_DLOG
    [Conditional("__NEVER_DEFINED__")]
    #endif
    public static void Log(params object[] msg) {
        WriteLog(new StackFrame(1), DebugLog, msg);
    }

    #if DISABLE_DLOG
    [Conditional("__NEVER_DEFINED__")]
    #endif
    public static void Err(object msg) {
        WriteLog(new StackFrame(1), DebugLogError, msg);
    }

    #if DISABLE_DLOG
    [Conditional("__NEVER_DEFINED__")]
    #endif
    public static void Err(params object[] msg) {
        WriteLog(new StackFrame(1), DebugLogError, msg);
    }

    #if DISABLE_DLOG
    [Conditional("__NEVER_DEFINED__")]
    #endif
    public static void Warn(object msg) {
        WriteLog(new StackFrame(1), DebugLogWarning, msg);
    }

    #if DISABLE_DLOG
    [Conditional("__NEVER_DEFINED__")]
    #endif
    public static void Warn(params object[] msg) {
        WriteLog(new StackFrame(1), DebugLogWarning, msg);
    }

    #if DISABLE_DLOG
    [Conditional("__NEVER_DEFINED__")]
    #endif
    private static void WriteLog(StackFrame frame, LogDelegate logMethod, params object[] msg) {
        string[] msgs = new string[msg.Length];
        for (int i=0; i<msg.Length; i++) {
            msgs[i] = msg[i] == null ? "null" : msg[i].ToString();
        }
        WriteLog(frame, logMethod, String.Join(" ", msgs));
    }

    #if DISABLE_DLOG
    [Conditional("__NEVER_DEFINED__")]
    #endif
    private static void WriteLog(StackFrame frame, LogDelegate logMethod, string msg) {
        MethodBase methodBase = frame.GetMethod();
        Type declaringType = methodBase.DeclaringType;

        StringBuilder sb = new StringBuilder();
        string methodName = methodBase.Name;

        if (declaringType.IsGenericType) {
            sb.Append(declaringType.GetGenericTypeDefinition().FullName)
                .Remove(sb.Length - 2, 2);

            Type[] args_a = declaringType.GetGenericArguments();
            if (args_a.Length > 0) {
                sb.Append("<");
            }
            foreach (Type t in args_a) {
                sb.Append(t.Name + ", ");
            }
            sb.Remove(sb.Length - 2, 2);
            if (args_a.Length > 0) {
                sb.Append(">");
            }
        } else {
            sb.Append(declaringType.FullName);
        }

        sb.Append("::"+methodName+" : "+msg);
        logMethod(sb.ToString());
    }

}
