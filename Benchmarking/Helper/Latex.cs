using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Helper
{
    public static class Latex
    {

        public static List<string> Table(string[] columns, List<string[]> rows, string caption, string label)
        {
            var results = new List<string>();

            results.Add("\\begin{table}[h!]");
            results.Add("\\centering");
            results.Add($"\\begin{{tabular}}{{ {string.Join(" ", Enumerable.Range(0, columns.Length).Select(s => "c"))} }}");
            results.Add("\\toprule");
            results.Add($"{string.Join(" & ", columns)} \\\\ [0.5ex]");
            results.Add("\\midrule");

            foreach (var row in rows)
            {
                results.Add($"{string.Join(" & ", row)} \\\\");
            }

            results.Add("\\bottomrule");
            results.Add("\\end{tabular}");
            results.Add($"\\caption{{{caption}}}");
            results.Add($"\\label{{table:{label}}}");
            results.Add("\\end{table}");

            return results;
        }
    }
}
