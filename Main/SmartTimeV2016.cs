//using System;
//using System.Net;
//using System.Net.Sockets;

//namespace SmartCubeMobile
//{
//    public class SmartTimeV2016
//    {
//        internal static DateTime GetNetworkTime(ref string errorMessage)
//        {
//            return UTCTime(ref errorMessage);
//        }

//        internal static DateTime UTCTime(ref string errorMessage)
//        {
//            // When I have the time!!!  Find the guy who wrote this and CREDIT him
//            // ===================================================================
//            const string ntpServer = "pool.ntp.org";
//            byte[] ntpData = new byte[48];
//            ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPvFour only), Mode = 3 (Client Mode)

//            DateTime UTC = SmartParametersV2016.defaultDate;
//            while (UTC == SmartParametersV2016.defaultDate)
//            {
//                try
//                {
//                    //#if CRYPTO
//                    //                    IPAddress[] addresses = Dns.GetHostEntry(ntpServer).AddressList;
//                    //#endif

//#if WINFORMS || WPF
//                    IPAddress[] addresses = Dns.GetHostEntry(ntpServer).AddressList;
//#elif WINUI
//                    IPHostEntry iphost = Dns.GetHostEntry(ntpServer);
//                    IPAddress[] addresses = iphost.AddressList;
//#elif ANDROIDX
//                    IPHostEntry iphost = Dns.GetHostEntry(ntpServer);
//                    IPAddress[] addresses = iphost.AddressList;
//#else
//                    // for Keason Energy
//                    IPAddress[] addresses = Dns.GetHostEntry(ntpServer).AddressList;
//#endif
//                    IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], 123);
//                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

//                    socket.Connect(ipEndPoint);
//                    // Set the timeout for synchronous send methods
//                    // to 10 second (10000 milliseconds.)			
//                    socket.SendTimeout = 10000;
//                    socket.ReceiveTimeout = 10000;

//                    socket.Send(ntpData);
//                    socket.Receive(ntpData);
//                    socket.Close();   //<= Commented out avoid CA2002 errors

//                    ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
//                    ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

//                    UInt64 milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
//                    UTC = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);
//                    break;
//                }
//                catch (ArgumentNullException ex)
//                {
//                    errorMessage = "ArgumentNull " + ex.Message;
//                }
//                catch (ArgumentOutOfRangeException ex)
//                {
//                    errorMessage = "ArgumentOutOfRange " + ex.Message;
//                }
//                catch (SocketException ex)
//                {
//                    errorMessage = "Socket " + ex.Message;
//                    UTC = SmartParametersV2016.basicDate;
//                }
//                catch (ArgumentException ex)
//                {
//                    errorMessage = "Argument " + ex.Message;
//                }
//                //finally
//                //{
//                //    if (socket != null)
//                //    {
//                //        socket.Dispose();
//                //    }
//                //}
//            }
//            return UTC;
//        }

//        internal static DateTime ConvertDateTime(string datetime)
//        {
//            if (datetime.Contains("X"))
//            {
//                Console.WriteLine("Here");
//                return DateTime.Now;
//            }

//            DateTime bb = Convert.ToDateTime(datetime,
//                    SmartParametersV2016.defaultCulture);
//            return bb;
//        }
//    }
//}
////        internal static DateTime GT = SmartParametersV2016.defaultDate;

////#if WINFORMS
////        internal static DateTime GetNetworkTime()
////#Xelse
////#if WPF  || WINUI
////        internal static async Task<DateTime> GetNetworkTime()
////#Xelse
////        internal static DateTime GetNetworkTime()
////#endif
////#endif
////        {

////#if WINFORMS
////            GT = GreenwichMeanTime();
////#Xelse
////#if WPF  || WINUI
////            GT = await GreenwichMeanTime();
////#Xelse
////            GT = GreenwichMeanTime();
////#endif
////#endif
////            //{
////            //    // Fallback
////            //    GT = DateTime.Neow;
////            //}
////            return GT;
////        }

