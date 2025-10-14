using FluentValidation;

namespace NasaSyncService.Application.Features.Meteorites.GetGroupedMeteorites
{
    public class GetGroupedMeteoritesValidator : AbstractValidator<GetGroupedMeteoritesQuery>
    {
        public GetGroupedMeteoritesValidator()
        {
            RuleFor(x => x.YearFrom).GreaterThan(0).When(x => x.YearFrom.HasValue);

            RuleFor(x => x.YearTo).GreaterThan(0).When(x => x.YearTo.HasValue);

            RuleFor(x => x.YearTo)
                .GreaterThanOrEqualTo(x => x.YearFrom)
                .When(x => x.YearFrom.HasValue && x.YearTo.HasValue);

            RuleFor(x => x.NameContains)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.NameContains));

            RuleFor(x => x.SortBy)
                .Must(v => new[] { "year", "count", "mass" }.Contains(v!))
                .When(x => !string.IsNullOrEmpty(x.SortBy))
                .WithMessage("SortBy must be 'year', 'count' or 'mass'");
        }
    }
}
