using System;

namespace Radischevo.Wahha.Web
{
    /// <summary>
    /// Describes the allowed types 
    /// of the HTTP transfer method
    /// </summary>
    [Flags]
    public enum HttpMethod : int
    {
        None =    0x00000000,
        Get =     0x00000001,
        Head =    0x00000010,
        Post =    0x00000100,
        Put =     0x00001000,
        Delete =  0x00010000,
        Options = 0x00100000,
        Trace =   0x01000000,
        Connect = 0x10000000
    }
}