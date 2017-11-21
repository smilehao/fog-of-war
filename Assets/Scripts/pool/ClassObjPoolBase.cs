using System;
using System.Collections.Generic;

public abstract class ClassObjPoolBase
{
    private static List<ClassObjPoolBase> instanceList = new List<ClassObjPoolBase>();

    protected ClassObjPoolBase()
    {
    }

    public abstract void Release(PooledClassObject obj);
    protected abstract void OnDispose();

    protected static void AddInstance(ClassObjPoolBase instance)
    {
        instanceList.Add(instance);
    }

    public static void Dispose()
    {
        for (int i = 0; i < instanceList.Count; i++)
        {
            instanceList[i].OnDispose();
        }
        instanceList.Clear();
    }
}