using System;
using System.Threading.Tasks;
using NServiceBus;

namespace Sample.NHibernate.Outbox
{
    internal class Sender : IWantToRunWhenBusStartsAndStops
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            ConsoleKey key;

            while ((key = Console.ReadKey(true).Key) != ConsoleKey.Escape)
            {
                if (key == ConsoleKey.Enter)
                {
                    OrderCompleteHandler.TimeStarted = DateTime.UtcNow;
                    OrderCompleteHandler.NumExpectedMessages = 1000;
                    OrderCompleteHandler.MessagesCounter = 0;

                    Parallel.For(0, OrderCompleteHandler.NumExpectedMessages, i =>
                        Bus.SendLocal(new NewOrder
                        {
                            OrderId = GenerateCombGuid(),
                            Product = "iPhone 4S",
                            Quantity = 5
                        }));
                    Console.Out.WriteLine("Orders placed");
                }
            }
        }

        public void Stop()
        {
        }

        static Guid GenerateCombGuid()
        {
            var guidArray = Guid.NewGuid().ToByteArray();

            var baseDate = new DateTime(1900, 1, 1);
            var now = DateTime.Now;

            // Get the days and milliseconds which will be used to build the byte string 
            var days = new TimeSpan(now.Ticks - baseDate.Ticks);
            var timeOfDay = now.TimeOfDay;

            // Convert to a byte array 
            // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
            var daysArray = BitConverter.GetBytes(days.Days);
            var millisecondArray = BitConverter.GetBytes((long)(timeOfDay.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering 
            Array.Reverse(daysArray);
            Array.Reverse(millisecondArray);

            // Copy the bytes into the guid 
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(millisecondArray, millisecondArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }
    }
}