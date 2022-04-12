#if DEBUG
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MintyLoader;
using UnityEngine;

namespace MintMod.Interpreter {
    internal class NativeInterpreter : MintSubMod {
        public override string Name => "Native Hooks";
        public override string Description => "Fun Shit";

        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //private delegate bool BoolFn();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool TwoVarFloatFn(float one, float two);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void VoidFn();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void IntVarVoidFn(int img);

        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //private delegate void SceneFn(int buildIndex, [MarshalAs(UnmanagedType.LPStr)] string sceneName);

        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //private delegate void IsBadFn([MarshalAs(UnmanagedType.LPStr)] string mod, [MarshalAs(UnmanagedType.LPStr)] string author);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        private readonly IntPtr _nativeDllPtr;

        private readonly IntVarVoidFn _imgYeet;
        private readonly TwoVarFloatFn _add;

        private T GetExportedFunction<T>(string name) {
            T delegateForFunctionPointer = Marshal.GetDelegateForFunctionPointer<T>(GetProcAddress(_nativeDllPtr, name));
            if (delegateForFunctionPointer == null) {
                var lastWin32Error = Marshal.GetLastWin32Error();
                var ex = new Win32Exception(lastWin32Error);
                ex.Data.Add("LastWin32Error", lastWin32Error);
                throw new Exception("Can't find exported function \"" + name + "\"", ex);
            }
            return delegateForFunctionPointer;
        }

        internal NativeInterpreter() {
            _nativeDllPtr = LoadLibrary("MintyNative.dll");
            if (_nativeDllPtr == IntPtr.Zero) {
                var lastWin32Error = Marshal.GetLastWin32Error();
                throw new Exception("Can't load native DLL: ", new Win32Exception(lastWin32Error) {
                    Data = { { "LastWin32Error", lastWin32Error } }
                });
            }
            //_testCmd = GetExportedFunction<VoidFn>("TestCommand");
            _imgYeet = GetExportedFunction<IntVarVoidFn>("yeet");
            //_add = GetExportedFunction<TwoVarFloatFn>("add");
            //_add(1, 1);
        }

        internal override void OnStart() {
            var f = new NativeInterpreter();
        }

        //internal override void OnUpdate() {
        //    if (Input.GetKeyDown(KeyCode.L)) {
        //        _testCmd();
        //    }
        //}

        internal override void OnApplicationQuit() {
            Con.Msg("Releasing native DLL");
            FreeLibrary(_nativeDllPtr);
        }
    }
}
#endif