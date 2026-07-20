using Core.Types;
using Test.Shared;
using Web.Client.Extensions;

namespace Core.Test.Extensions;

[TestClass]
public sealed class LetterStateExtensionsTests
{
    [TestMethod]
    public void ToCssClass_ReturnsCssClassForEachState()
        => EnumExtensionTest.VerifyCanProcessAllKnown<LetterState>(state => state.ToCssClass());

    [TestMethod]
    public void ToCssClass_InvalidValue_ThrowsArgumentOutOfRange()
        => EnumExtensionTest.VerifyInvalidValueThrows<LetterState>(state => state.ToCssClass());
}