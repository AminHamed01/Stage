using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using WebApi.models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        private DataTable SqlInit(string query)
        {
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();

                    myCon.Close();
                }
            }
            return table;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                            select DepartmentId , DepartmentName from dbo.Department";
            var table = SqlInit(query);
            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(Department dep)
        {
            string query = @"
                            insert into dbo.Department (DepartmentName) values( '" + dep.DepartmentName + @"')          
                               ";
            var table = SqlInit(query);
            return new JsonResult("added departement Successfully");
        }




        [HttpPut]
        public JsonResult Put(Department dep)
        {
            string query = @"
                            update dbo.Department set
                                DepartmentName = '" + dep.DepartmentName + @"'
                                where DepartmentId = " + dep.DepartmentId + @"
                               ";

            var table = SqlInit(query);
            return new JsonResult("update Successfully");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                            delete from dbo.Department 
                                where DepartmentId = " + id + @"
                               ";

       
            var table = SqlInit(query);
            return new JsonResult("delete Successfully");
        }
    }
}
