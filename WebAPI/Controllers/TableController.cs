using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services;

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
            var tables = await _tableService.GetAllTablesAsync();
            return Ok(tables);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TableDto>> GetTable(int id)
        {
            var table = await _tableService.GetTableByIdAsync(id);
            return table != null ? Ok(table) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<TableDto>> CreateTable(TableDto tableDto)
        {
            var createdTable = await _tableService.CreateTableAsync(tableDto);
            return CreatedAtAction(nameof(GetTable), new { id = createdTable.Id }, createdTable);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTable(int id, TableDto tableDto)
        {
            if (id != tableDto.Id)
            {
                return BadRequest("ID in URL does not match ID in body");
            }

            try
            {
                await _tableService.UpdateTableAsync(id, tableDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            try
            {
                await _tableService.DeleteTableAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult> GetPaged([FromQuery] TableQueryDto query)
        {
            var (tables, totalCount) = await _tableService.GetTablesPagedAsync(query);

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            Response.Headers.Add("X-Page-Size", query.PageSize.ToString());
            Response.Headers.Add("X-Current-Page", query.Page.ToString());

            return Ok(tables);
        }

        [HttpGet("sort-options")]
        public ActionResult GetSortOptions()
        {
            return Ok(_tableService.GetAvailableSortColumns());
        }
    }
}
