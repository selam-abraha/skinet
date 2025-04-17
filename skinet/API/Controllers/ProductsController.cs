using System;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;

namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductsController :ControllerBase
{
    private readonly StoreContext context;

    public ProductsController(StoreContext context){
        this.context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(){
        return await context.Products.ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProducts(int id){
        var product= await context.Products.FindAsync(id);

        if (product == null ) return NotFound();

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product){
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product){
        if (!ProductExists(id) || product.Id != id)
            return BadRequest("Cannot update the product.");

        /*context: the DbContext representing a session with the database
        Entry(product): gets the trakcing info for product entity and lets you 
        inspect or change the entity's state manually.
        .State = EntityState.Modified: setting the entity as modified so when 
        saveChanges() is called, EFwill generate an Update statement for it
        */
        context.Entry(product).State = EntityState.Modified;

        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct (int id){
        var product = context.Products.Find(id);
        if(product ==null) return NotFound();
        context.Remove(product);
        await context.SaveChangesAsync();
        return NoContent();
    }
    private bool ProductExists(int id){
        return context.Products.Any(x => x.Id==id);
    }
}
