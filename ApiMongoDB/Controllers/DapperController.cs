using ApiMongoDB.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace ApiMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DapperController : ControllerBase
    {
        //private readonly IConfiguration _configuration;
        //public DapperController(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        //[HttpGet]
        //public async Task<ActionResult> GetLocations()
        //{
        //    var sql = "select top 100 * from locations";

        //    IEnumerable locations; // = new List<LocationModel>();

        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        locations = await connection.QueryAsync<LocationModel>(sql);
        //    }
        //    return Ok(locations);
        //}
    }
}
