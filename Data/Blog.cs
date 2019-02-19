using System;


// A blog entity for persistence on dbcontext
public class Blog {

    public int ID { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime Date { get; set; }
}