﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductApi.Models;
using ProductApi.Parser;
using ProductApi.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger) 
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            _logger.LogInformation($"Get request for products");
            try
            {
                string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int userId = Convert.ToInt32(currentUserId);

                return await _productService.GetUserProducts(userId);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogInformation($"Get request failed: {e.Message}");
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Get request failed: {e.Message}");
                return BadRequest();
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                _logger.LogInformation($"Get request for product (id = {id})");

                return await _productService.GetProduct(id);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Get request failed: {e.Message}");
                return StatusCode(500);
            }
        }

        // PUT: api/Products/
        [HttpPut]
        public async Task<ActionResult<Product>> PutProduct([FromBody]string productUrl)
        {
            _logger.LogInformation($"Put request for product (url = {productUrl})");
            try
            {
                string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int userId = Convert.ToInt32(currentUserId);

                var product = await _productService.PutUserProduct(userId, productUrl);
                return product;
            }
            catch (ArgumentNullException e)
            {
                _logger.LogInformation($"Put request failed: No such user, {e.Message}");
                return StatusCode(404);
            }
            catch (ParserException e)
            {
                _logger.LogInformation($"Put request failed: Can't parse product {e.Message}");
                return StatusCode(500);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Put request failed: {e.Message}");
                return StatusCode(500);
            }
        }


        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            _logger.LogInformation($"Delete request for product (id = {id})");
            try
            {
                string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int userId = Convert.ToInt32(currentUserId);
                var product = await _productService.RemoveUserProduct(userId, id);
                return Ok(product);
            }
            catch (ArgumentNullException e)
            {
                _logger.LogInformation($"Delete request failed: No such user, {e.Message}");
                return StatusCode(404);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Delete request failed: {e.Message}");
                return StatusCode(500);
            }
        }
    }
}
