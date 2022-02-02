using Bogus;
using CrystalDecisions.CrystalReports.Engine;
using CrystalReportTry.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrystalReportTry.Controllers
{
    public class HomeController : Controller
    {
        ReportContext reportContext = new ReportContext(); 
        public ActionResult Index()
        {

            reportContext.EmployeeInfoes.RemoveRange(reportContext.EmployeeInfoes);
            reportContext.SaveChanges();

            var fakeData = new Faker<EmployeeInfo>()
                .RuleFor(x => x.EmployeeName, f => f.Name.FullName())
                .RuleFor(x=>x.Email,f=>f.Internet.Email())
                .RuleFor(x=>x.Phone,f=>f.Person.Phone)
                .RuleFor(x=>x.Experience,f=>f.Random.Number(1,15));

            var addAll = fakeData.Generate(100);
            reportContext.EmployeeInfoes.AddRange(addAll);
            reportContext.SaveChanges();
            return View(reportContext.EmployeeInfoes.ToList());
        }

        public ActionResult exportReport()
        {
            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Report"), "CrystalReport.rpt"));
            rd.SetDataSource(reportContext.EmployeeInfoes.ToList());
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "Employee_list.pdf");
            }
            catch
            {
                throw;
            }
        }
    }
}