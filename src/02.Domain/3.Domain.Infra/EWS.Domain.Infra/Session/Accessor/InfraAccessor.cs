using EWS.Infrastructure.Session.Abstract;
using ISequentialService = EWS.Domain.Infra.Service.ISequentialService;

namespace EWS.Domain.Infra.Session.Accessor;

/// <summary>
/// 시스템 공통 Infra Accessor
/// </summary>
public class InfraAccessor : IInfraAccessor
{
    public readonly ISequentialService SequentialService;
    public InfraAccessor(ISequentialService sequentialService)
    {
        SequentialService = sequentialService;
    }
}