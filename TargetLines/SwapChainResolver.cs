using TeffLib;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;

namespace TargetLines;

internal static unsafe class SwapChainResolver {
    public static IntPtr PresentMethod { get; private set; }
    public static IntPtr ResizeBuffers { get; private set; }

    public static Device* KernelDevice { get; private set; } = (Device*)IntPtr.Zero;
    public static SwapChain* SwapChain { get; private set; } = (SwapChain*)IntPtr.Zero;
    public static void* DXGISwapChain { get; private set; } = (void*)IntPtr.Zero;

    public static List<IntPtr> SwapChainVtbl { get; private set; }

    public static unsafe void Setup() {
        const int maxRetries = 500;
        for (int attempt = 0; attempt < maxRetries; attempt++) {
            KernelDevice = Device.Instance();
            if (KernelDevice == null) {
                Thread.Sleep(10);
                continue;
            }

            SwapChain = KernelDevice->SwapChain;
            if (SwapChain == null) {
                Thread.Sleep(10);
                continue;
            }

            DXGISwapChain = SwapChain->DXGISwapChain;
            if (DXGISwapChain == null) {
                Thread.Sleep(10);
                continue;
            }

            break;
        }

        if (KernelDevice == null || SwapChain == null || DXGISwapChain == null) {
            Service.Logger.Error("Failed to resolve swap chain after max retries");
            return;
        }

        SwapChainVtbl = GetVTblAddresses((IntPtr)DXGISwapChain, Enum.GetValues(typeof(IDXGISwapChainVtbl)).Length);
        PresentMethod = SwapChainVtbl[(int)IDXGISwapChainVtbl.Present];
        ResizeBuffers = SwapChainVtbl[(int)IDXGISwapChainVtbl.ResizeBuffers];
    }

    private static List<IntPtr> GetVTblAddresses(IntPtr pointer, int numberOfMethods) {
        return GetVTblAddresses(pointer, 0, numberOfMethods);
    }

    private static List<IntPtr> GetVTblAddresses(IntPtr pointer, int startIndex, int numberOfMethods) {
        List<IntPtr> vtblAddresses = new List<IntPtr>();
        IntPtr vTable = Marshal.ReadIntPtr(pointer);
        for (var index = startIndex; index < startIndex + numberOfMethods; index++) {
            vtblAddresses.Add(Marshal.ReadIntPtr(vTable, index * IntPtr.Size));
        }

        return vtblAddresses;
    }
}
