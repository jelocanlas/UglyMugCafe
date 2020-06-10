using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using UglyMug.Models;
using UglyMug.OrderConfig;
using UglyMug.TimerFeatures;

namespace UglyMug.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailController : ControllerBase
    {
        public List<Orders> orders = new List<Orders>() { };
        public IMemoryCache _cache;
        //public MemoryContainer _memCache;

        private readonly IHubContext<OrderHub> _hub;

        //public IEnumerable<Orders> RefreshList()
        //{
        //    orders = _cache.Get<List<Orders>>(CacheKeys.Entry);
        //    return orders;
        //}
        public OrderDetailController(IHubContext<OrderHub> hub, IMemoryCache cache)
        {
            _hub = hub;
            _cache = cache;

            List<Orders> cacheEntry = _cache.GetOrCreate(CacheKeys.Entry, entry =>
            {
                orders.Add(new Orders()
                {
                    OrderNumber = orders.Count() + 1,
                    OrderName = "Jelo",
                    OrderStatus = "Pending",
                    OrderTime = DateTime.Now,
                    Order = new Order() { OrderDetails = "Coffee", OrderQuantity = 1 }
                });

                entry.SlidingExpiration = TimeSpan.FromDays(3);
                return orders;
            });
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(3));

            _cache.Set(CacheKeys.Entry, cacheEntry, cacheEntryOptions);
        }
        public IActionResult Get()
        {
            //orders.Add(new Orders()
            //{
            //    OrderNumber = orders.Count() + 1,
            //    OrderName = "Jelo",
            //    OrderStatus = "Pending",
            //    OrderTime = DateTime.Now,
            //    Order = new Order() { OrderDetails = "Coffee", OrderQuantity = 1 }
            //});
            //orders = _memCache._memoryCache.Get<List<Orders>>(CacheKeys.Entry);
            orders = _cache.Get<List<Orders>>(CacheKeys.Entry);
            var timeManager = new TimerManager(() => _hub.Clients.All.SendAsync("transferorderdata", orders));
            return Ok(new { Message = "Request Completed" });
        }

        //public OrderDetailController(IMemoryCache memoryCache, IHubContext<OrderHub> hub)
        //{
        //    //List<Orders> orders = new List<Orders>() {};
        //    _hub = hub;
        //    _cache = memoryCache;
        //    _memCache = new MemoryContainer(memoryCache);

        //    List<Orders> cacheEntry = _cache.GetOrCreate(CacheKeys.Entry, entry =>
        //    {
        //        orders.Add(new Orders()
        //        {
        //            OrderNumber = orders.Count() + 1,
        //            OrderName = "Jelo",
        //            OrderStatus = "Pending",
        //            OrderTime = DateTime.Now,
        //            Order = new Order() { OrderDetails = "Coffee", OrderQuantity = 1 }
        //        });

        //        entry.SlidingExpiration = TimeSpan.FromDays(3);
        //        return orders;
        //    });
        //    var cacheEntryOptions = new MemoryCacheEntryOptions()
        //    .SetSlidingExpiration(TimeSpan.FromDays(3));

        //    _cache.Set(CacheKeys.Entry, cacheEntry, cacheEntryOptions);
        //    _memCache._memoryCache = _cache;

        //}

        //[HttpGet]
        //public IEnumerable<Orders> GetAllOrders()
        //{
        //    //List<Orders> orders = new List<Orders>() { };

        //    orders = _memCache._memoryCache.Get<List<Orders>>(CacheKeys.Entry);
        //    return orders;
        //}

        [HttpPost]
        public void CreateOrders(Orders order)
        {
            var obj = order;

            orders = _cache.Get<List<Orders>>(CacheKeys.Entry);
            orders.Add(new Orders()
            {
                OrderNumber = orders.Count() + 1,
                OrderName = obj.OrderName,
                OrderStatus = "Pending",
                OrderTime = DateTime.Now,
                Order = new Order() { OrderDetails = obj.Order.OrderDetails, OrderQuantity = obj.Order.OrderQuantity }
            });
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(3));

            _cache.Set(CacheKeys.Entry, orders, cacheEntryOptions);
        }

        [HttpPut]
        [Route("CompleteOrder/{orderNumber:int}")]
        public void CompleteOrder(int orderNumber)
        {
            orders = _cache.Get<List<Orders>>(CacheKeys.Entry);
            foreach (var order in orders.Where(o => o.OrderNumber == orderNumber))
            {
                order.OrderStatus = "Complete";
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(3));

            _cache.Set(CacheKeys.Entry, orders, cacheEntryOptions);
        }

        [HttpPut]
        [Route("CancelOrder/{orderNumber:int}")]
        public void CancelOrder(int orderNumber)
        {
            orders = _cache.Get<List<Orders>>(CacheKeys.Entry);
            foreach (var order in orders.Where(o => o.OrderNumber == orderNumber))
            {
                order.OrderStatus = "Cancel";
            }
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(3));

            _cache.Set(CacheKeys.Entry, orders, cacheEntryOptions);
        }

        [HttpPut]
        [Route("UpdateOrder/{orderNumber:int}")]
        public void UpdateOrder(Orders updatedOrder)
        {
            orders = _cache.Get<List<Orders>>(CacheKeys.Entry);
            foreach (var order in orders.Where(o => o.OrderNumber == updatedOrder.OrderNumber))
            {
                order.OrderNumber = updatedOrder.OrderNumber;
                order.OrderName = updatedOrder.OrderName;
                order.OrderStatus = "Pending";
                order.OrderTime = DateTime.Now;
                order.Order = new Order() { OrderDetails = updatedOrder.Order.OrderDetails, OrderQuantity = updatedOrder.Order.OrderQuantity };
            }
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(3));

            _cache.Set(CacheKeys.Entry, orders, cacheEntryOptions);
        }
    }
}
