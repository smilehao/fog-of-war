
using System;

public abstract class PooledClassObject
{
    public bool bChkReset = true;
    public ClassObjPoolBase holder;
    public uint usingSeq;

    public abstract void OnRelease();

    public abstract void OnInit();

    public virtual void Dispose() { }

    public void Release()
    {
        this.OnRelease();
        this.holder.Release(this);
    }
}

