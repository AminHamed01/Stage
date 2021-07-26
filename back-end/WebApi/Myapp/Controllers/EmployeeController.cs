using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using WebApi.models;
using System.IO;
using Microsoft.AspNetCore.Hosting;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
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
                            select EmployeeId , EmployeeName , Department,PhoneNumber,
                            convert (varchar(10),DateOfJoining,120) as DateOfJoining,
                             PhotoFileName
                              from dbo.Employee
                                 ";

            var table = SqlInit(query);
            return new JsonResult(table);
        }
        [HttpPost]
        public JsonResult Post(Employee emp)
        {
            string query = @"
                            insert into dbo.Employee 
                               (EmployeeName, PhotoFileName,DateOfJoining,Department,PhoneNumber)
                                values
                        (
                               '" + emp.EmployeeName + @"'
                              , '" + emp.PhotoFileName + @"'
                               ,'" + emp.DateOfJoining + @"'                         
                              , '" + emp.Department + @"'
                               , '" + emp.PhoneNumber + @"'
                                                                 )
                                                              ";
            var table = SqlInit(query);
            return new JsonResult("added Successfully");
        }




        [HttpPut]
        public JsonResult Put(Employee emp)
        {
            string query = @"
                            update dbo.Employee set
                                EmployeeName= '" + emp.EmployeeName + @"'
                               , DateOfJoining= '" + emp.DateOfJoining + @"'
                                , PhoneNumber= '" + emp.PhoneNumber + @"' 
                               , Department= '" + emp.Department + @"'
                              


                                where EmployeeId = " + emp.EmployeeId + @"
                               ";

            var table = SqlInit(query);
            return new JsonResult("update Successfully");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                            delete from dbo.Employee 
                                where EmployeeId = " + id + @"
                               ";

            var table = SqlInit(query);
            return new JsonResult("deleted Successfully");
        }
        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;
                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                return new JsonResult(filename);
            }
            catch (Exception)
            {

                return new JsonResult("anonymous.png");
            }

        }
        [Route("GetAllDepartmentNames")]
        public JsonResult GetAllDepartmentNames()
        {

            string query = @"
                       select DepartmentName from dbo.Department
                                 ";

            var table = SqlInit(query);
            return new JsonResult(table);
        }

    }
}
