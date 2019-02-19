public static class DBSeeder
{
    static public void AddTestData(DataContext context)
    {
        var blog1 = new Blog
        {
            ID = 1,
            Title = "Titulo Blog I",
            Body = "Body Blog I"
        };

        context.Blogs.Add(blog1);

        var blog2 = new Blog
        {
            ID = 2,
            Title = "Titulo Blog II",
            Body = "Body Blog II"
        };

        context.Blogs.Add(blog2);

        context.SaveChanges();
    }
}