using System.ComponentModel.DataAnnotations;
using Renmei.Application.Features.Kanji;

namespace Renmei.Application.Tests.Features.Kanji;

public class ListKanjiRequestValidationTests
{
    private static bool TryValidate(ListKanjiRequest model, out List<ValidationResult> results)
    {
        results = [];
        return Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
    }

    [Theory]
    [InlineData(0, 20, null)]
    [InlineData(1, 1000, null)]
    [InlineData(1, 20, 9)]
    public void Invalid_query_parameters_fail_validation(int page, int pageSize, int? level)
    {
        var req = new ListKanjiRequest { Page = page, PageSize = pageSize, Level = level };
        Assert.False(TryValidate(req, out _));
    }
}