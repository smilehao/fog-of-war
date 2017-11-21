using System;
using System.Collections.Generic;

public class ClassObjPool<T> : ClassObjPoolBase where T : PooledClassObject, new()
{
    // 优化说明：使用Queue或者BetterList都比使用List效率高
    // 经验规则：不要使用List去模拟队列
    private static ClassObjPool<T> instance;
    protected uint reqSeq = 0;
    protected Queue<T> pool = new Queue<T>(32);

    public static T Get()
    {
        if (instance == null)
        {
            instance = new ClassObjPool<T>();
            AddInstance(instance);
        }
        T local;
        if (instance.pool.Count > 0)
        {
            local = instance.pool.Dequeue();
        }
        else
        {
            local = Activator.CreateInstance<T>();
        }
        instance.reqSeq++;
        local.usingSeq = instance.reqSeq;
        local.holder = instance;
        local.OnInit();
        return local;
    }

    public override void Release(PooledClassObject obj)
    {
        T item = obj as T;
        obj.usingSeq = 0;
        pool.Enqueue(item);
    }

    protected override void OnDispose()
    {
        while (pool.Count > 0)
        {
            pool.Dequeue().Dispose();
        }
    }
}