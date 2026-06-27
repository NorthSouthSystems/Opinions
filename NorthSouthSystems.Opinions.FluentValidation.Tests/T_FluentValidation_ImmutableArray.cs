using FluentValidation.Results;

public class T_FluentValidation_ImmutableArray
{
    public class WithImmArr
    {
        public ImmutableArray<string> ImmArr { get; set; }
        internal ImmutableArray<string> ImmArrSafe => ImmArr.IsDefault ? [] : ImmArr;
    }

    public class WithImmArrNotNullV : AbstractValidator<WithImmArr>
    {
        public WithImmArrNotNullV() =>
            RuleFor(x => x.ImmArr).NotNull();
    }

    public class WithImmArrNotEmptyV : AbstractValidator<WithImmArr>
    {
        public WithImmArrNotEmptyV() =>
            RuleFor(x => x.ImmArr).NotEmpty();
    }

    public class WithImmArrSafeNotEmptyV : AbstractValidator<WithImmArr>
    {
        public WithImmArrSafeNotEmptyV() =>
            RuleFor(x => x.ImmArrSafe)
                .NotEmpty()
                .OverridePropertyName(x => x.ImmArr);
    }

    [Fact]
    public void Sanity()
    {
        ValidationResult result;
        Action action;

        result = new WithImmArrNotNullV().Validate(new WithImmArr());
        result.Errors.Count.Should().Be(0);

        action = () => new WithImmArrNotEmptyV().Validate(new WithImmArr());
        action.Should().ThrowExactly<InvalidOperationException>();

        result = new WithImmArrSafeNotEmptyV().Validate(new WithImmArr());
        result.Errors.Count.Should().Be(1);
        result.Errors[0].PropertyName.Should().Be(nameof(WithImmArr.ImmArr));

        result = new WithImmArrSafeNotEmptyV().Validate(new WithImmArr { ImmArr = ["foobar"] });
        result.Errors.Count.Should().Be(0);
    }
}