#if MejbiSomeOtherTIme

using System.Net.NetworkInformation;

public /*static*/ class VvNetworkInfo
{
   public string GatewayIPaddress { get; set; }

   public VvNetworkInfo()
   {
      //IPGlobalProperties theIPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
      //
      //GatewayIPaddress = theIPGlobalProperties.HostName;
      ShowNetworkInterfaces();
   }

public static void ShowNetworkInterfaces()
{
    IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
    NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

    //Console.WriteLine("Interface information for {0}.{1}     ",
    //        computerProperties.HostName, computerProperties.DomainName);
    //if (nics == null || nics.Length < 1)
    //{
    //    Console.WriteLine("  No network interfaces found.");
    //    return;
    //}
    //
    //Console.WriteLine("  Number of interfaces .................... : {0}", nics.Length);
    foreach (NetworkInterface adapter in nics)
    {
        IPInterfaceProperties properties = adapter.GetIPProperties();

        int i = 1;
        int w = 1;
        int a = 1;
    //    Console.WriteLine();
    //    Console.WriteLine(adapter.Description);
    //    Console.WriteLine(String.Empty.PadLeft(adapter.Description.Length,'='));
    //    Console.WriteLine("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
    //    Console.WriteLine("  Physical Address ........................ : {0}", 
    //               adapter.GetPhysicalAddress().ToString());
    //    Console.WriteLine("  Operational status ...................... : {0}", 
    //        adapter.OperationalStatus);
    //    string versions ="";
    //
    //    // Create a display string for the supported IP versions.
    //    if (adapter.Supports(NetworkInterfaceComponent.IPv4))
    //    {
    //         versions = "IPv4";
    //     }
    //    if (adapter.Supports(NetworkInterfaceComponent.IPv6))
    //    {
    //        if (versions.Length > 0)
    //        {
    //            versions += " ";
    //         }
    //        versions += "IPv6";
    //    }
    //    Console.WriteLine("  IP version .............................. : {0}", versions);
    //    ShowIPAddresses(properties);
    //
    //    // The following information is not useful for loopback adapters.
    //    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
    //    {
    //        continue;
    //    }
    //    Console.WriteLine("  DNS suffix .............................. : {0}", 
    //        properties.DnsSuffix);
    //
    //    string label;
    //    if (adapter.Supports(NetworkInterfaceComponent.IPv4))
    //    {
    //        IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
    //        Console.WriteLine("  MTU...................................... : {0}", ipv4.Mtu);
    //        if (ipv4.UsesWins)
    //        {
    //
    //            IPAddressCollection winsServers = properties.WinsServersAddresses;
    //            if (winsServers.Count > 0)
    //            {
    //                label = "  WINS Servers ............................ :";
    //                ShowIPAddresses(label, winsServers);
    //            }
    //        }
        }
    //
    //    Console.WriteLine("  DNS enabled ............................. : {0}", 
    //        properties.IsDnsEnabled);
    //    Console.WriteLine("  Dynamically configured DNS .............. : {0}", 
    //        properties.IsDynamicDnsEnabled);
    //    Console.WriteLine("  Receive Only ............................ : {0}", 
    //        adapter.IsReceiveOnly);
    //    Console.WriteLine("  Multicast ............................... : {0}", 
    //        adapter.SupportsMulticast);
    //    ShowInterfaceStatistics(adapter);
    //
    //    Console.WriteLine();
    }
}
#endif

#if MejbiSomeOtherTIme
using System.Runtime.InteropServices;
using System;

[System.Security.SecuritySafeCritical]
public static class SystemTime
{

   [DllImport("Kernel32.dll")]
   private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

   [DllImport("Kernel32.dll")]
   private extern static /*uint*/ bool SetSystemTime(ref SYSTEMTIME lpSystemTime);

   private struct SYSTEMTIME
   {
      public ushort wYear;
      public ushort wMonth;
      public ushort wDayOfWeek;
      public ushort wDay;
      public ushort wHour;
      public ushort wMinute;
      public ushort wSecond;
      public ushort wMilliseconds;
   }

   // pa kaj nije ovo DateTime.Now() ? 
   public static DateTime VvGetSystemTime()
   {
      // Call the native GetSystemTime method 
      // with the defined structure.
      SYSTEMTIME stime = new SYSTEMTIME();
      GetSystemTime(ref stime);

      DateTime systemDateTime = new DateTime(stime.wYear, stime.wMonth, stime.wDay, stime.wHour, stime.wMinute, stime.wSecond, stime.wMilliseconds);
      //return DateTime.SpecifyKind(systemDateTime, DateTimeKind.Local);
      TimeZone localZone = TimeZone.CurrentTimeZone;
      TimeSpan localOffset = localZone.GetUtcOffset(DateTime.Now);
      return systemDateTime + localOffset;

      //MessageBox.Show("Current Time: " +
      //    stime.wHour.ToString() + ":"
      //    + stime.wMinute.ToString());
   }
   public static void VvSetSystemTime(DateTime newDateTime)
   {
      // Call the native GetSystemTime method 
      // with the defined structure.
      SYSTEMTIME stime = new SYSTEMTIME();

      stime.wYear         = (ushort)newDateTime.Year       ; 
      stime.wMonth        = (ushort)newDateTime.Month      ; 
      stime.wDay          = (ushort)newDateTime.Day        ; 
      stime.wHour         = (ushort)newDateTime.Hour       ; 
      stime.wMinute       = (ushort)newDateTime.Minute     ; 
      stime.wSecond       = (ushort)newDateTime.Second     ; 
      stime.wMilliseconds = (ushort)newDateTime.Millisecond;
      
      SetSystemTime(ref stime);

      //GetSystemTime(ref systime);
      //// Set the system clock ahead one hour.
      //systime.wHour = (ushort)(systime.wHour + 1 % 24);
      //SetSystemTime(ref systime);
      //MessageBox.Show("New time: " + systime.wHour.ToString() + ":"
      //    + systime.wMinute.ToString());
   }
}
#endif
