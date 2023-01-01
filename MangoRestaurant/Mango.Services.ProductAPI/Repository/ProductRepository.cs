using AutoMapper;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public ProductRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            Product product = _mapper.Map<ProductDto,Product>(productDto);
            if(product.Id > 0)
            {
                _db.Products.Update(product);
            }
            else
            {
                _db.Products.Add(product);
            }
            await _db.SaveChangesAsync();
            return _mapper.Map<Product, ProductDto>(product);
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            try
            {
                Product Product = await _db.Products.FirstOrDefaultAsync(x => x.Id == productId);
                if (Product == null)
                {
                    return false;
                }
                _db.Products.Remove(Product);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ProductDto> GetProductById(int productId)
        {
            Product Product = await _db.Products.Where(x => x.Id == productId).FirstOrDefaultAsync();
            return _mapper.Map<ProductDto>(Product);
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            List<Product> ProductList = await _db.Products.ToListAsync();
            return _mapper.Map<List<ProductDto>>(ProductList);
        }
    }
}
