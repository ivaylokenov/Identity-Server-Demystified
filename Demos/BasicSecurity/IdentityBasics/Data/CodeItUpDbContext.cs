namespace IdentityBasics.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    // IdentityDbContext contains all the user tables.
    public class CodeItUpDbContext : IdentityDbContext<CodeItUpUser>
    {
        public CodeItUpDbContext(DbContextOptions<CodeItUpDbContext> options)
            : base(options)
        {
        }
    }
}
