//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Minipay.Application.Commons.Interfaces;

//namespace Minipay.Application.Commons.Interfaces
//{
//    public interface IEventBus
//    // TEevent a generic type instead of object.
//    {
//        //TEvent is just a placeholder for any class that implements the IIntegrationEvent interface... publishing 

//        Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)

//            // whatever replaces TEvent must be a class.
//        where TEvent : class;
//    }
//}

//// ex: authorizepayment publishes an event through this interface => handled by Infrastructure. l