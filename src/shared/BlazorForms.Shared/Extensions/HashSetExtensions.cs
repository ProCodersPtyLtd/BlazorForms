using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared
{
    public static class HashSetExtensions
    {
        //HashSet<Type>

        public static void Set<T>(this HashSet<T> s, T value) where T: class
        {
            if(!s.Contains(value))
            {
                s.Add(value);
            }
        }
    }
}
