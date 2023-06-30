namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            IPaymentSystem system;

            IPaymentSystemsCreator[] PaymentSystemCreators =
            {
                new QiwiPaymentCreator(),
                new WebMoneyPaymentCreator(),
                new CardPaymentCreator()
            };

            PaymentSystems paymentSystems = new PaymentSystems(PaymentSystemCreators);

            var orderForm = new OrderForm(paymentSystems);
            var paymentHandler = new PaymentHandler(paymentSystems);
            var systemId = orderForm.ShowForm();

            if (paymentSystems.TryFindSystem(systemId, out system) == false)
                return;

            system.ImplementPayment();
            paymentHandler.ShowPaymentResult(systemId);
        }
    }

    public class OrderForm
    {
        private readonly PaymentSystems _system;

        public OrderForm(PaymentSystems paymentSystems)
        {
            _system = paymentSystems;
        }

        public string ShowForm()
        {
            string systemId;
            Console.WriteLine("Мы принимаем: QIWI, WebMoney, Card");

            //симуляция веб интерфейса
            Console.WriteLine("Какой системой вы хотите совершить оплату?");

            do
            {
                systemId = Console.ReadLine();

                if(string.IsNullOrEmpty(systemId) || string.IsNullOrWhiteSpace(systemId))
                    Console.WriteLine("Введите название системы: ");
            }
            while(string.IsNullOrEmpty(systemId) || string.IsNullOrWhiteSpace(systemId));

            return systemId;
        }
    }

    public class PaymentHandler
    {
        private PaymentSystems _paymentSystems;

        public PaymentHandler(PaymentSystems paymentSystems)
        {
            _paymentSystems = paymentSystems;
        }

        public void ShowPaymentResult(string systemId)
        {
            Console.WriteLine($"Вы оплатили с помощью {systemId}");
            Console.WriteLine("Оплата прошла успешно!");
        }
    }

    public class PaymentSystems
    {
        private IPaymentSystemsCreator[] _paymentSystemCreators;
        private List<IPaymentSystem> _paymentSystems;

        public PaymentSystems(IPaymentSystemsCreator[] paymentSystemCreators)
        {
            _paymentSystemCreators = paymentSystemCreators;
            _paymentSystems = new List<IPaymentSystem>();
            CreatePaymentSystems();
        }

        public bool TryFindSystem(string systemId, out IPaymentSystem? system)
        {
            system = _paymentSystems.FirstOrDefault(system => system.Name == systemId);

            return system != null;
        }

        public string GetSystem()
        {
            string result = "";
            string comma = ", ";

            foreach (IPaymentSystem system in _paymentSystems)
            {
                result += system.Name;

                if (system != _paymentSystems[^1])
                    result += comma;
            }

            return result;
        }

        private void CreatePaymentSystems()
        {
            foreach (var paymentSystem in _paymentSystemCreators)
                _paymentSystems.Add(paymentSystem.Create());
        }
    }

    public interface IPaymentSystem
    {
        public string Name { get; }

        public void ImplementPayment();
        public void ShowResult();
    }

    public class QiwiPayment : IPaymentSystem
    {
        public string Name => "QIWI";

        public void ImplementPayment()
        {
            Console.WriteLine("Перевод на страницу QIWI...");
        }

        public void ShowResult()
        {
            Console.WriteLine("Проверка платежа через QIWI...");
        }
    }

    public class WebMoneyPayment : IPaymentSystem
    {
        public string Name => "WebMoney";

        public void ImplementPayment()
        {
            Console.WriteLine("Вызов API WebMoney...");
        }

        public void ShowResult()
        {
            Console.WriteLine("Проверка платежа через WebMoney...");
        }
    }

    public class CardPayment : IPaymentSystem
    {
        public string Name => "Card";

        public void ImplementPayment()
        {
            Console.WriteLine("Вызов API банка эмитера карты Card...");
        }

        public void ShowResult()
        {
            Console.WriteLine("Проверка платежа через Card...");
        }
    }

    public interface IPaymentSystemsCreator
    {
        public IPaymentSystem Create();
    }

    public class QiwiPaymentCreator : IPaymentSystemsCreator
    {
        public IPaymentSystem Create() => new QiwiPayment();
    }

    public class WebMoneyPaymentCreator : IPaymentSystemsCreator
    {
        public IPaymentSystem Create() => new WebMoneyPayment();
    }

    public class CardPaymentCreator : IPaymentSystemsCreator
    {
        public IPaymentSystem Create() => new CardPayment();
    }
}