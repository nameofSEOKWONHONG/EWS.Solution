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
        if (minLimit.xIsNotEmpty())
        {
            rule.MinimumLength(minLimit)
                .WithMessage(this._localizer["LBL00054"].xValue<string>("test"));
        }
        if (maxLimit.xIsNotEmpty())
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

    protected void Must(Expression<Func<T, string>> expression, string expected)
    {
        RuleFor(expression)
            .Must(m => m.Contains(expected))
            .WithMessage(this._localizer[""].xValue<string>($"{expected} 가 포함되어 있지 않습니다."));
    }

    protected void Email(Expression<Func<T, string>> expression)
    {
        RuleFor(expression)
            .EmailAddress()
            .WithMessage(this._localizer[""].xValue<string>("올바른 이메일 주소 형식이 아닙니다."));
    }
}