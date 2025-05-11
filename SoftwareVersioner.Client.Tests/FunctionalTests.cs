using SoftwareVersioner.Client.Services;
using SoftwareVersioner.Core;

namespace SoftwareVersioner.Client.Tests;

public class FunctionalSvcTests
{
    // Tests for ProcessUserVersionInput
    [Theory]
    [InlineData(null, false, "Input cannot be empty.", null, "")]
    [InlineData("", false, "Input cannot be empty.", null, "")]
    [InlineData("   ", false, "Input cannot be empty.", null, "")]
    [InlineData(".", false, "Version is malformed (became empty after dot processing).", null, ".")]
    [InlineData("..", false, "Version contains an empty part (e.g., '1..2').", null, "..")] // After leading dot removal, core is "."
    [InlineData("1.a.3", false, "Version contains invalid characters (only digits and '.' allowed).", null, "1.a.3")]
    [InlineData("1.-2.3", false, "Version contains invalid characters (only digits and '.' allowed).", null, "1.-2.3")]
    [InlineData("1.2.3.4.5.6", false, "Version has too many parts (maximum 5).", null, "1.2.3.4.5.6")] // 6 parts
    [InlineData(".1.2.3.4.5.6", false, "Version has too many parts (maximum 5).", null, ".1.2.3.4.5.6")] // core is "1.2.3.4.5.6" -> 6 parts
    [InlineData("1..2", false, "Version contains an empty part (e.g., '1..2').", null, "1..2")]
    public void ProcessUserVersionInput_InvalidInputs_ReturnsError(string input, bool expectedIsValid, string expectedError, int[]? _, string expectedOriginalInput)
    {
        var result = FunctionalSvc.ProcessUserVersionInput(input);
        Assert.Equal(expectedIsValid, result.IsValid);
        Assert.Equal(expectedError, result.ErrorMessage);
        Assert.Null(result.VersionArray);
        Assert.Equal(expectedOriginalInput, result.OriginalInput); // Test original input storage
    }

    [Theory]
    [InlineData("1.2.3", true, null, new int[] { 1, 2, 3, 0, 0 }, "1.2.3")]
    [InlineData(" 1.2.3 ", true, null, new int[] { 1, 2, 3, 0, 0 }, "1.2.3")] // Trim
    [InlineData("1.2.3.", true, null, new int[] { 1, 2, 3, 0, 0 }, "1.2.3.")] // Trailing dot
    [InlineData(".1.2.3", true, null, new int[] { 0, 0, 1, 2, 3 }, ".1.2.3")] // Leading dot
    [InlineData(".1", true, null, new int[] { 0, 0, 0, 0, 1 }, ".1")]
    [InlineData("5", true, null, new int[] { 5, 0, 0, 0, 0 }, "5")]
    [InlineData("0.0.0.0.1", true, null, new int[] { 0, 0, 0, 0, 1 }, "0.0.0.0.1")]
    [InlineData("17.0.31919.166.0", true, null, new int[] { 17, 0, 31919, 166, 0 }, "17.0.31919.166.0")]
    [InlineData(".0.0.1", true, null, new int[] { 0, 0, 0, 0, 1 }, ".0.0.1")]
    [InlineData(".1.2.3.4.5", true, null, new int[] { 1, 2, 3, 4, 5 }, ".1.2.3.4.5")] // CORRECTED TEST CASE
    public void ProcessUserVersionInput_ValidInputs_ParsesCorrectly(string input, bool expectedIsValid, string? expectedError, int[] expectedArray, string expectedOriginalInput)
    {
        var result = FunctionalSvc.ProcessUserVersionInput(input);
        Assert.Equal(expectedIsValid, result.IsValid);
        Assert.Equal(expectedError, result.ErrorMessage);
        Assert.Equal(expectedArray, result.VersionArray);
        Assert.Equal(expectedOriginalInput, result.OriginalInput);
    }

    // Tests for FilterSoftwareList (No change from previous "Redo" plan - these should be fine)
    public static IEnumerable<object[]> FilterTestData =>
        new List<object[]>
        {
                // Test Case 1: Basic greater than
                new object[] {
                    new List<Software> { new Software { Name = "A", Version = "1.0.1" }, new Software { Name = "B", Version = "1.0.0" } },
                    new int[] { 1, 0, 0, 0, 0 }, // User version "1.0.0"
                    new List<string> { "A" }, // Expected names
                    0 // Expected product parsing errors
                },
                // Test Case 2: No results
                new object[] {
                    new List<Software> { new Software { Name = "A", Version = "1.0.0" } },
                    new int[] { 1, 0, 0, 0, 0 }, // User version "1.0.0"
                    new List<string> { },
                    0
                },
                // Test Case 3: User version is higher
                new object[] {
                    new List<Software> { new Software { Name = "A", Version = "1.0.0" } },
                    new int[] { 2, 0, 0, 0, 0 }, // User version "2.0.0"
                    new List<string> { },
                    0
                },
                // Test Case 4: Full version numbers
                new object[] {
                    new List<Software> { new Software { Name = "VS1", Version = "17.0.31919.166.0"}, new Software { Name = "VS2", Version = "16.11.9.3.55" } },
                    new int[] { 16, 11, 9, 3, 55 }, // User "16.11.9.3.55"
                    new List<string> { "VS1" },
                    0
                },
                 // Test Case 5: Short product version
                new object[] {
                    new List<Software> { new Software { Name = "Ang", Version = "13"} }, // "13.0.0.0.0"
                    new int[] { 12, 0, 0, 0, 0 }, // User "12"
                    new List<string> { "Ang" },
                    0
                },
                // Test Case 6: Invalid product version in list
                new object[] {
                    new List<Software> { new Software { Name = "Valid", Version = "2.0"}, new Software { Name = "Invalid", Version = "1.x.0"} },
                    new int[] { 1, 0, 0, 0, 0 },
                    new List<string> { "Valid" },
                    1 // One product parsing error
                },
                // Test Case 7: Empty product version in list
                new object[] {
                    new List<Software> { new Software { Name = "Valid", Version = "2.0"}, new Software { Name = "EmptyVer", Version = " " } },
                    new int[] { 1, 0, 0, 0, 0 },
                    new List<string> { "Valid" },
                    1
                },
                 // Test Case 8: Complex comparison
                new object[] {
                    new List<Software> {
                        new Software { Name = "S1", Version = "2.0.0.0.1" }, // >
                        new Software { Name = "S2", Version = "2" },         // == (not greater)
                        new Software { Name = "S3", Version = "2.1" },       // >
                        new Software { Name = "S4", Version = "1.9.9.9.9" }  // <
                    },
                    new int[] { 2, 0, 0, 0, 0 }, // User "2"
                    new List<string> { "S1", "S3" },
                    0
                },
        };

    [Theory]
    [MemberData(nameof(FilterTestData))]
    public void FilterSoftwareList_VariousScenarios_FiltersCorrectly(
        IEnumerable<Software> allSoftware,
        int[] userVersionArray,
        List<string> expectedNames,
        int expectedProductParsingErrors)
    {
        var result = FunctionalSvc.FilterSoftwareList(allSoftware, userVersionArray, out var productParsingErrors);

        Assert.Equal(expectedNames.Count, result.Count);
        Assert.Equal(expectedProductParsingErrors, productParsingErrors.Count);
        foreach (var name in expectedNames)
        {
            Assert.Contains(result, s => s.Name == name);
        }
    }
}