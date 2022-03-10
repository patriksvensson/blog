using System.Collections.Generic;
using System.Linq;
using Statiq.Common;

public class PathNormalizer
{
    public static string OptimizeFileName(string input)
    {
        input = input.Replace("+", "plus");
        input = input.Replace("#", "sharp");
        return NormalizedPath.OptimizeFileName(input);
    }
}
