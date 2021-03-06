﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeElements
{
    public abstract class ElementList : Element
    {
        public ElementList(string name, bool isDefault = false) : base(name, isDefault)
        {
        }
        public abstract void Add(Element item);
        public abstract void Remove(Element item);
    }

    public class ElementList<T> : ElementList, ICollection<T>, IEnumerable<T>, IDictionary<string,T> where T : Element
    {
        protected Dictionary<string,T> elements;

        public ElementList(string name, bool isDefault = false) : base(name, isDefault)
        {
            elements = new Dictionary<string, T>();
        }

        protected void RemoveFromElements(Element e)
        {
            if (e is Data temp)
                temp.Categories.Remove(this);
        }
        protected void AddToElements(Element e)
        {
            if (e is Data temp && !temp.Categories.Contains(this))
                temp.Categories.Add(this);
        }

        public ICollection Keys => elements.Keys;

        public ICollection Values => elements.Values;

        T IDictionary<string, T>.this[string key] { get => elements[key];
            set
            {
                // If the given value's name matches the key
                if (value.Name == key)
                {
                    RemoveFromElements(elements[key]);
                    AddToElements(value);
                    elements[key] = value;
                }
                else throw new ArgumentException("The new value hasn't a name corresponding to the key");
            }
        }

        ICollection<string> IDictionary<string, T>.Keys => elements.Keys;

        ICollection<T> IDictionary<string, T>.Values => elements.Values;

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();
        
        /// <summary>
        /// Shouldn't be called as you can only add a T kind of value
        /// </summary>
        /// <param name="value"></param>
        public override void Add(Element value)
        {
            throw new ArgumentException("Value must be of type T");
        }
        public void Add(T value)
        {
            if (!elements.ContainsKey(value.Name))
            {
                AddToElements(value);
                elements.Add(value.Name, value);
            }
            else
                throw new ArgumentException("An element with the same name already exists in the dictionary");
        }
        public void Add(string key, T value)
        {
            if (key == null) throw new ArgumentNullException("Key is null");
            else if (elements.ContainsKey(key)) throw new ArgumentException("An element with the given key already exists in the dictionary");
            else if (value.Name != key) throw new ArgumentException("The Elements name must be the same as the key");
            else
            {
                Add(value);
            }
        }

        public void Add(KeyValuePair<string, T> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(T item)
        {
            return elements.Values.Contains(item);
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            if (item.Key != item.Value.Name) throw new ArgumentException("The key does match the items name");
            return Contains(item.Value);
        }

        public bool ContainsKey(string key)
        {
            return elements.Keys.Contains(key);
        }

        public void CopyTo(T[] array, int index)
        {
            if (array.Length - index < elements.Count) throw new ArgumentException("Array is too small");
            else if (index < 0) throw new IndexOutOfRangeException("Index must be positive");
            else if (array == null) throw new ArgumentNullException("Undefined array");

            List<T> temp = elements.Values.ToList<T>();
            for (int i = 0; i < temp.Count; i++)
                array[index + i] = temp[i];
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int index)
        {
            if (array.Length - index < elements.Count) throw new ArgumentException("Array is too small");
            else if (index < 0) throw new IndexOutOfRangeException("Index must be positive");
            else if (array == null) throw new ArgumentNullException("Undefined array");

            int i = 0;
            foreach(string s in elements.Keys)
            {
                array[index + i] = new KeyValuePair<string, T>(s, elements[s]);
                i++;
            }
        }

        /// <summary>
        /// Shouldn't be called as you can only add a T kind of value
        /// </summary>
        /// <param name="value"></param>
        public override void Remove(Element value)
        {
            throw new ArgumentException("Value must be of type T");
        }
        public bool Remove(T value)
        {
            if (elements.ContainsValue(value))
            {
                RemoveFromElements(value);
                return elements.Remove(value.Name);
            }
            else return false;
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            if (item.Key != item.Value.Name) throw new ArgumentException("The key does match the items name");
            else return Remove(item.Value);
        }

        bool IDictionary<string, T>.Remove(string key)
        {
            if (elements.ContainsKey(key))
            {
                RemoveFromElements(elements[key]);
                return elements.Remove(key);
            }
            else return false;
        }

        public void Clear()
        {
            foreach(T t in elements.Values)
            {
                RemoveFromElements(t);
            }
            elements.Clear();
        }

        public bool TryGetValue(string key, out T value)
        {
            if (key == null) throw new ArgumentNullException("key is null");
            else if (elements.ContainsKey(key))
            {
                value = elements[key];
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return elements.Values.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return elements.GetEnumerator();
        }
    }
}
