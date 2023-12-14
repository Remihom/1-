using System;
using System.Collections.Generic;
using System.Linq;

public interface IOrder
{
    int Id { get; set; }
    List<IProduct> Products { get; set; }
    decimal TotalPrice { get; }
}
public interface IProduct
{
    int Id { get; set; }
    string Name { get; set; }
    decimal Price { get; set; }
}

public class Product:IProduct
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public override string ToString()
    {
        return $"{Id}: {Name} - ${Price}";
    }
}

public class Sneakers : IProduct
{
    public string Brand { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()} - {Brand} Sneakers";
    }
}

public class Tracksuit : IProduct
{
    public string Size { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()} - Size: {Size}";
    }
}

public class TShirt : IProduct
{
    public string Color { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()} - Color: {Color}";
    }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class Order : IOrder
{
    public int Id { get; set; }
    public List<Product> Products { get; set; }
    public decimal TotalPrice => Products.Sum(p => p.Price);
}

public delegate void OrderPaidEventHandler(int orderId, decimal totalAmount);

public class PaymentService
{
    public event OrderPaidEventHandler OrderPaid;

    public void ProcessPayment(Order order)
    {
        OrderPaid?.Invoke(order.Id, order.TotalPrice);
    }
}

public class SalesManager
{
    public int TotalSales { get; private set; }

    public SalesManager()
    {
        TotalSales = 0;
    }

    public void UpdateTotalSales(int orderId, decimal totalAmount)
    {
        Console.WriteLine($"Order #{orderId} has been paid. Total amount: ${totalAmount}");
        TotalSales++;
        Console.WriteLine($"Total Sales: {TotalSales}");
    }
}

public class ShoppingCart<T>
{
    private List<T> items = new List<T>();

    public void AddItem(T item)
    {
        items.Add(item);
    }

    public void DisplayItems()
    {
        foreach (var item in items)
        {
            Console.WriteLine(item.ToString());
        }
    }

    public List<T> Items => items;
}

public class OnlineStore
{
    private List<Order> orders = new List<Order>();
    private List<Customer> customers = new List<Customer>();
    private PaymentService paymentService = new PaymentService();
    private SalesManager salesManager = new SalesManager();

    public void PlaceOrder(Order order, Customer customer, Action<Order> orderProcessedCallback)
    {
        order.Id = orders.Count + 1;
        orders.Add(order);
        customers.Add(customer);
        orderProcessedCallback?.Invoke(order);
        paymentService.ProcessPayment(order);
    }

    public void DisplayOrders()
    {
        Console.WriteLine("Orders in the store:");
        foreach (var order in orders)
        {
            Console.WriteLine($"Order #{order.Id} - Total Price: ${order.TotalPrice}");
        }
    }

    public void DisplayCustomers()
    {
        Console.WriteLine("Customers in the store:");
        foreach (var customer in customers)
        {
            Console.WriteLine($"Customer #{customer.Id} - Name: {customer.Name}, Email: {customer.Email}");
        }
    }

    public PaymentService PaymentService => paymentService;
    public SalesManager SalesManager => salesManager;
}

class Program
{
    static void Main(string[] args)
    {
        var customer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        var productCart = new ShoppingCart<Product>();
        productCart.AddItem(new Sneakers { Id = 1, Name = "Running Shoes", Price = 99.99m, Brand = "Nike" });
        productCart.AddItem(new Tracksuit { Id = 2, Name = "Sports Tracksuit", Price = 49.99m, Size = "Medium" });
        productCart.AddItem(new TShirt { Id = 3, Name = "Athletic T-Shirt", Price = 29.99m, Color = "Blue" });

        Console.WriteLine("Products in Cart:");
        productCart.DisplayItems();

        var order = new Order
        {
            Products = productCart.Items
        };

        var store = new OnlineStore();
        store.PlaceOrder(order, customer, (processedOrder) =>
        {
            Console.WriteLine($"Order #{processedOrder.Id} has been processed.");
        });

        store.PaymentService.OrderPaid += store.SalesManager.UpdateTotalSales;

        store.DisplayOrders();
        store.DisplayCustomers();

        Console.ReadLine();
    }
}