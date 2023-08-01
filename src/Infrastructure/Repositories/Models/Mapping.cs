using Domain.Entities;
using Mapster;

namespace Infrastructure.Repositories.Models;

public sealed class Mapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Audit, AuditPoco>().TwoWays();
    }
}