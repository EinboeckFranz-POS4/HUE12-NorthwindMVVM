using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using MvvmTools;
using NorthwindDb;

namespace NorthwindViewModelLib;

public class MainWindowViewModel: ObservableObject
{
    private NorthwindContext? _db;

    private DateTime _minDate = DateTime.Now;
    public DateTime MinDate
    {
        get => _minDate;
        set
        {
            _minDate = value;
            Orders = _db?.Orders
                .Include(x => x.OrderDetails)
                .Where(x => x.OrderDate >= MinDate)
                .ToList() ?? new List<Order>();
            NotifyPropertyChanged(nameof(MinDate));
        }
    }
    
    private List<Order> _orders = new();
    public List<Order> Orders
    {
        get => _orders;
        set
        {
            _orders = value;
            NotifyPropertyChanged(nameof(Orders));
        }
    }
    
    private string _currentOrderDate = "";
    public string CurrentOrderDate
    {
        get => _currentOrderDate;
        set
        {
            _currentOrderDate = value;
            NotifyPropertyChanged(nameof(CurrentOrderDate));
        }
    }

    private Order? _selectedOrder;
    public Order? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OrderDetailsOfSelectedOrder = _db?.OrderDetails
                .ToList()
                .Where(x => x.OrderId == SelectedOrder?.OrderId)
                .AsObservableCollection() ?? new ObservableCollection<OrderDetail>();
            CurrentOrderDate = $"{value?.OrderDate:yyyy-MM-dd}";
        }
    }
    
    private ObservableCollection<OrderDetail> _orderDetailsOfSelectedOrder = new();
    public ObservableCollection<OrderDetail> OrderDetailsOfSelectedOrder
    {
        get => _orderDetailsOfSelectedOrder;
        set
        {
            _orderDetailsOfSelectedOrder = value;
            NotifyPropertyChanged(nameof(OrderDetailsOfSelectedOrder));
        }
    }

    #region Selected OrderDetail Properties
    private OrderDetail? _selectedOrderDetail;
    public OrderDetail? SelectedOrderDetail
    {
        get => _selectedOrderDetail;
        set
        {
            _selectedOrderDetail = value;
            
            var product = _db?.Products.Include(x => x.Supplier).First(x => value != null && x.ProductId == value.ProductId);
            OrderDetailProductName = product?.ProductName ?? "";
            OrderDetailSupplierName = product?.Supplier?.CompanyName ?? "";
        }
    }
    
    private string _orderDetailProductName = "";
    public string OrderDetailProductName
    {
        get => _orderDetailProductName;
        set
        {
            if (_db?.Products.ToList().All(x => x.ProductName != value) ?? false)
            {
                var product = _db.Products.SingleOrDefault(x => x.ProductName == _orderDetailProductName);
                if (product != null)
                {
                    product.ProductName = value;
                    _db.SaveChanges();
                }
                SelectedOrderDetail = SelectedOrderDetail;
                ReloadProducts();
            }
            _orderDetailProductName = value;
            NotifyPropertyChanged(nameof(OrderDetailProductName));
        }
    }

    private string _orderDetailSupplierName = "";

    public string OrderDetailSupplierName
    {
        get => _orderDetailSupplierName;
        set
        {
            if (_db?.Suppliers.ToList().All(x => x.CompanyName != value) ?? false)
            {
                var supplier = _db.Suppliers.SingleOrDefault(x => x.CompanyName == _orderDetailSupplierName);
                if (supplier != null)
                {
                    supplier.CompanyName = value;
                    _db.SaveChanges();
                }
                SelectedOrderDetail = SelectedOrderDetail;
                ReloadProducts();
            }

            _orderDetailSupplierName = value;
            NotifyPropertyChanged(nameof(OrderDetailSupplierName));
        }
    }

    private void ReloadProducts() 
        => Products = _db?.Products.Include(x => x.Category).AsObservableCollection() ?? new ObservableCollection<Product>();

    #endregion

    #region Add OrderDetail Properties
    private ObservableCollection<Product> _products = new();
    public ObservableCollection<Product> Products
    {
        get => _products;
        set
        {
            _products = value;
            NotifyPropertyChanged(nameof(Products));
        }
    }
    
    private int _orderDetailQuantity;
    public int OrderDetailQuantity
    {
        get => _orderDetailQuantity;
        set
        {
            _orderDetailQuantity = value;
            NotifyPropertyChanged(nameof(OrderDetailQuantity));
        }
    }

    public Product? SelectedProduct { get; set; }
    #endregion
    
    public MainWindowViewModel Init(NorthwindContext db)
    {
        _db = db;
        MinDate = DateTime.ParseExact("01.01.1998", "dd.MM.yyyy", CultureInfo.InvariantCulture);
        Products = _db.Products.Include(x => x.Category).AsObservableCollection();
        return this;
    }

    #region Add OrderDetail Commands
    public ICommand AddOrderDetailCommand 
        => new RelayCommand<OrderDetail>(AddOrderDetail, _ => SelectedOrder != null && SelectedProduct != null && OrderDetailQuantity > 0);

    private void AddOrderDetail(OrderDetail? obj)
    {
        var newProduct = new OrderDetail
        {
            OrderId = SelectedOrder?.OrderId ?? 0,
            ProductId = SelectedProduct?.ProductId ?? 0,
            Discount = 0.0f,
            Quantity = Convert.ToInt16(OrderDetailQuantity),
            UnitPrice = SelectedProduct?.UnitPrice ?? 0
        };
        if (SelectedOrder?.OrderDetails.Contains(newProduct) ?? false) 
            return;

        _db?.OrderDetails.Add(newProduct);
        _db?.SaveChanges();
        SelectedOrder = SelectedOrder;
    }
    #endregion

    #region Quantity Commands
    public ICommand IncreaseQuantity => new RelayCommand<int>(_ => OrderDetailQuantity++);

    public ICommand DecreaseQuantity => new RelayCommand<int>(_ => OrderDetailQuantity = OrderDetailQuantity > 0 ? OrderDetailQuantity - 1 : OrderDetailQuantity);
    #endregion
}