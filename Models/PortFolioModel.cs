namespace PortFolio.Models;

public class PortFolioModel
{
    public List<Contact> Contacts { get; set; } = new List<Contact>();
    public List<Link_social> Links_social { get; set; } = new List<Link_social>();
    public List<Education> Educations { get; set; } = new List<Education>();
    public List<Language> Languages { get; set; } = new List<Language>();
    public List<Skill> Skills { get; set; } = new List<Skill>();
    public List<Xp> XPS { get; set; } = new List<Xp>();
    public List<Cert> Certificates { get; set; } = new List<Cert>();
    public List<Project> Proyects { get; set; } = new List<Project>();
}

public class Contact
{
    public string? Name { get; set; }
    public string? Img { get; set; }
    public string? Email { get; set; }
    public string? Profession { get; set; }
    public string? Description { get; set; }
}
public class Link_social
{
    public string? Name {get; set;}
    public string? Link {get; set;}
}
public class Education
{
    public string? Degree { get; set; }
    public string? School { get; set; }

    public string? Duration { get; set; }
    public string? Description { get; set; }
}
public class Language
{
    public string? Name {get; set;}
    public string? Level{get; set;}
}
public class Skill
{
    public string? Name { get; set; }
    public int? Percent { get; set; }
}
public class Xp
{
    public string? Company {get; set;}
    public string? Position {get; set;}
    public string? Duration { get; set; }
    public string? Description {get; set;} 
}

public class Cert
{
    public string? Company { get; set; }
    public string? Name { get; set; }
}

public class Project
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Img { get; set; }
    public string? Link { get; set; }
}