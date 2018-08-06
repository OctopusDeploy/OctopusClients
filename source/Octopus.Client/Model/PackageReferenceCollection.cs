using System;
using System.Collections;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class PackageReferenceCollection : ICollection<PackageReference>
    {
        readonly Dictionary<string, PackageReference> map = new Dictionary<string, PackageReference>(StringComparer.OrdinalIgnoreCase);

        public PackageReference this[string name] => map[name];

        public PackageReference PrimaryPackage => map.ContainsKey("") ? map[""] : null; 
        
        public void Add(PackageReference item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            
           if (map.ContainsKey(item.Name)) 
               throw new ArgumentException("A package reference with this name already exists", nameof(item)); 
            
            map.Add(item.Name, item);
        }

        public bool Contains(PackageReference item)
        {
            return map.ContainsKey(item.Name);
        }

        public void CopyTo(PackageReference[] array, int arrayIndex)
        {
            map.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(PackageReference item)
        {
            return map.Remove(item.Name);
        }

        public int Count => map.Count; 
        
        public void Clear()
        {
            map.Clear();
        }

        public bool IsReadOnly => false;
        
        public IEnumerator<PackageReference> GetEnumerator()
        {
            return map.Values.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}