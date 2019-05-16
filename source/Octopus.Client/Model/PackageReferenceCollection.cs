using System;
using System.Collections;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class PackageReferenceCollection : ICollection<PackageReference>
    {
        readonly Dictionary<string, PackageReference> nameMap = new Dictionary<string, PackageReference>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Returns the primary (un-named) package reference, or null if it does not exist. 
        /// </summary>
        public PackageReference PrimaryPackage => nameMap.ContainsKey("") ? nameMap[""] : null;
        
        public void Add(PackageReference item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (nameMap.ContainsKey(item.Name))
                throw new ArgumentException($"A package reference with the name '{item.Name}' already exists");

            nameMap.Add(item.Name, item);
        }
        
        public PackageReference GetByName(string name)
        {
            var key = name ?? "";
            return nameMap[key];
        }
        
        public bool TryGetByName(string name, out PackageReference package)
        {
            var key = name ?? "";
            if (nameMap.ContainsKey(key))
            {
                package = nameMap[key];
                return true;
            }

            package = null;
            return false;
        }
        
        /// <summary>
        /// Returns true if a PackageReference with the same name exists; otherwise false. 
        /// </summary>
        public bool Contains(PackageReference item)
        {
            return nameMap.ContainsKey(item.Name);
        }

        public void CopyTo(PackageReference[] array, int arrayIndex)
        {
            nameMap.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove the PackageReference with the matching name from the collection
        /// </summary>
        public bool Remove(PackageReference item)
        {
            return nameMap.Remove(item.Name);
        }

        public int Count => nameMap.Count; 
        
        public void Clear()
        {
            nameMap.Clear();
        }

        public bool IsReadOnly => false;
        
        public IEnumerator<PackageReference> GetEnumerator()
        {
            return nameMap.Values.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}