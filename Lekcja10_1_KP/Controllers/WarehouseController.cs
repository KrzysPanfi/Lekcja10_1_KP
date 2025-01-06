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
        //metoda używana w testowaniu
        [HttpGet("Orders")]
        public IActionResult GetOrders()
        {
            var dane = new _2019sbdContext().Orders;
            return Ok(dane);
        }
        //metoda używana w testowaniu
        [HttpGet("ProductWarehouse")]
        public IActionResult GetProductWarehouse()
        {
            var dane = new _2019sbdContext().ProductWarehouses;
            return Ok(dane);
        }

        [HttpPost("AddProductWarehouse")]
        public IActionResult AddOrder(int Idproduct, int IdWarehouse, int amount, DateTime createdAt)
        {
            //createdAt = DateTime.Now;
            var context = new _2019sbdContext();
            var isproduct = context.Products.Where(p => p.IdProduct == Idproduct).Any();
            var iswarehouse = context.Warehouses.Where(w => w.IdWarehouse == IdWarehouse).Any();
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
            var orderid = -1;
            var order = context.Orders.Where(o => o.IdProduct == Idproduct && o.Amount == amount && o.CreatedAt < createdAt).SingleOrDefault();
            if (order != null)
            {
                order.FulfilledAt = DateTime.Now;
                orderid = order.IdOrder;
            }

            if (orderid < 0)
            {
                return BadRequest("Zamówienie nie istnieje");
            }
            var iscompleted = context.ProductWarehouses.Where(pw => pw.IdOrder == orderid).Any();
            if (!iscompleted)
            {
                return BadRequest("Zamówienie nie zostało zrealizowane");
            }

            ProductWarehouse insert = new ProductWarehouse();
            var product = context.Products.Where(p => p.IdProduct == Idproduct).SingleOrDefault();
            var productwarehousesid = context.ProductWarehouses.ToList().Count;
            if (product != null)
            {
                decimal price = product.Price * amount;
                insert.IdProduct = Idproduct;
                insert.IdWarehouse = IdWarehouse;
                insert.Amount = amount;
                insert.CreatedAt = DateTime.Now;
                insert.Price = price;
                insert.IdOrder = orderid;
                insert.IdProductWarehouse = productwarehousesid;
                context.ProductWarehouses.Add(insert);
            }
            context.SaveChanges();
            return Ok(productwarehousesid);
        }
    }
}

