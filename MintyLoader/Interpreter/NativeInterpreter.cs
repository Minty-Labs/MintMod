using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MintyLoader;
using Pastel;
using UnityEngine;
using VRC.UI;

namespace MintyLoader.Interpreter {
    internal class NativeInterpreter {
        internal static NativeInterpreter Interpreter;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool BoolFn();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool MultiVarFloatFn(float one, float two, float? three, float? four, float? five, float? six, float? seven);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void VoidFn();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void IntVarVoidFn(long img);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SceneFn(int buildIndex, [MarshalAs(UnmanagedType.LPStr)] string sceneName);

        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //private delegate void IsBadFn([MarshalAs(UnmanagedType.LPStr)] string mod, [MarshalAs(UnmanagedType.LPStr)] string author);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        private readonly IntPtr _nativeDllPtr;

        private readonly IntVarVoidFn _imgYeet;
        //private readonly TwoVarFloatFn Add;

        private T GetExportedFunction<T>(string name) {
            var delegateForFunctionPointer = Marshal.GetDelegateForFunctionPointer<T>(GetProcAddress(_nativeDllPtr, name));

            if (delegateForFunctionPointer != null)
                return delegateForFunctionPointer;

            var lastWin32Error = Marshal.GetLastWin32Error();
            var ex = new Win32Exception(lastWin32Error);
            ex.Data.Add("LastWin32Error", lastWin32Error);
            throw new Exception("Can't find exported function \"" + name + "\"", ex);
        }

        internal static bool AlreadyLoaded;

        internal NativeInterpreter() {
            Interpreter = this;
            if (!AlreadyLoaded) {
                _nativeDllPtr = LoadLibrary("MintyNative.dll");
                AlreadyLoaded = true;
            }
            if (_nativeDllPtr == IntPtr.Zero) {
                var lastWin32Error = Marshal.GetLastWin32Error();
                throw new Exception("Can't load native DLL: ", new Win32Exception(lastWin32Error) {
                    Data = { { "LastWin32Error", lastWin32Error } }
                });
            }
            //_testCmd = GetExportedFunction<VoidFn>("TestCommand");
            _imgYeet = GetExportedFunction<IntVarVoidFn>("yeet");
            //Add = GetExportedFunction<MultiVarFloatFn>("add");
            //Add(1, 1);
        }

        internal static void RunOnStart() {
            var f = new NativeInterpreter();
            MintyLoader.InternalLogger.Msg("Native".Pastel("EBCAFE") + ": Start");
        }

        internal void RemoveAssembly() {
            //_imgYeet(0);
            //MintyLoader.InternalLogger.Msg("Native".Pastel("EBCAFE") + ": Removed");
        }

        internal void RunOnAppQuit() {
            MintyLoader.InternalLogger.Msg("Native".Pastel("EBCAFE") + ": Releasing Assembly");
            FreeLibrary(_nativeDllPtr);
        }
    }
}