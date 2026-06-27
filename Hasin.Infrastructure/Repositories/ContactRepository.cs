using System.Diagnostics.Contracts;
using Hasin.Model.Entities;
using Hasin.Model.Repositories;

namespace Hasin.Infrastructure.Repositories;

public class ContactRepository : InMemoryRepository<Contact>, IContactRepository
{
   
}