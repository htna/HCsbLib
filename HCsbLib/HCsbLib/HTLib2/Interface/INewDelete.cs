/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
//using System.Runtime.Serialization;

namespace HTLib2
{
    public interface INewDelete<T>
        where T : new()
    {
        T New();
        void Delete(T obj);
    }

    // https://docs.microsoft.com/en-us/dotnet/api/system.activator?view=netcore-3.1
    public class HNewDeleteDefault<T> : INewDelete<T>
        where T : new()
    {
        public ref T New()
        {
            Nullable<object> t;


            return new T();
        }
        public void Delete(T obj)
        {
        }
    }
    public class HNewDelete<T> : INewDelete<T>
        where T : new()
    {
        List<T>   elems;
        List<int> nexts;
        int       igarbage;

        public HNewDelete(int capacity=64)
        {
            elems = new List<T  >(capacity);
            nexts = new List<int>(capacity);
            ElemsReset();
        }

        void ElemsReset()
        {
            HDebug.Assert(elems.Count == nexts.Count);
            for(int i=0; i<elems.Count; i++)
            {
                elems[i] = new T();
                nexts[i] = i+1;
            }
            nexts[nexts.Count-1] = -1;
            igarbage = 1;
        }

        void ElemsAdd()
        {
            HDebug.Assert(igarbage == -1);
            elems.Add(new T());
            nexts.Add(-1);
            igarbage = nexts.Count-1;
        }

        public ref T New()
        {
            if(igarbage != -1)
            {
                int inew = igarbage;
                igarbage = nexts[igarbage];
                elems[inew] = new T();
                nexts[inew] = -2;
                return elems[inew];
            }
            else
            {

            }
        }
        public void Delete(T obj)
        {

        }
    }
}
*/