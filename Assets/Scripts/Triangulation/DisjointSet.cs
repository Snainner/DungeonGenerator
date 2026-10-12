using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public struct DisjointSet 
{
    int[] Parent;

    public void Initialize(int size)
    {
        Parent = new int[size];
        for (int i = 0; i < size; ++i)
            Parent[i] = i;
    }

    /// <summary>
    /// Find the representative (root) of the set that includes the i element 
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public int Find(int i)
    {
        //if the index itself is root or representative 
        if (Parent[i] == i)
            return i;

        //Otherwise recursively find the representative of the parent
        return Find(Parent[i]);
    }
    /// <summary>
    /// Merge the set that includes element i and the set that includes element j
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public void Union(int i, int j)
    {
        //Find representative of i
        int iRep = Find(i);
        //Find representative of j
        int jRep = Find(j);

        //Make the representative of i's set be the representative of j's set
        Parent[iRep] = jRep;
    }

}
