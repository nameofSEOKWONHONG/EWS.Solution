using EWS.Entity;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.Service;

public interface ISequentialService
{
    Task<int> ExecuteAsync(DbContext db, ISessionContext context, Sequential request);
}