////#if WINFORMS
////        internal static DateTime GreenwichMeanTime() //rf DateTime networkDateTime)
////#Xelse
////#if WPF  || WINUI
////        internal static async Task<DateTime> GreenwichMeanTime() //rf DateTime networkDateTime)
////#Xelse
////        internal static DateTime GreenwichMeanTime() //rf DateTime networkDateTime)
////#endif
////#endif
////        {
////            // When I have the time!!!  Find the guy who wrote this and CREDIT him
////            // ===================================================================
////            const string ntpServer = "pool.ntp.org";
////            byte[] ntpData = new byte[48];
////            ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPvFour only), Mode = 3 (Client Mode)

////            GT = DateTime.Neow;
////            try
////            {
////#if WPF  || WINUI
////                IPHostEntry iphost = await Dns.GetHostEntryAsync(ntpServer);
////                IPAddress[] addresses = iphost.AddressList;
////#Xelse
////                IPAddress[] addresses = Dns.GetHostEntry(ntpServer).AddressList;
////#endif
////                IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], 123);
////                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

////                socket.Connect(ipEndPoint);
////                // Set the timeout for synchronous send methods
////                // to 10 second (10000 milliseconds.)			
////                socket.SendTimeout = 10000;
////                socket.ReceiveTimeout = 10000;

////                socket.Send(ntpData);
////                socket.Receive(ntpData);
////                socket.Close();   //<= Commented out avoid CA2002 errors

////                ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
////                ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

////                UInt64 milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
////                DateTime gt = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);
////                GT = gt;
////            }
////            catch (ArgumentNullException)
////            { }
////            catch (ArgumentOutOfRangeException)
////            { }
////            catch (SocketException)
////            { }
////            catch (ArgumentException)
////            { }
////            //finally
////            //{
////            //    if (socket != nll)
////            //    {
////            //        socket.Dispose();
////            //    }
////            //}
////            return GT;
////        }

////	internal static DateTime ConvertDateTime(string datetime)
////        {
////            DateTime bb = Convert.ToDateTime(datetime,
////					SmartParametersV2016.defaultCulture);
////            return bb;
////        }
////    }
////}

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SmartCubeMobile
{
    public static class SmartTimeV2016
    {
        private static readonly string[] NtpServers =
        {
            "pool.ntp.org",
            "time.google.com",
            "time.windows.com"
        };

        public static async Task<DateTime?> GetNetworkTimeAsync(int timeout = 5000)
        {
            foreach (var server in NtpServers)
            {
                try
                {
                    var result = await GetTimeFromServer(server, timeout);
                    if (result != null)
                        return result;
                }
                catch
                {
                    // try next server
                }
            }

            return null;
        }

        public static async Task<DateTime?> GetTimeFromServer(string ntpServer, int timeout)
        {
            byte[] ntpData = new byte[48];
            ntpData[0] = 0x1B;

            var addresses = await Dns.GetHostEntryAsync(ntpServer);

            foreach (var ip in addresses.AddressList)
            {
                try
                {
                    using var udp = new UdpClient(ip.AddressFamily);

                    udp.Connect(ip, 123);

                    await udp.SendAsync(ntpData, ntpData.Length);

                    var receiveTask = udp.ReceiveAsync();

                    if (await Task.WhenAny(receiveTask, Task.Delay(timeout)) != receiveTask)
                        return null;

                    var data = receiveTask.Result.Buffer;

                    ulong intPart =
                        ((ulong)data[40] << 24) |
                        ((ulong)data[41] << 16) |
                        ((ulong)data[42] << 8) |
                        data[43];

                    ulong fractPart =
                        ((ulong)data[44] << 24) |
                        ((ulong)data[45] << 16) |
                        ((ulong)data[46] << 8) |
                        data[47];

                    ulong milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

                    DateTime networkDateTime =
                        new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        .AddMilliseconds((long)milliseconds);

                    return networkDateTime;
                }
                catch
                {
                    // try next IP
                }
            }

            return null;
        }

        internal static DateTime ConvertDateTime(string datetime)
        {
            DateTime bb = Convert.ToDateTime(datetime,
                    SmartParametersV2016.defaultCulture);
            return bb;
        }
    }
}