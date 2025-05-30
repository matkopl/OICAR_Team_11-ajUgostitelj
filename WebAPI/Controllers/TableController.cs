using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services;
using Serilog;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableService _tableService;

        public TableController(ITableService tableService)
        {
            _tableService = tableService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableDto>>> GetAllTables()
        {
            try
            {
                var tables = await _tableService.GetAllTablesAsync();
                Log.Information("All tables fetched successfully!");
                return Ok(tables);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TableDto>> GetTable(int id)
        {
            try
            {
                var table = await _tableService.GetTableByIdAsync(id);
                if (table == null)
                {
                    Log.Warning($"Table with ID {id} not found");
                    return NotFound();
                }
                return Ok(table);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"Error fetching table with ID {id}");
                return BadRequest($"Error fetching table with ID {id}, please see error log!");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TableDto>> CreateTable(TableDto tableDto)
        {
            try
            {
                Log.Information($"Creating table...");
                var createdTable = await _tableService.CreateTableAsync(tableDto);
                Log.Information($"Successfully created {createdTable.Name} with ID:{createdTable.Id}");
                return CreatedAtAction(nameof(GetTable), new { id = createdTable.Id }, createdTable);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating table");
                return BadRequest($"Error creating table, please see error log!");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTable(int id, TableDto tableDto)
        {
            if (id != tableDto.Id)
            {
                Log.Error($"ID in URL does not match ID in body)", id, tableDto.Id);
                return BadRequest($"ID in URL does not match ID in body");
            }

            try
            {
                Log.Information($"Updating Table with ID {id}...");
                await _tableService.UpdateTableAsync(id, tableDto);
                Log.Information($"Table with ID {id} updated successfully");
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                Log.Error(ex, $"Table with ID {id} not found during update", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error updating table with ID {id}");
                return StatusCode(500, "Error occurred");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            try
            {
                Log.Information($"Deleting table ID:{id}...");
                await _tableService.DeleteTableAsync(id);
                Log.Information($"Successfully deleted table with ID {id}");
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                Log.Error(ex, $"Table with ID {id} not found during deletion");
                return NotFound();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while deleting table with ID {id}");
                return StatusCode(500, "Error with request!");
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult> GetPaged([FromQuery] TableQueryDto query)
        {
            try
            {
                var (tables, totalCount) = await _tableService.GetTablesPagedAsync(query);

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("X-Page-Size", query.PageSize.ToString());
                Response.Headers.Add("X-Current-Page", query.Page.ToString());

                return Ok(tables);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("sort-options")]
        public ActionResult GetSortOptions()
        {
            try
            {
                return Ok(_tableService.GetAvailableSortColumns());
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
