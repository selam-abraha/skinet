using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;

namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductsController (IProductRepository repo):ControllerBase
{
  
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort ){
        return Ok(await repo.GetProductsAsync(brand, type, sort));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id){
        var product= await repo.GetProductByIdAsync(id);

        if (product == null ) return NotFound();

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product){
        repo.AddProduct(product);
        if(await repo.SaveChangesAsync()){
            return CreatedAtAction("GetProduct", new{id=product.Id}, product);
        }
        return BadRequest("Problem creating product.");
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
       repo.UpdateProduct(product);

        if (await repo.SaveChangesAsync()){
            return NoContent();
        }

        return BadRequest("Problem updating product");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct (int id){
        var product = await repo.GetProductByIdAsync(id);
        if(product ==null) return NotFound();
        repo.DeleteProduct(product);
        if(await repo.SaveChangesAsync()){
            return NoContent();
        }
        return BadRequest("Problem deleting product.");
    }
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands(){
        return Ok(await repo.GetBrandsAsync());
    }
       [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes(){
        return Ok(await repo.GetTypesAsync());
    }
    private bool ProductExists(int id){
        return repo.ProductExists(id);
    }
}
