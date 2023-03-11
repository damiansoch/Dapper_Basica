using Dapper;
using Dapper_Basick.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace Dapper_Basick.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperheroeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SuperheroeController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperheroes()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<SuperHero> heroes = await SelectAllHeroes(connection);
            return Ok(heroes);
        }


        [HttpGet]
        [Route("{heroId}")]
        public async Task<ActionResult<SuperHero>> GetSuperheroById(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var hero = await connection.QueryFirstAsync<SuperHero>("SELECT * FROM Supercheroes WHERE id = @Id",
                new { Id = heroId });

            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> AddSuperhero(SuperHero supeHero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("INSERT INTO Supercheroes (hero,firstName,lastName) VALUES (@hero,@firstName,@lastName)",supeHero);
            
            return Ok(await SelectAllHeroes(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateSuperhero( SuperHero superHero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("UPDATE Supercheroes " +
                "SET hero = @Hero, firstName = @FirstName, lastName = @LastName " +
                "WHERE id = @Id", superHero);
            return Ok(await SelectAllHeroes(connection));
        }

        [HttpDelete]
        [Route("{heroId}")]
        public async Task<ActionResult<List<SuperHero>>> DeleteSuperhero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("DELETE FROM Supercheroes WHERE id = @Id",
                new {Id = heroId});
            return Ok(await SelectAllHeroes(connection));
        }

        //-------------------------------------------------------------------------------------------------
        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection connection)
        {
            return await connection.QueryAsync<SuperHero>("SELECT * FROM Supercheroes");
        }
    }
}
