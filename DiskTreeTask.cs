using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DiskTree
{
    public static class DiskTreeTask
    {
        public static List<string> Solve(List<string> input)
        {
            var _roots = new List<Catalog>();

            foreach (string directory in input)
            {
                var catalogs = directory.Split('\\');
                var root = new Catalog(catalogs[0]);
                if (!_roots.Contains(root))
                    _roots.Add(root);
                var parentCatalog = _roots[_roots.IndexOf(root)];
                foreach (var catalog in catalogs.Skip(1))
                {
                    var currentCatalog = new Catalog(catalog, parentCatalog.Height + 1);
                    if (!parentCatalog.NestedCatalogs.Contains(currentCatalog))
                        parentCatalog.AddNested(catalog);
                    parentCatalog = parentCatalog.NestedCatalogs[parentCatalog.NestedCatalogs.IndexOf(currentCatalog)];
                }
            }

            return _roots.OrderBy(r => r.Name, StringComparer.Ordinal)
                .SelectMany(r => r)
                .ToList();
        }
    }

    public class Catalog : IEnumerable<string>
    {
        public string Name
        {
            get { return name.PadLeft(Height + name.Length); }
            private set { name = value; }
        }
        public List<Catalog> NestedCatalogs = new List<Catalog>();
        public int Height { get; set; }
        private string name;

        public Catalog(string name)
        {
            Name = name;
        }

        public Catalog(string name, int height) : this(name)
        {
            Height = height;
        }

        public void AddNested(string name)
        {
            NestedCatalogs.Add(new Catalog(name, Height + 1));
        }

        public override bool Equals(object obj)
        {
            return obj is Catalog catalog &&
                   Name == catalog.Name;
        }

        public override int GetHashCode()
        {
            int hashCode = 1748798246;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Catalog>>.Default.GetHashCode(NestedCatalogs);
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            return hashCode;
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return Name;
            foreach (var catalog in NestedCatalogs
                .OrderBy(s => s.Name, StringComparer.Ordinal)
                .SelectMany(x => x))
                yield return catalog;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
