namespace Gadget.Server.Domain.Enums
{
    public enum ServiceStatus
    {
        Stopped = 1,

        /// <summary>The service is starting. This corresponds to the Win32 <see langword="SERVICE_START_PENDING" /> constant, which is defined as 0x00000002.</summary>
        StartPending = 2,

        /// <summary>The service is stopping. This corresponds to the Win32 <see langword="SERVICE_STOP_PENDING" /> constant, which is defined as 0x00000003.</summary>
        StopPending = 3,

        /// <summary>The service is running. This corresponds to the Win32 <see langword="SERVICE_RUNNING" /> constant, which is defined as 0x00000004.</summary>
        Running = 4,

        /// <summary>The service continue is pending. This corresponds to the Win32 <see langword="SERVICE_CONTINUE_PENDING" /> constant, which is defined as 0x00000005.</summary>
        ContinuePending = 5,

        /// <summary>The service pause is pending. This corresponds to the Win32 <see langword="SERVICE_PAUSE_PENDING" /> constant, which is defined as 0x00000006.</summary>
        PausePending = 6,

        /// <summary>The service is paused. This corresponds to the Win32 <see langword="SERVICE_PAUSED" /> constant, which is defined as 0x00000007.</summary>
        Paused = 7,
    }
}