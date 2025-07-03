using Application.Interfaces;
using Infrastructure.Persistence;


namespace Infrastructure.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly AppDbContext _context;

        public OrganizationService(AppDbContext context)
        {
            _context = context;
        }

        // TODO: Implementera metoder som definieras i interfacet
    }
}
