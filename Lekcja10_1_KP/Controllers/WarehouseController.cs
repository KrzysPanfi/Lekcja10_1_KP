using Lekcja10_1_KP.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text.Json;

namespace lekcja10_KP_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
       // [HttpGet]
        public IActionResult Get()
        {
            var dane = new _2019sbdContext().Warehouses;
            return Ok(dane);
        }
       // [HttpGet("Warehouse")]
        public bool IsWarehouseindb(int id)
        {
            var dane = new _2019sbdContext().Warehouses.Where(w => w.IdWarehouse == id).Any();
            return dane;
        }
      //  [HttpGet("Product")]
        public bool IsProductindb(int id)
        {
            var dane = new _2019sbdContext().Products.Where(p => p.IdProduct == id).Any();
            return dane;
        }
        [HttpPost]
        public IActionResult AddOrder(int Idproduct, int IdWarehouse, int amount, DateTime CreatedAt)
        {
            var isproduct = IsProductindb(Idproduct);
            var iswarehouse = IsWarehouseindb(IdWarehouse);
            if (!isproduct)
            {
                return BadRequest("Produkt nie istnieje");
            }
            if (!iswarehouse)
            {
                return BadRequest("Magazyn nie istnieje");
            }
            if (amount <= 0)
            {
                return BadRequest("Zła ilość");
            }
            var orderid = IsOrder(Idproduct, amount, CreatedAt);
            if (orderid < 0)
            {
                return BadRequest("Zamówienie nie istnieje");
            }
            if (!Iscompleted(orderid))
            {
                return BadRequest("Zamówienie nie zostało zrealizowane");
            }
            Update(orderid);
            return Ok(Insert(IdWarehouse,Idproduct,orderid,amount));
        }
        public int IsOrder(int id, int amount, DateTime createdAt)
        {
            
                var dane = new _2019sbdContext().Orders.Where(o => o.IdProduct == id && o.Amount == amount && o.CreatedAt < createdAt).SingleOrDefault();
            if (!dane.Equals(null)){ 

                return dane.IdOrder;
            }
                return -1;
        }
 
        public bool Iscompleted(int id)
        {
            var dane = new _2019sbdContext().ProductWarehouses.Where(pw => pw.IdOrder == id).Any();
            return dane;
        }
        public  void Update(int Orderid)
        { var context = new _2019sbdContext();
                var dane = context.Orders.Where(o => o.IdOrder == Orderid).SingleOrDefault();
            if (dane != null){
            dane.FulfilledAt = DateTime.Now;
                context.SaveChanges();
            }
        }
        public int Insert(int Warehouseid, int Productid, int Orderid, int Amount)
        {
            var context = new _2019sbdContext();
            ProductWarehouse insert = new ProductWarehouse();
            var product = context.Products.Where(p => p.IdProduct == Productid).SingleOrDefault();
            if (!product.Equals(null))
            {
                decimal price = product.Price * Amount;

                insert.IdProduct = Productid;
                insert.IdWarehouse = Warehouseid;
                insert.Amount = Amount;
                insert.CreatedAt = DateTime.Now;
                insert.Price = price;
                insert.IdOrder = Orderid;
                context.ProductWarehouses.Add(insert);
                context.SaveChanges();
                return insert.IdProductWarehouse;
            }
            return -1;
        }
    }
}
