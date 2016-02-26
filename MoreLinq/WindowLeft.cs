#region License and Terms
// MoreLINQ - Extensions to LINQ to Objects
// Copyright (c) 2008 Jonathan Skeet. All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace MoreLinq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class MoreEnumerable
    {
        /// <summary>
        /// Creates a left-aligned sliding window over the source sequence
        /// of a given size.
        /// </summary>

        public static IEnumerable<IEnumerable<TSource>> WindowLeft<TSource>(this IEnumerable<TSource> source, int size)
        {
            return source.WindowLeft(size, (_, w) => w);
        }

        /// <summary>
        /// Creates a left-aligned sliding window over the source sequence
        /// of a given size.
        /// </summary>

        public static IEnumerable<TResult> WindowLeft<TSource, TResult>(this IEnumerable<TSource> source, int size,
            Func<TSource, IEnumerable<TSource>, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (size <= 0) throw new ArgumentOutOfRangeException("size");

            return source.WindowLeftWhile((_, i) => i < size, resultSelector);
        }

        /// <summary>
        /// Creates a left-aligned sliding window over the source sequence
        /// with a predicate function determining the window range.
        /// </summary>

        public static IEnumerable<TResult> WindowLeftWhile<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, bool> predicate, Func<TSource, IEnumerable<TSource>, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return source.WindowLeftWhile((e, _) => predicate(e), resultSelector);
        }

        /// <summary>
        /// Creates a left-aligned sliding window over the source sequence
        /// with a predicate function determining the window range.
        /// </summary>

        public static IEnumerable<TResult> WindowLeftWhile<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate, Func<TSource, IEnumerable<TSource>, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return WindowLeftWhileImpl(source, predicate, resultSelector);
        }

        static IEnumerable<TResult> WindowLeftWhileImpl<TSource, TResult>(IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate, Func<TSource, IEnumerable<TSource>, TResult> resultSelector)
        {
            var window = new List<TSource>();
            foreach (var item in source)
            {
                window.Add(item);
                if (predicate(item, window.Count))
                    continue;
                yield return resultSelector(window[0], window);
                window = new List<TSource>(window.Skip(1));
            }
            while (window.Count > 0)
            {
                yield return resultSelector(window[0], window);
                window = new List<TSource>(window.Skip(1));
            }
        }
    }
}