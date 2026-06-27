using FluentValidation.Results;

// This test class sanity checks the default FluentValidation library's behavior. In order to not bind to our
// NorthSouthSystems.Opinions.FluentValidation extensions, we invoke FluentValidation methods directly and not via
// extension syntax. This is 100% necessary because of our global usings in the csproj.
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
            DefaultValidatorExtensions.NotNull(RuleFor(x => x.ImmArr));
    }

    public class WithImmArrNotEmptyV : AbstractValidator<WithImmArr>
    {
        public WithImmArrNotEmptyV() =>
            DefaultValidatorExtensions.NotEmpty(RuleFor(x => x.ImmArr));
    }

    public class WithImmArrSafeNotEmptyV : AbstractValidator<WithImmArr>
    {
        public WithImmArrSafeNotEmptyV() =>
            DefaultValidatorExtensions.NotEmpty(RuleFor(x => x.ImmArrSafe))
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