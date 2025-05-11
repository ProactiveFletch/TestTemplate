using SoftwareVersioner.Core;

namespace SoftwareVersioner.Client.Services;

public class UserVersionParseResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public int[]? VersionArray { get; set; }
    public string OriginalInput { get; set; } = string.Empty;
}

public static class FunctionalSvc
{
    public static UserVersionParseResult ProcessUserVersionInput(string? rawInput)
    {
        var result = new UserVersionParseResult();

        if (string.IsNullOrWhiteSpace(rawInput))
        {
            result.IsValid = false;
            result.ErrorMessage = "Input cannot be empty.";
            return result;
        }

        string processedInput = rawInput.Trim();
        result.OriginalInput = processedInput; // Store the trimmed original for display

        // FR-INPUT-02: Trailing Dot Handling
        if (processedInput.EndsWith('.') && !processedInput.EndsWith(".."))
        {
            processedInput = processedInput.Substring(0, processedInput.Length - 1);
        }
        if (string.IsNullOrEmpty(processedInput))
        {
            result.IsValid = false;
            result.ErrorMessage = "Version is malformed (became empty after dot processing).";
            return result;
        }

        // FR-INPUT-03: Leading Dot Interpretation
        bool hasLeadingDot = false;
        string coreVersionStr = processedInput;

        if (processedInput.StartsWith('.'))
        {
            hasLeadingDot = true;
            coreVersionStr = processedInput.Substring(1);
        }

        if (string.IsNullOrEmpty(coreVersionStr))
        {
            result.IsValid = false;
            result.ErrorMessage = "Version is malformed (no actual version parts).";
            return result;
        }

        // FR-VALIDATE-01: Allowed Characters
        if (coreVersionStr.Any(c => !char.IsDigit(c) && c != '.'))
        {
            result.IsValid = false;
            result.ErrorMessage = "Version contains invalid characters (only digits and '.' allowed).";
            return result;
        }

        string[] parts = coreVersionStr.Split('.');

        // FR-VALIDATE-02: Maximum Parts
        if (parts.Length > 5)
        {
            result.IsValid = false;
            result.ErrorMessage = "Version has too many parts (maximum 5).";
            return result;
        }

        List<int> numericParts = new List<int>();
        foreach (var partString in parts)
        {
            // FR-VALIDATE-03: Part Structure
            if (string.IsNullOrEmpty(partString))
            {
                result.IsValid = false;
                result.ErrorMessage = "Version contains an empty part (e.g., '1..2').";
                return result;
            }
            if (!int.TryParse(partString, out int numPart) || numPart < 0)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Version part '{partString}' is not a non-negative integer.";
                return result;
            }
            numericParts.Add(numPart);
        }

        if (!numericParts.Any())
        {
            result.IsValid = false;
            result.ErrorMessage = "No numeric parts found in version.";
            return result;
        }

        var finalVersionArray = new int[5]; // Defaults to 0s
        if (hasLeadingDot)
        {
            int targetIndex = 4;
            for (int i = numericParts.Count - 1; i >= 0; i--)
            {
                if (targetIndex >= 0) finalVersionArray[targetIndex--] = numericParts[i];
                else break;
            }
        }
        else
        {
            for (int i = 0; i < numericParts.Count; i++)
            {
                if (i < 5) finalVersionArray[i] = numericParts[i];
                else break;
            }
        }

        result.IsValid = true;
        result.VersionArray = finalVersionArray;
        return result;
    }

    // Helper to parse standard product versions (no leading/trailing dot special logic)
    private static int[]? ParseProductVersion(string? versionString)
    {
        if (string.IsNullOrWhiteSpace(versionString)) return null;

        string[] parts = versionString.Trim().Split('.');
        if (parts.Length > 5) return null; // Invalid format

        var numericVersion = new int[5]; // Default 0s
        for (int i = 0; i < parts.Length; i++)
        {
            if (i >= 5) break;
            if (!int.TryParse(parts[i], out int numPart) || numPart < 0)
            {
                return null; // Invalid part
            }
            numericVersion[i] = numPart;
        }
        return numericVersion;
    }

    private static bool IsVersionAGreaterThanB(int[] versionA, int[] versionB)
    {
        for (int i = 0; i < 5; i++) // Assuming both are 5-part arrays
        {
            if (versionA[i] > versionB[i]) return true;
            if (versionA[i] < versionB[i]) return false;
        }
        return false; // Equal
    }

    public static List<Software> FilterSoftwareList(
        IEnumerable<Software> allSoftware,
        int[] userVersionArrayToCompareAgainst,
        out List<string> productParsingErrors)
    {
        var filteredList = new List<Software>();
        productParsingErrors = new List<string>();

        if (userVersionArrayToCompareAgainst == null || userVersionArrayToCompareAgainst.Length != 5)
        {
            // Should not happen if ProcessUserVersionInput was called correctly
            productParsingErrors.Add("Invalid user version array for comparison.");
            return filteredList;
        }

        foreach (var software in allSoftware)
        {
            if (string.IsNullOrWhiteSpace(software.Version))
            {
                productParsingErrors.Add($"Software '{software.Name}' has a missing version string.");
                continue;
            }

            var productVersionArray = ParseProductVersion(software.Version);
            if (productVersionArray == null)
            {
                productParsingErrors.Add($"Software '{software.Name}' has an invalid version format: '{software.Version}'.");
                continue;
            }

            if (IsVersionAGreaterThanB(productVersionArray, userVersionArrayToCompareAgainst))
            {
                filteredList.Add(software);
            }
        }
        return filteredList;
    }
}