using System;
using System.Collections;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class PackageReferenceCollection : ICollection<PackageReference>
    {
        readonly Dictionary<string, PackageReference> nameMap = new Dictionary<string, PackageReference>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, PackageReference> idMap = new Dictionary<string, PackageReference>(StringComparer.OrdinalIgnoreCase);

        public PackageReference PrimaryPackage => nameMap.ContainsKey("") ? nameMap[""] : null;
        
        public void Add(PackageReference item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (nameMap.ContainsKey(item.Name))
                throw new ArgumentException($"A package reference with the name '{item.Name}' already exists");

            if (idMap.ContainsKey(item.Id))
                throw new ArgumentException($"A package reference with the ID '{item.Id}' already exists");

            nameMap.Add(item.Name, item);
            idMap.Add(item.Id, item);
        }
        
        public PackageReference GetById(string id)
        {
            return idMap[id];
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
        
        public bool TryGetById(string id, out PackageReference package)
        {
            if (!string.IsNullOrEmpty(id) && idMap.ContainsKey(id))
            {
                package = idMap[id];
                return true;
            }

            package = null;
            return false;
        }

        public bool TryGetByIdOrName(string idOrName, out PackageReference package)
        {
            if (TryGetById(idOrName, out package))
            {
                return true;
            }

            if (TryGetByName(idOrName, out package))
            {
                return true;
            }

            package = null;
            return false;
        }

        public bool Contains(PackageReference item)
        {
            return idMap.ContainsKey(item.Id);
        }

        public void CopyTo(PackageReference[] array, int arrayIndex)
        {
            nameMap.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(PackageReference item)
        {
            nameMap.Remove(item.Name);
            return idMap.Remove(item.Id);
        }

        public int Count => nameMap.Count; 
        
        public void Clear()
        {
            idMap.Clear();
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