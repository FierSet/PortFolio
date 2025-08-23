using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using PortFolio.Models;

namespace Portfolio.Controllers;

public class PortfolioController : Controller
{
    public static FilesAndOtherViewModel GlobalModel = ModelGlobal();
    public IActionResult Portfolio()
    {
        return View(GlobalModel);
    }

    public IActionResult ChangePartialView(string PartialName)
    {
        return PartialView($"sheets/{PartialName}", GlobalModel.PortFolioModel);
    }

    public IActionResult GetNavbarCard(string PartialName)
    {
        FilesAndOtherViewModel Model = new()
        {
            Partials = GlobalModel.Partials,
            SelectedPartial = PartialName,
            PortFolioModel = GlobalModel.PortFolioModel
        };

        return PartialView("_navbarcard", Model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public static FilesAndOtherViewModel ModelGlobal()
    {
        Transfer _Transfer = new Transfer();

        PortFolioModel PortFolioinfo = new PortFolioModel();

        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "views", "Portfolio/sheets");
        var files = Directory.GetFiles(folderPath)
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToList();

        var contact = _Transfer.Loaddata("SELECT * FROM Users;");

        PortFolioinfo.Contacts = new List<Contact>
        {
            new Contact
            {
                Name = contact[0]["Name"].ToString(),
                Img = contact[0]["Img"].ToString(),
                Email = contact[0]["Email"].ToString(),
                Profession = contact[0]["Profession"].ToString(),
                Description = contact[0]["Description"].ToString()
            }
        };

        var link_social = _Transfer.Loaddata("SELECT * FROM Link_social;");
        PortFolioinfo.Links_social = link_social.Select(e => new Link_social
        {
            Name = e["Name"].ToString(),
            Link = e["Link"].ToString()
        }).ToList();

        var educ = _Transfer.Loaddata("Select * from Education;");
        PortFolioinfo.Educations = educ.Select(e => new Education
        {
            Degree = e["School"].ToString(),
            School = e["Degree"].ToString(),
            Duration = e["Duration"].ToString(),
            Description = e["Description"].ToString()

        }).ToList();

        var Language = _Transfer.Loaddata("Select * from Language;");
        PortFolioinfo.Languages = Language.Select(e => new Language
        {
            Name = e["Name"].ToString(),
            Level = e["lEVEL"].ToString()

        }).ToList();

        var skill = _Transfer.Loaddata("Select * from Skill;");
        PortFolioinfo.Skills = skill.Select(e => new Skill
        {
            Name = e["Name"].ToString(),
            Percent = int.Parse(e["percent"].ToString())

        }).ToList();

        var xp = _Transfer.Loaddata("Select * from Xp;");
        PortFolioinfo.XPS = xp.Select(e => new Xp
        {
            Company = e["Company"].ToString(),
            Position = e["Position"].ToString(),
            Duration = e["Duration"].ToString(),
            Description = e["Description"].ToString()
        }).ToList();

        var Cert = _Transfer.Loaddata("Select * From Cert;");
        PortFolioinfo.Certificates = Cert.Select(e => new Cert
        {
            Company = e["Company"].ToString(),
            Name = e["Name"].ToString()

        }).ToList();

        var project = _Transfer.Loaddata("Select * from Projects;");
        PortFolioinfo.Proyects = project.Select(e => new Project
        {
            Title = e["Title"].ToString(),
            Description = e["Description"].ToString(),
            Img = e["Img"].ToString(),
            Link = e["Link"].ToString()
             
        }).ToList();

        Console.Write(PortFolioinfo.Contacts);

        FilesAndOtherViewModel Model = new()
        {
            Partials = files,
            SelectedPartial = files.FirstOrDefault(),
            PortFolioModel = PortFolioinfo
        };

        return Model;
    }
}
public class FilesAndOtherViewModel
{
    public List<string?>? Partials { get; set; }
    public string? SelectedPartial { get; set; }
    public PortFolioModel? PortFolioModel { get; set; }
}
