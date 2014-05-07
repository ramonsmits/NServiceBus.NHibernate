using System;
using NServiceBus;

namespace Test.NHibernate
{
    class Sender:IWantToRunWhenBusStartsAndStops
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            Guid duplicate = Guid.NewGuid();
            Console.Out.WriteLine("Press Enter to place order");
            string s;
            while ((s = Console.ReadLine()) != null)
            {
                if (s == "d")
                {
                    Bus.SendLocal<NewOrder>(m =>
                    {
                        m.Product = "duplicate";
                        m.Quantity = 50;
                        m.SetHeader(Headers.MessageId, duplicate.ToString());
                    });

                    continue;
                }
                Bus.SendLocal(new NewOrder {Product = "Shinny new car", Quantity = 5});
            }
        }

        public void Stop()
        {
        }
    }
}