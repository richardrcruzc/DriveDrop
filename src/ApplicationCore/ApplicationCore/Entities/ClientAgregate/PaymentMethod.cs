using ApplicationCore.Entities.Helpers;
using ApplicationCore.Execeptions;

using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.ClientAgregate
{
    public class PaymentMethod : Entity
    {
        private string _alias;
        private string _cardNumber;
        private string _securityNumber;
        private string _cardHolderName;
        private DateTime _expiration;

        private int _cardTypeId;
        public HelperTable CardType { get; private set; }


        protected PaymentMethod() { }

        public PaymentMethod(int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
        {

            _cardNumber = !string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new DriveDropException(nameof(cardNumber));
            _securityNumber = !string.IsNullOrWhiteSpace(securityNumber) ? securityNumber : throw new DriveDropException(nameof(securityNumber));
            _cardHolderName = !string.IsNullOrWhiteSpace(cardHolderName) ? cardHolderName : throw new DriveDropException(nameof(cardHolderName));

            if (expiration < DateTime.UtcNow)
            {
                throw new DriveDropException(nameof(expiration));
            }

            _alias = alias;
            _expiration = expiration;
            _cardTypeId = cardTypeId;
        }

        public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
        {
            return _cardTypeId == cardTypeId
                && _cardNumber == cardNumber
                && _expiration == expiration;
        }
    }
}
