// Copyright © 2017 - 2023 Chocolatey Software, Inc
// Copyright © 2011 - 2017 RealDimensions Software, LLC
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// 
// You may obtain a copy of the License at
// 
// 	http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace chocolatey.infrastructure.app.utility
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class ProgramRegistryEntry
    {
        public string DisplayName { get; set; }
        public string DisplayVersion { get; set; }


        public List<string> SearchableNames
        {
            get
            {
                string[] split = Regex.Split(DisplayName.ToLower(), @"[ ]+");
                return GenerateAllNameVariants(split);
            }
        }

        private List<string> GenerateAllNameVariants(string[] split)
        {
            HashSet<string> possibleNames = new HashSet<string>();
            
            for (int j = split.Length; j > 0; --j)
            {
                string[] splitName = split.Take(j).ToArray();

                Stack<string> prev = new Stack<string>();
                Stack<string> next = new Stack<string>();

                if (split[0].Contains("-"))
                    prev.Push(split[0].Replace("-", ""));

                prev.Push(split[0]);

                for (int k = 0; k < 2; ++k)
                {
                    for (int i = 1; i < splitName.Length - 1; ++i)
                    {
                        while (prev.Any())
                        {
                            string s = prev.Pop();
                            string a;
                            if (k == 0)
                                a = s + splitName[i];
                            else
                                a = s + "-" + splitName[i];
                            next.Push(a);
                            if (splitName[i].Contains("-"))
                            {
                                string z = splitName[i].Replace("-", "");
                                string c;
                                if (k == 0)
                                    c = s + z;
                                else
                                    c = s + "-" + z;
                                next.Push(c);
                            }
                        }
                        (prev, next) = (next, prev);
                    }
                    
                }

                while (prev.Any())
                    possibleNames.Add(prev.Pop());
            }

            return possibleNames.ToList();
        }

        public string ChocolateyVersion
        {
            get
            {
                string version = null;
                string[] split = Regex.Split(DisplayVersion.ToLower(), @"[ ]+");
                if (split.Length > 1)
                {
                    for (int i = 0; i < split.Length; ++i)
                    {
                        if (Regex.IsMatch(split[i], "^[0-9\\.-]+$"))
                        {
                            version = split[i];
                            break;
                        }
                    }
                    if(version == null)
                        version = split[split.Length - 1];
                }
                else
                    version = DisplayVersion;
                
                if (version.Contains("-") && !version.ContainsAlpha())
                    return version.Replace('-', '.');

                return version;
            }
        }
    }
}