using System;
using System.Collections.Generic;

namespace LRU
{
    public struct LRUNode
    {
        public string Key;

        public object Ref;


        public void OnCreate()
        {
        }

        public void OnDelete()
        {
        }
    }

    public class LRUCache<T> : LRUCache where T : class
    {
        public LRUCache(int capacity) : base(capacity)
        {
        }

        public T Get(string key)
        {
            return base.Get(key) as T;
        }

        public bool Put(string key, T val)
        {
            return base.Put(key, val);
        }
    }

    public class LRUCache
    {
        private Dictionary<string, LinkedListNode<LRUNode>> map;
        private LinkedList<LRUNode> cache;
        // 最大容量
        private int cap;
        public int Capacity
        {
            get 
            {
                return cap;
            }

            set 
            {
                if (cap > value && Size > value)
                {
                    _Resize(value);
                }

                cap = value;
            }
        }

        private void _Resize(int newSize)
        {
            while (cache.Count > newSize)
            {
                _RemoveLastOne();
            }
        }

        public LRUCache(int capacity)
        {
            cap = capacity;
            map = new Dictionary<string, LinkedListNode<LRUNode>>(cap);
            cache = new LinkedList<LRUNode>();
            RemoveCount = 0;
        }

        public void Clear()
        {
            RemoveCount = 0;
            map.Clear();
            cache.Clear();
        }

        public int RemoveCount 
        {
            get; private set;
        }

        public int Size
        {
            get 
            {
                return cache.Count;
            }
        }

        public object Get(string key)
        {
            if (Capacity <= 0)
            {
                return null;
            }

            if (!map.ContainsKey(key))
            {
                return null;
            }

            var val = map[key].Value.Ref;
            Put(key, val);
            return val;
        }

        public bool Put(string key, object val)
        {
            if (Capacity <= 0)
            {
                return false;
            }

            if (map.ContainsKey(key))
            {
                // 删除旧的节点，新的插到头部
                cache.Remove(map[key]);
                cache.AddFirst(map[key]);
                return false;
            }
            else
            {
                if (cap == cache.Count)
                {
                    _RemoveLastOne();
                }

                // 先把新节点 x 做出来
                LRUNode x = new LRUNode();//key, val);
                x.Key = key;
                x.Ref = val;
                cache.AddFirst(x);
                map.Add(key, cache.First);

                return true;
            }
        }

        private void _RemoveLastOne()
        {
            RemoveCount++;

            var node = cache.Last;
            var lastKey = node.Value.Key;

            //UnityEngine.Debug.LogErrorFormat("remove last {0} {1}", lastKey, node.Value.UseCount);

            cache.RemoveLast();
            map.Remove(lastKey);

            node.Value.OnDelete();
        }
    }

    public class CollectLRUNode
    {
        public string Key;

        public object Ref;

        // 代
        public byte Generation;


        public void OnCreate()
        {
        }

        public void OnDelete()
        {
        }
    }

    public class CollectLRUCache<T> : CollectLRUCache where T : class
    {
        public CollectLRUCache(int capacity) : base(capacity)
        {
        }

        public T Get(string key)
        {
            return base.Get(key) as T;
        }

        public bool Put(string key, T val)
        {
            return base.Put(key, val);
        }
    }
    
     public class CollectLRUCache
    {
        private Dictionary<string, LinkedListNode<CollectLRUNode>> map;
        private LinkedList<CollectLRUNode> cache;

        private bool isClear;
        // 最大容量
        private int cap;
        public int Capacity
        {
            get 
            {
                return cap;
            }

            set 
            {
                if (cap > value && Size > value)
                {
                    _Resize(value);
                }

                cap = value;
            }
        }

        private void _Resize(int newSize)
        {
            while (cache.Count > newSize)
            {
                _RemoveLastOne();
            }
        }

        public CollectLRUCache(int capacity)
        {
            cap = capacity;
            map = new Dictionary<string, LinkedListNode<CollectLRUNode>>(cap);
            cache = new LinkedList<CollectLRUNode>();
            RemoveCount = 0;
        }

        public void Clear()
        {
            RemoveCount = 0;
            map.Clear();
            cache.Clear();
        }

        public void Start()
        {
            isClear = false;
            var currentNode = cache.First;
            while (currentNode != null)
            {
                var currentNodeValue = currentNode.Value;
                currentNodeValue.Generation++;
                currentNode = currentNode.Next;
            }
        }
        
        public void Collect(int generation = 1)
        {
            if(isClear)
                return;
            
            var currentNode = cache.First;
            while (currentNode != null)
            {
                if (currentNode.Value.Generation >= generation)
                {
                    var toRemove = currentNode;
                    currentNode = currentNode.Next;
                    cache.Remove(toRemove);
                    map.Remove(toRemove.Value.Key);
                    //UnityEngine.Debug.LogWarning(toRemove.Value.Key);
                }
                else
                {
                    currentNode = currentNode.Next;
                }
            }

            isClear = true;
        }

        public int RemoveCount 
        {
            get; private set;
        }

        public int Size
        {
            get 
            {
                return cache.Count;
            }
        }

        public object Get(string key)
        {
            if (Capacity <= 0)
            {
                return null;
            }

            if (!map.ContainsKey(key))
            {
                return null;
            }

            var val = map[key].Value.Ref;
            Put(key, val);
            return val;
        }

        public bool Put(string key, object val)
        {
            if (Capacity <= 0)
            {
                return false;
            }

            if (map.ContainsKey(key))
            {
                // 删除旧的节点，新的插到头部
                var node = map[key].Value;
                node.Generation = 0;
                cache.Remove(node);
                cache.AddFirst(node);
                return false;
            }
            else
            {
                if (cap == cache.Count)
                {
                    _RemoveLastOne();
                }

                // 先把新节点 x 做出来
                CollectLRUNode x = new CollectLRUNode();//key, val);
                x.Key = key;
                x.Ref = val;
                x.Generation = 0;
                cache.AddFirst(x);
                map.Add(key, cache.First);

                return true;
            }
        }

        private void _RemoveLastOne()
        {
            RemoveCount++;

            var node = cache.Last;
            var lastKey = node.Value.Key;

            //UnityEngine.Debug.LogErrorFormat("remove last {0} {1}", lastKey, node.Value.UseCount);

            cache.RemoveLast();
            map.Remove(lastKey);

            node.Value.OnDelete();
        }
    }
}
