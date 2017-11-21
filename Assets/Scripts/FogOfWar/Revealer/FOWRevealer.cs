using UnityEngine;

/// <summary>
/// 说明：视野对象基类
/// 
/// @by wsh 2017-05-20
/// </summary>

public class FOWRevealer : PooledClassObject, IFOWRevealer
{
    // 共享数据
    protected bool m_isValid;
    protected Vector3 m_position;
    protected float m_radius;

    public FOWRevealer()
    {
    }

    static public FOWRevealer Get()
    {
        return ClassObjPool<FOWRevealer>.Get();
    }

    public override void OnInit()
    {
        m_position = Vector3.zero;
        m_radius = 0f;
        m_isValid = false;
    }

    public override void OnRelease()
    {
        m_isValid = false;
    }

    public virtual Vector3 GetPosition()
    {
        return m_position;
    }

    public virtual float GetRadius()
    {
        return m_radius;
    }

    public bool IsValid()
    {
        return m_isValid;
    }

    public virtual void Update(int deltaMS)
    {
        // 更新所有共享数据，m_isValid最后更新
    }
}
