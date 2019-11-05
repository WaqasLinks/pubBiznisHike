using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class Dashboard1ViewModel
    {
        public decimal TotSaleOrders { get; set; }
        public decimal TotPurchaseOrders { get; set; }
        public decimal TotCustomers { get; set; }
        public decimal TotSuppliers { get; set; }
        public decimal TotProducts { get; set; }

        public SalesChart SalesChart { get; set; } = new SalesChart();
        public IQueryable<SO> SOes { get; set; }
        public IQueryable<PO> POes { get; set; }
        public IQueryable<Customer> Customers { get; set; }
        public IQueryable<Supplier> Suppliers { get; set; }
        public IQueryable<Product> Products { get; set; }
        //public IQueryable<CustomersWiseSaleViewModel> CWS { get; set; }


    }
    public class SalesChart
    {
        public List<string> SalesLabelsData { get; set; } = new List<string>();
        public List<decimal> SalesLegendsData { get; set; } = new List<decimal>();

    }


}