using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerCheckApp.Controllers
{
    // Customer entity class
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
     }

     //products entity class
     public class Product
     {
        public int ProductId {get; set; }
        public String Name {get; set; }
        public decimal Price { get; set; }
     }

         // Basket entity class
    public class Basket
    {
        public int BasketID { get; set; }
        public List<Product> Products { get; set; }
    }


    // Database context class
    public class CustomerContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products {get; set; }
        public DbSet<Basket> Baskets { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configuring SQL Server connection string
            optionsBuilder.UseSqlServer("Server=localhost;Database=Harambe;User Id=RegHarambe;Password=RegHarambe;");

        }
    }

    // Customer Controller class
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerContext _context;

        public CustomerController(CustomerContext context)
        {
            _context = context;
        }


        [HttpGet("{id}")]
        public IActionResult GetCustomer(int id)
        {
            // Check if the customer exists in the database
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerID == customerId);

            if (customer != null)
            {
                 //return 200 if customer found
                return Ok(customer);
            }
            else
            {
                //return 404
                return NotFound();;
            }
        }
    }

    // Product controller class
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly CustomerContext _context;

        public ProductController(CustomerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }
    }

    // Basket controller
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly CustomerContext _context;

        public BasketController(CustomerContext context)
        {
            _context = context;
        }

        [HttpPost("{basketId}/add")]
        public IActionResult AddToBasket(int basketId, [FromBody] Product product)
        {
            var basket = _context.Baskets.Include(b => b.Products).FirstOrDefault(b => b.BasketID == basketId);

            if (basket != null)
            {
                basket.Products.Add(product);
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{basketId}/value")]
        public IActionResult CalculateBasketValue(int basketId)
        {
            var basket = _context.Baskets.Include(b => b.Products).FirstOrDefault(b => b.BasketID == basketId);

            if (basket != null)
            {
                decimal value = basket.Products.Sum(p => p.Price);
                return Ok(value);
            }
            else
            {
                return NotFound();
            }
        }
    }

    // Startup class
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Add database context
            services.AddDbContext<CustomerContext>();
        }

        public void Configure(IApplicationBuilder app)
        {
           {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    // Entry point
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
             Console.WriteLine("hello");
        }
    }
    }
}
