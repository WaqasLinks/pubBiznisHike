using MYBUSINESS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MYBUSINESS.Controllers
{
    public class DashBoard1Controller : Controller
    {
        private BusinessContext db = new BusinessContext();
        //[NoCache]
        public ActionResult Index()
        {

            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            //var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            //var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);
            DateTime dtStartDate = DateTime.ParseExact("01/11/2001", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dtEndtDate = DateTime.Today;

            ViewBag.Customers = db.Customers;


            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");


            Dashboard1ViewModel dbVM = new Dashboard1ViewModel();
            ///////////////////////////////////////
            List<SO> FilteredSaleOrders;// = db.Customers;
            List<Customer> FilteredCustomers = new List<Customer>();
            foreach (Customer cust in db.Customers)
            {
                FilteredSaleOrders = new List<SO>();
                foreach (SO so in cust.SOes)
                {
                    if (so.Date >= dtStartDate && so.Date <= dtEndtDate)
                    {
                        FilteredSaleOrders.Add(so);
                    }
                }

                if (FilteredSaleOrders.Count > 0)
                {
                    cust.SOes = FilteredSaleOrders;
                    FilteredCustomers.Add(cust);

                }
            }

            dbVM.Customers = FilteredCustomers.OrderBy(x => x.Name).AsQueryable();
            /////////////////

            List<SOD> FilteredSaleOrderDetails;// = db.Customers;
            List<Product> FilteredProducts = new List<Product>();
            foreach (Product prod in db.Products)
            {
                //FilteredSaleOrders = new List<SO>();
                FilteredSaleOrderDetails = new List<SOD>();

                foreach (SOD sod in prod.SODs)
                {
                    if (sod.SO.Date >= dtStartDate && sod.SO.Date <= dtEndtDate)
                    {
                        FilteredSaleOrderDetails.Add(sod);
                    }
                }

                if (FilteredSaleOrderDetails.Count > 0)
                {
                    prod.SODs = FilteredSaleOrderDetails;
                    FilteredProducts.Add(prod);

                }
            }

            //IQueryable<Product> Products = db.Products;
            dbVM.Products = FilteredProducts.OrderBy(x => x.Name).AsQueryable();

            ///////////////////


            dbVM.TotSaleOrders = (decimal)db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate).Sum(o=>o.SaleOrderAmount- o.Discount);
            dbVM.TotPurchaseOrders = (decimal)db.POes.Where(po => po.Date >= dtStartDate && po.Date <= dtEndtDate).Sum(p => p.PurchaseOrderAmount - p.Discount);
            dbVM.TotProducts = db.Products.Where(pr => pr.CreateDate >= dtStartDate && pr.CreateDate <= dtEndtDate).Count();
            dbVM.TotCustomers = db.Customers.Where(cu => cu.CreateDate >= dtStartDate && cu.CreateDate <= dtEndtDate).Count();
            dbVM.TotSuppliers = db.Customers.Where(sp => sp.CreateDate >= dtStartDate && sp.CreateDate <= dtEndtDate).Count();

            /////////////////////
            string SalesAcrordingTo = string.Empty;
            //if (dtStartDate.Date == dtEndtDate.Date)
            //{
            //    //its mean same day
            //    SalesAcrordingTo = "SameDay";

            //}

            //if (SalesAcrordingTo == string.Empty && dtStartDate.Year == dtEndtDate.Year && dtStartDate.Month == dtEndtDate.Month)
            //{
            //    //its mean same month
            //    SalesAcrordingTo = "SameWeek";
            //}
            //////////////////////////////
            

            //if (SalesAcrordingTo == string.Empty && dtStartDate.Year == dtEndtDate.Year && dtStartDate.Month == dtEndtDate.Month)
            //{
                //its mean same month
                SalesAcrordingTo = "SameMonth";

                int days = DateTime.DaysInMonth(dtStartDate.Year, dtStartDate.Month);
                //sOes = sOes.Where(x => x.CustomerId == custId && x.Date >= dtStartDate && x.Date <= dtEndtDate).OrderBy(i => i.SOSerial).AsQueryable();

                FilteredSaleOrders = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate).OrderBy(i => i.Date).ToList();

                for (byte day = 1; day <= days; day++)
                {
                    dbVM.SalesChart.SalesLabelsData.Add(day.ToString());
                    decimal lagend = (decimal)FilteredSaleOrders.Where(x => x.Date.Value.Day == day).Sum(y => y.SaleOrderAmount - y.Discount);
                    dbVM.SalesChart.SalesLegendsData.Add(lagend);

                }
            //}


            ////////////////////////////////////////
            //if (SalesAcrordingTo == string.Empty && dtStartDate.Year == dtEndtDate.Year)
            //{
            //    //its mean same year
            //    SalesAcrordingTo = "SameYear";

            //    int months = 12;//DateTime.DaysInMonth(dtStartDate.Year, dtStartDate.Month);
            //    //sOes = sOes.Where(x => x.CustomerId == custId && x.Date >= dtStartDate && x.Date <= dtEndtDate).OrderBy(i => i.SOSerial).AsQueryable();

            //    FilteredSaleOrders = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate).OrderBy(i => i.Date).ToList();

            //    for (byte month = 1; month <= months; month++)
            //    {
            //        dbVM.SalesChart.SalesLabelsData.Add(month.ToString());
            //        decimal lagend = (decimal)FilteredSaleOrders.Where(x => x.Date.Value.Month == month).Sum(y => y.SaleOrderAmount - y.Discount);
            //        dbVM.SalesChart.SalesLegendsData.Add(lagend);

            //    }
            //}
            ///////////////////////////////
            //if (SalesAcrordingTo == string.Empty)
            //{
            //    //its mean between years
            //    SalesAcrordingTo = "Years";
            //}







            //if (SalesAcrordingTo == "SameMonth")
            //{
               

            //}








            //////////////////////


            //dbVM.SOes = sOes;
            //dbVM.POes = db.POes;

            return View("Dashboard1", dbVM);
            //return View("Dashboard", sOes);

        }
        public ActionResult FilterIndex(string custId, string suppId, string startDate, string endDate)
        {

            DateTime dtStartDate;
            DateTime dtEndtDate;

            if (startDate == string.Empty)
            {
                dtStartDate = DateTime.Parse("1-1-1800");
            }
            else
            {
                dtStartDate = DateTime.Parse(startDate);
            }

            if (endDate == string.Empty)
            {
                dtEndtDate = DateTime.Today.AddDays(1);
            }
            else
            {
                dtEndtDate = DateTime.Parse(endDate);
                dtEndtDate = dtEndtDate.AddDays(1);
            }

            ViewBag.Customers = db.Customers;

            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");

            Dashboard1ViewModel dbVM = new Dashboard1ViewModel();


            IQueryable<SO> selectedSOes = null;
            /////////////////////////////////////////////////////////////////////////////
            List<SO> FilteredSaleOrders;// = db.Customers;
            List<Customer> FilteredCustomers = new List<Customer>();
            foreach (Customer cust in db.Customers)
            {
                FilteredSaleOrders = new List<SO>();
                foreach (SO so in cust.SOes)
                {
                    if (so.Date >= dtStartDate && so.Date <= dtEndtDate)
                    {
                        FilteredSaleOrders.Add(so);
                    }
                }

                if (FilteredSaleOrders.Count > 0)
                {
                    cust.SOes = FilteredSaleOrders;
                    FilteredCustomers.Add(cust);

                }
            }

            dbVM.Customers = FilteredCustomers.AsQueryable();

            ///////////////////////////////////////////////////////////////////////////

            List<SOD> FilteredSaleOrderDetails;// = db.Customers;
            List<Product> FilteredProducts = new List<Product>();
            foreach (Product prod in db.Products)
            {
                //FilteredSaleOrders = new List<SO>();
                FilteredSaleOrderDetails = new List<SOD>();

                foreach (SOD sod in prod.SODs)
                {
                    if (sod.SO.Date >= dtStartDate && sod.SO.Date <= dtEndtDate)
                    {
                        FilteredSaleOrderDetails.Add(sod);
                    }
                }

                if (FilteredSaleOrderDetails.Count > 0)
                {
                    prod.SODs = FilteredSaleOrderDetails;
                    FilteredProducts.Add(prod);

                }
            }

            //IQueryable<Product> Products = db.Products;
            dbVM.Products = FilteredProducts.AsQueryable();

            ///////////////////


            dbVM.TotSaleOrders = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate).Count();
            dbVM.TotPurchaseOrders = db.POes.Where(po => po.Date >= dtStartDate && po.Date <= dtEndtDate).Count();
            dbVM.TotProducts = db.Products.Where(pr => pr.CreateDate >= dtStartDate && pr.CreateDate <= dtEndtDate).Count();
            dbVM.TotCustomers = db.Customers.Where(cu => cu.CreateDate >= dtStartDate && cu.CreateDate <= dtEndtDate).Count();
            dbVM.TotSuppliers = db.Customers.Where(sp => sp.CreateDate >= dtStartDate && sp.CreateDate <= dtEndtDate).Count();

            /////////////////////


            return PartialView("_Dashboard1", dbVM);
            //return View("Some thing went wrong");
        }
        public ActionResult CustomerWiseSale(int custId, string custName)
        {

            //DateTime dtEndtDate = DateTime.Today.AddDays(1);
            //DateTime dtStartDate = dtEndtDate.AddDays(-7);
            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);

            ViewBag.CustomerId = custId;
            ViewBag.CustName = custName;
            //ViewBag.SupplierName = supplierName;//db.Products.FirstOrDefault(x => x.Id == productId).Name;
            ViewBag.Customers = db.Customers;
            //01-Jan-2019

            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");

            IQueryable<SO> sOes = db.SOes;//.Include(s => s.Customer);
            sOes = sOes.Where(x => x.CustomerId == custId && x.Date >= dtStartDate && x.Date <= dtEndtDate).OrderBy(i => i.SOSerial).AsQueryable();
            //foreach (SO itm in sOes)
            //{
            //    //itm.Id = Encryption.Encrypt(itm.Id, "ABCD");
            //    itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "ABCD")));
            //}



            //      unitOfWork.deviceInstanceRepository.Get()
            //.GroupBy(w => new
            //{
            //    DeviceId = w.DeviceId,
            //    Device = w.Device.Name,
            //    Manufacturer = w.Device.Manufacturer,
            //})
            //.Select(s => new
            //{
            //    DeviceId = s.Key.DeviceId,
            //    Name = s.Key.Device,
            //    Manufacturer = s.Key.Manufacturer.Name,
            //    Quantity = s.Sum(x => x.Quantity)
            //})






            return View("Dashboard1", sOes);

            //return View("CustomerWiseSale", sOes.OrderBy(i => i.Date).ToList());
        }
        public ActionResult FilterCustomerWiseSale(string custId, string suppId, string startDate, string endDate)
        {

            /////////////////////////////////////////////////////////////////////////////
            IQueryable<SO> sOes = db.SOes;//.Include(s => s.Customer);
                                          //sOes = sOes.Where(x => x.CustomerId == custId && x.Date >= dtStartDate && x.Date <= dtEndtDate).OrderBy(i => i.Date).OrderBy(i => i.SOSerial).AsQueryable();






            int intCustId;
            DateTime dtStartDate;
            DateTime dtEndtDate;
            IQueryable<SO> selectedSOes = null;
            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
                dtEndtDate = dtEndtDate.AddDays(1);
                endDate = dtEndtDate.ToString();

            }

            if (custId != string.Empty && startDate != string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            if (custId == string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes;//.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers data acornding to start end date
            if (custId == string.Empty && startDate != string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from undefined startdate to this defined enddate
            if (custId != string.Empty && startDate == string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from defined start date to undefined end date
            if (custId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with all dates
            if (custId != string.Empty && startDate == string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customer with defined startdate and undefined end date
            if (custId == string.Empty && startDate != string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers with undifined start date with defined enddate
            if (custId == string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }


            //foreach (SO itm in selectedSOes)
            //{
            //    //itm.Id = Encryption.Encrypt(itm.Id, "ABCD");
            //    itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "ABCD")));
            //}
            //GetTotalBalance(ref selectedSOes);
            //return PartialView("_SelectedSOSR", selectedSOes.OrderByDescending(i => i.Date).ToList());
            //_ProfitGainFromSupplier
            return PartialView("_Dashboard1", selectedSOes.OrderBy(i => i.SOSerial).ToList());
            //return View("Some thing went wrong");
        }
        public ActionResult ProductWiseSale(int productId)
        {
            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);
            ViewBag.ProductId = productId;
            ViewBag.ProductName = db.Products.FirstOrDefault(x => x.Id == productId).Name;
            ViewBag.Customers = db.Customers;
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");

            IQueryable<SO> sOes = db.SOes;//.Include(s => s.Customer);

            //sOes = db.SOes.Where(x => x.SODs.Where(y => y.ProductId == productId));

            List<SOD> lstSODs = db.SODs.Where(x => x.ProductId == productId).ToList();
            List<SO> lstSlectedSO = new List<SO>();
            foreach (SOD lsod in lstSODs)
            {
                //do not add if already added
                if (lstSlectedSO.Where(x => x.Id == lsod.SOId).FirstOrDefault() == null)
                {
                    lstSlectedSO.Add(lsod.SO);
                }
            }

            sOes = lstSlectedSO.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).AsQueryable();
            foreach (SO itm in sOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "ABCD");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "ABCD")));
            }

            return View("ProductWiseSale", sOes.OrderBy(i => i.SOSerial).ToList());
        }
        public ActionResult FilterProductWiseSale(string prodId, string custId, string suppId, string startDate, string endDate)
        {
            int intProdId;
            intProdId = Int32.Parse(prodId);
            IQueryable<SO> sOes = db.SOes;//.Include(s => s.Customer);

            //sOes = db.SOes.Where(x => x.SODs.Where(y => y.ProductId == productId));

            List<SOD> lstSODs = db.SODs.Where(x => x.ProductId == intProdId).ToList();
            List<SO> lstSlectedSO = new List<SO>();
            foreach (SOD lsod in lstSODs)
            {
                //do not add if already added
                if (lstSlectedSO.Where(x => x.Id == lsod.SOId).FirstOrDefault() == null)
                {
                    lstSlectedSO.Add(lsod.SO);
                }
            }
            sOes = lstSlectedSO.AsQueryable();
            //sOes = lstSlectedSO.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).AsQueryable();

            /////////////////////////////////////////////////////////////////////////////
            //IQueryable<SO> sOes = db.SOes;//.Include(s => s.Customer);
            //sOes = sOes.Where(x => x.CustomerId == custId && x.Date >= dtStartDate && x.Date <= dtEndtDate).OrderBy(i => i.Date).OrderBy(i => i.SOSerial).AsQueryable();






            int intCustId;
            DateTime dtStartDate;
            DateTime dtEndtDate;
            IQueryable<SO> selectedSOes = null;
            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
                dtEndtDate = dtEndtDate.AddDays(1);
                endDate = dtEndtDate.ToString();

            }

            if (custId != string.Empty && startDate != string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            if (custId == string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes;//.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers data acornding to start end date
            if (custId == string.Empty && startDate != string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from undefined startdate to this defined enddate
            if (custId != string.Empty && startDate == string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from defined start date to undefined end date
            if (custId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with all dates
            if (custId != string.Empty && startDate == string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customer with defined startdate and undefined end date
            if (custId == string.Empty && startDate != string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers with undifined start date with defined enddate
            if (custId == string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }


            //foreach (SO itm in selectedSOes)
            //{
            //    //itm.Id = Encryption.Encrypt(itm.Id, "ABCD");
            //    itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "ABCD")));
            //}
            //GetTotalBalance(ref selectedSOes);
            //return PartialView("_SelectedSOSR", selectedSOes.OrderByDescending(i => i.Date).ToList());
            //_ProfitGainFromSupplier
            return PartialView("_CustomerWiseSale", selectedSOes.OrderBy(i => i.SOSerial).ToList());
            //return View("Some thing went wrong");
        }

        public ActionResult About()
        {
            ViewBag.Message = "BiznisHike";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
