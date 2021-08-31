using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.Stdio.Cli
{
    public class PathName
    {
        public string Wildcard { get; }
        public string Expression { get; }
        public string Name { get; }

        private readonly string[] segments = new string[0];
        private readonly string fullName;

        public PathName(string fullName)
        {
            this.fullName = fullName;

            if (string.IsNullOrEmpty(fullName))
                segments = new string[0];

            else
            {
                segments = fullName.Split('\\');
                int n1 = 0;
                int n2 = segments.Length - 1;

                if (string.IsNullOrEmpty(segments[n1]))
                    segments[n1] = "\\";

                if (segments[n2] == "")
                {
                    segments = segments.Take(n2).ToArray();
                }
                else if (segments[n2].IndexOf('*') >= 0 || segments[n2].IndexOf('?') >= 0)
                {
                    Wildcard = segments[n2];
                    segments = segments.Take(n2).ToArray();
                }
                else if (IsExpression(segments[n2]))
                {
                    Expression = segments[n2];
                    segments = segments.Take(n2).ToArray();
                }
                else
                {
                    Name = segments[n2];
                }
            }
        }



        private static bool IsExpression(string text)
        {
            string[] keys = new string[] { "(", ")", "=", ">", "<", " and ", " or ", " between ", " not ", " is " };
            text = text.ToLower();

            foreach (var key in keys)
            {
                if (text.IndexOf(key) > 0)
                    return true;
            }

            return false;
        }

        public string[] FullSegments
        {
            get
            {
                return this.segments;
            }
        }

        public override string ToString()
        {
            return this.fullName;
        }
    }
}
