﻿using EPiServer.Commerce.Order;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Plugins.Payment;

namespace Foundation.Commerce.Order.Payments
{
    public class GiftCardPaymentGateway : AbstractPaymentGateway, IPaymentPlugin
    {
        public PaymentProcessingResult ProcessPayment(IOrderGroup orderGroup, IPayment payment)
        {
            //TODO
            return PaymentProcessingResult.CreateSuccessfulResult("Gift card processed");
        }

        public override bool ProcessPayment(Payment payment, ref string message)
        {
            var result = ProcessPayment(null, payment);
            message = result.Message;
            return result.IsSuccessful;
        }
    }
}