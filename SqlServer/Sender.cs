using System;
using NServiceBus;

namespace Test.NHibernate
{
    class Sender:IWantToRunWhenBusStartsAndStops
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            Console.Out.WriteLine("Press Enter to place order");

            while (Console.ReadLine() != null)
            {
                Bus.SendLocal(new NewOrder {Product = "Shinny new car", Quantity = 5});
            }
        }

        public void Stop()
        {
        }
    }
}