using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace corewebapi.Controllers
{
    
    [Authorize(Policy = "Administrator")]  // requiere header Authorization:Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyRGF0YSI6IntcIlVzZXJJZFwiOlwiYjA1ZDJlZmEtZDBjMy00Njg3LWEwMjUtNmQ1Mjg1MDNiMmJiXCIsXCJOYW1lXCI6XCJFZHVhcmRvXCIsXCJSb2xlXCI6XCJBZG1pblwifSIsIm5iZiI6MTUxNjg1NjUyMywiZXhwIjoxNTIyMDQwNTIzLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUxNzYxIiwiYXVkIjoiTXlUZXN0QXBpIn0.beMAnbautEc7ch_9SSu0Amn-cgMWOit6Po0qB5FBEfk
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        readonly DataContext DataContext;
        public ValuesController(DataContext dataContext)
        {
            DataContext=dataContext;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Blog>> Get()
        {
            return DataContext.Blogs.ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
