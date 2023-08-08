using System.Linq.Expressions;
using EWS.Application.Language;
using eXtensionSharp;
using FluentValidation;

namespace EWS.Domain.Base;

public abstract class JValidatorBase<T> : AbstractValidator<T>
{
    private readonly ILocalizer _localizer;
    protected JValidatorBase(ILocalizer localizer)
    {
        _localizer = localizer;
    }

    protected void NotEmpty<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        RuleFor(expression)
            .NotEmpty()
            .WithMessage(this._localizer["LBL00048"].xValue<string>("test"));
    }

    protected void MinLength(Expression<Func<T, string>> expression, int minLimit)
    {
        var rule = RuleFor(expression);
        rule.MinimumLength(minLimit)
            .WithMessage(this._localizer["LBL00054"].xValue<string>("test"));
    }

    protected void MaxLength(Expression<Func<T, string>> expression, int maxLimit)
    {   
        var rule = RuleFor(expression);
        rule.MaximumLength(maxLimit)
            .WithMessage(this._localizer["LBL00056"].xValue<string>("test"));
    }

    protected void MinMaxLength(Expression<Func<T, string>> expression, int minLimit, int maxLimit)
    {
        var rule = RuleFor(expression);
        if (minLimit.xIsNotEmptyNum())
        {
            rule.MinimumLength(minLimit)
                .WithMessage(this._localizer["LBL00054"].xValue<string>("test"));
        }
        if (maxLimit.xIsNotEmptyNum())
        {
            rule.MaximumLength(maxLimit)
                .WithMessage(this._localizer["LBL00056"].xValue<string>("test"));
        }
    }

    protected void GreaterThan<TProperty>(Expression<Func<T, TProperty>> expression, TProperty limit) 
        where TProperty : IComparable<TProperty>, IComparable
    {
        RuleFor(expression)
            .GreaterThan(limit)
            .WithMessage(this._localizer["LBL00049"].xValue<string>("test"));
    }
    
    protected void LessThan<TProperty>(Expression<Func<T, TProperty>> expression, TProperty limit) 
        where TProperty : IComparable<TProperty>, IComparable
    {
        RuleFor(expression)
            .LessThan(limit)
            .WithMessage(this._localizer["LBL00050"].xValue<string>("test"));
    }
}