﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RajceUI.Elements
{
    public static class ElementExtension
    {
        #region Element

        public static IEnumerable<Element> AsEnumerable(this Element element) => new ElementEnumerable(element);

        public static IEnumerable<T> Query<T>(this Element element)
        {
            foreach (var e in element.AsEnumerable())
            {
                if (e is T t) yield return t;
            }
        }

        public static IEnumerable<Element> Parents(this Element element)
        {
            for (var parent = element.Parent; parent != null; parent = parent.Parent)
            {
                yield return parent;
            }
        }

        public static LabelElement FirstLabel(this Element element) => element.Query<LabelElement>().FirstOrDefault();


        public static bool ValidateSingleParent(this Element element)
        {
            var invalid = element.Parent != null;
            if (invalid)
            {
                var lineage = new List<string>();
                var tgt = element;
                while (tgt != null)
                {
                    var str = tgt switch
                    {
                        LabelElement l when !string.IsNullOrEmpty(l.Value) => $"Label[{l.Value}]",
                        _ => tgt.GetType().ToString()
                    };

                    lineage.Add(str);

                    tgt = tgt.Parent;
                }

                lineage.Reverse();
                Debug.LogError($"Element already has a parent. {string.Join(" > ", lineage.ToArray())}.\nAn element can only have a single parent.");
            }

            return invalid;
        }


        private readonly struct ElementEnumerable : IEnumerable<Element>
        {
            private readonly Element _element;

            public ElementEnumerable(Element element) => _element = element;
            public IEnumerator<Element> GetEnumerator()
            {
                yield return _element;
                foreach (var child in _element.Children.SelectMany(e => e.AsEnumerable()))
                {
                    yield return child;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        #endregion

        #region LabelElement

        public static LabelElement Clone(this LabelElement me) => new(me);

        #endregion
    }
}
