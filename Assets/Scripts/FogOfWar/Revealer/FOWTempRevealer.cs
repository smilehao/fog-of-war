using UnityEngine;

/// <summary>
/// 说明：临时视野
/// 
/// @by wsh 2017-05-20
/// </summary>

public class FOWTempRevealer : FOWRevealer
{
    protected int m_leftMS;

    public FOWTempRevealer()
    {
    }

    static public new FOWTempRevealer Get()
    {
        return ClassObjPool<FOWTempRevealer>.Get();
    }

    public override void OnInit()
    {
        base.OnInit();
        m_leftMS = 0;
    }

    public override void OnRelease()
    {
        base.OnRelease();
    }

    public void InitInfo(Vector3 position, float radius, int leftMS)
    {
        m_position = position;
        m_radius = radius;
        m_leftMS = leftMS;
    }
    
    public override void Update(int deltaMS)
    {
        m_leftMS -= deltaMS;
        m_isValid = m_leftMS <= 0;
    }
}