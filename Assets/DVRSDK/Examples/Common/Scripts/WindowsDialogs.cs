﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class WindowsDialogs
{
#if UNITY_STANDALONE_WIN
    #region GetOpenFileName
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenFileName
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

    public static bool GetOpenFileName1([In, Out] OpenFileName ofn)
    {
        return GetOpenFileName(ofn);
    }

    static string Filter(params string[] filters)
    {
        return string.Join("\0", filters) + "\0";
    }

    public static string OpenFileDialog(string title, string extension)
    {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = Filter("All Files", "*.*", extension, "*" + extension);
        ofn.filterIndex = 2;
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = UnityEngine.Application.dataPath;
        ofn.title = title;
        ofn.defExt = "PNG";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 /*| 0x00000200*/ | 0x00000008; // OFN_EXPLORER | OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST | OFN_ALLOWMULTISELECT | OFN_NOCHANGEDIR
        if (!GetOpenFileName(ofn))
        {
            return null;
            //FileDialogResult("file:///" + ofn.file);
        }        // later, possibly in some other method:

        return ofn.file;
    }

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
    public static string SaveFileDialog(string title, string extension)
    {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = Filter("All Files", "*.*", extension, "*" + extension);
        ofn.filterIndex = 2;
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = UnityEngine.Application.dataPath;
        ofn.title = title;
        ofn.defExt = "PNG";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 /*| 0x00000200*/ | 0x00000008 | 0x00000002; // OFN_EXPLORER | OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT | OFN_NOCHANGEDIR | OFN_OVERWRITEPROMPT
        if (!GetSaveFileName(ofn))
        {
            return null;
            //FileDialogResult("file:///" + ofn.file);
        }        // later, possibly in some other method:

        return ofn.file;
    }
    #endregion
#else
    // Windows環境以外でビルドエラーにならない為の事故防止
    public static string OpenFileDialog(string title, string extension)
    {
        return null;
    }

    public static string SaveFileDialog(string title, string extension)
    {
        return null;
    }
#endif
}
