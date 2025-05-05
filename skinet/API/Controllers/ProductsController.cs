using System;
using System.Reflection.Metadata;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;

namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductsController (IGenericRepository<Product> repo):ControllerBase
{
  
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort ){
        var spec= new ProductSpecification(brand, type, sort);
        var products=await repo.ListAsync(spec);

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id){
        var product= await repo.GetByIdAsync(id);

        if (product == null ) return NotFound();

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product){
        repo.Add(product);
        if(await repo.SaveAllAsync()){
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
       repo.Update(product);

        if (await repo.SaveAllAsync()){
            return NoContent();
        }

        return BadRequest("Problem updating product");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct (int id){
        var product = await repo.GetByIdAsync(id);
        if(product ==null) return NotFound();
        repo.Remove(product);
        if(await repo.SaveAllAsync()){
            return NoContent();
        }
        return BadRequest("Problem deleting product.");
    }
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands(){
        var spec= new BrandListSpecification();
        
        return Ok(await repo.ListAsync(spec));
    }
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes(){
        var spec = new TypeListSpecification();
        return Ok(await repo.ListAsync(spec));
    }
    private bool ProductExists(int id){
        return repo.Exists(id);
    }
}
