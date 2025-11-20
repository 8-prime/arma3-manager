using System.Text.RegularExpressions;

namespace ArmA3Manager.Application.Services;

public static class WorkshopHelper
{
    /// <summary>
    /// Extracts the numeric Steam Workshop ID from a URL.
    /// </summary>
    /// <param name="workshopUrl">The full Steam Workshop URL.</param>
    /// <returns>The workshop ID as a string.</returns>
    /// <exception cref="ArgumentException">Thrown if ID cannot be extracted.</exception>
    public static string? ExtractWorkshopId(string workshopUrl)
    {
        if (string.IsNullOrWhiteSpace(workshopUrl))
            throw new ArgumentException("Workshop URL cannot be empty.");

        // Match last sequence of digits in URL
        var match = Regex.Match(workshopUrl, @"(\d+)(?:\/)?$");

        return !match.Success
            ? null
            : match.Groups[1].Value;
    }
}