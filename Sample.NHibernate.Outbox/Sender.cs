using System;
using NServiceBus;

namespace Sample.NHibernate.Outbox
{
    internal class Sender : IWantToRunWhenBusStartsAndStops
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            var duplicateMessageId = Guid.NewGuid().ToString();

            Console.Out.WriteLine("Initialising duplicate order...");

            var newOrder = new NewOrder
            {
                Product = "iPhone 5C",
                Quantity = 10,
            };
            Bus.SetMessageHeader(newOrder, Headers.MessageId, duplicateMessageId);

            Bus.SendLocal(newOrder);

            Console.Out.WriteLine(@"Commands:
'Enter' to place a new order
'd' to place a duplicate order
'b' to simulate an exception thrown in the saga timeout handler
'l' to list all the orders successfully processed");
            ConsoleKey key;

            while ((key = Console.ReadKey(true).Key) != ConsoleKey.Escape)
            {
                if (key == ConsoleKey.D)
                {
                    newOrder = new NewOrder
                    {
                        Product = "iPhone 5C",
                        Quantity = 10,
                    };
                    Bus.SetMessageHeader(newOrder, Headers.MessageId, duplicateMessageId);

                    Bus.SendLocal(newOrder);

                    Console.Out.WriteLine("Duplicate order placed, this won't work");
                }

                if (key == ConsoleKey.B)
                {
                    newOrder = new NewOrder
                    {
                        Product = "iPhone 6",
                        Quantity = 1,
                    };
                    Bus.SetMessageHeader(newOrder, "ThrowException", Boolean.TrueString);

                    Bus.SendLocal(newOrder);
                    
                    Console.Out.WriteLine("An exception will be thrown in the saga timeout handler");
                }

                if (key == ConsoleKey.L)
                {
                    Bus.SendLocal(new ShowOrders());

                    Console.Out.WriteLine("An exception will be thrown in the saga timeout handler");
                }

                if (key == ConsoleKey.Enter)
                {
                    Bus.SendLocal(new NewOrder {Product = "iPhone 4S", Quantity = 5});
                    Console.Out.WriteLine("Order placed");
                }
            }
        }

        public void Stop()
        {
        }
    }
}