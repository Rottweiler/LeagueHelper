using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

/// <summary>
/// System-wide event listener (hWnd changes)
/// Created by Rottweiler @ www.hackhound.org
/// 
/// Originally created in order to track movement of other windows
/// </summary>
class EventListener
{
    public enum WinEvents : uint
    {
        EVENT_OBJECT_ACCELERATORCHANGE = 0x8012,
        EVENT_OBJECT_CONTENTSCROLLED = 0x8015,
        EVENT_OBJECT_CREATE = 0x8000,
        EVENT_OBJECT_DEFACTIONCHANGE = 0x8011,
        EVENT_OBJECT_DESCRIPTIONCHANGE = 0x800D,
        EVENT_OBJECT_DESTROY = 0x8001,
        EVENT_OBJECT_DRAGSTART = 0x8021,
        EVENT_OBJECT_DRAGCANCEL = 0x8022,
        EVENT_OBJECT_DRAGCOMPLETE = 0x8023,
        EVENT_OBJECT_DRAGENTER = 0x8024,
        EVENT_OBJECT_DRAGLEAVE = 0x8025,
        EVENT_OBJECT_DRAGDROPPED = 0x8026,
        EVENT_OBJECT_END = 0x80FF,
        EVENT_OBJECT_FOCUS = 0x8005,
        EVENT_OBJECT_HELPCHANGE = 0x8010,
        EVENT_OBJECT_HIDE = 0x8003,
        EVENT_OBJECT_IME_SHOW = 0x8027,
        EVENT_OBJECT_IME_CHANGE = 0x8029,
        EVENT_OBJECT_INVOKED = 0x8013,
        EVENT_OBJECT_LIVEREGIONCHANGED = 0x8019,
        EVENT_OBJECT_LOCATIONCHANGE = 0x800B,
        EVENT_OBJECT_PARENTCHANGE = 0x800F,
        EVENT_OBJECT_REORDER = 0x8004,
        EVENT_OBJECT_SELECTION = 0x8006,
        EVENT_OBJECT_SELECTIONADD = 0x8007,
        EVENT_OBJECT_SELECTIONREMOVE = 0x8008,
        EVENT_OBJECT_SELECTIONWITHIN = 0x8009,
        EVENT_OBJECT_SHOW = 0x8002,
        EVENT_OBJECT_TEXTEDIT_CONVERSIONTARGETCHANGED = 0x8030,
        EVENT_OBJECT_TEXTSELECTIONCHANGED = 0x8014,
        EVENT_OBJECT_VALUECHANGE = 0x800E,
        EVENT_SYSTEM_ALERT = 0x0002,
        EVENT_SYSTEM_ARRANGMENTPREVIEW = 0x8016,
        EVENT_SYSTEM_CAPTUREEND = 0x0009,
        EVENT_SYSTEM_CAPTURESTART = 0x0008,
        EVENT_SYSTEM_CONTEXTHELPEND = 0x000D,
        EVENT_SYSTEM_CONTEXTHELPSTART = 0x000C,
        EVENT_SYSTEM_DESKTOPSWITCH = 0x0020,
        EVENT_SYSTEM_DIALOGEND = 0x0011,
        EVENT_SYSTEM_DIALOGSTART = 0x0010,
        EVENT_SYSTEM_DRAGDROPEND = 0x000F,
        EVENT_SYSTEM_DRAGDROPSTART = 0x000E,
        EVENT_SYSTEM_END = 0x00FF,
        EVENT_SYSTEM_FOREGROUND = 0x0003,
        EVENT_SYSTEM_MENUPOPUPEND = 0x0007,
        EVENT_SYSTEM_MENUPOPUPSTART = 0x0006,
        EVENT_SYSTEM_MENUEND = 0x0005,
        EVENT_SYSTEM_MENUSTART = 0x0004,
        EVENT_SYSTEM_MINIMIZESTART = 0x0016,
        EVENT_SYSTEM_MOVESIZEEND = 0x000B,
        EVENT_SYSTEM_MOVESIZESTART = 0x000A,
        EVENT_SYSTEM_SCROLLINGEND = 0x0013,
        EVENT_SYSTEM_SCROLLINGSTART = 0x0012,
        EVENT_SYSTEM_SOUND = 0x0001,
        EVENT_SYSTEM_SWITCHEND = 0x0015
    }

    [DllImport("user32.dll")]
    private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    private const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
    private const uint WINEVENT_OUTOFCONTEXT = 0;

    public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    public event WinEventDelegate HookedFunctionCallback;

    private List<IntPtr> hooks;
    private List<WinEventDelegate> delegates;

    public EventListener()
    {
        hooks = new List<IntPtr>();
        delegates = new List<WinEventDelegate>();
    }

    public bool Hook(uint EVENT_OBJECT)
    {
        WinEventDelegate del = new WinEventDelegate(WinEventProc);
        IntPtr hook = SetWinEventHook(EVENT_OBJECT, EVENT_OBJECT, IntPtr.Zero, del, 0, 0, 0);
        bool result = (hook != IntPtr.Zero);
        if (result)
        {
            delegates.Add(del);
            hooks.Add(hook);
        }
        return result;
    }

    public bool Hook(WinEvents EVENT_OBJECT)
    {
        return Hook((uint)EVENT_OBJECT);
    }

    public void Reset()
    {
        foreach(IntPtr hook in hooks)
        {
            if (hook != IntPtr.Zero )
            {
                UnhookWinEvent(hook);
            }
        }
    }

    private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (idObject != 0 || idChild != 0)
        {
            return;
        }
        HookedFunctionCallback(hWinEventHook, eventType, hwnd, idObject, idChild, dwEventThread, dwmsEventTime);
    }
}