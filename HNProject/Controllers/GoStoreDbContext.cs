using System.Collections.Generic;

namespace HNProject.Controllers
{
    public class GoStoreDbContext
    {
        public IEnumerable<object> Orders { get; internal set; }
    }
}