using UnityEngine;

/// <summary>
/// 说明：视野对象需要实现的接口，提供主线程与子线程的共享数据与在其上进行的操作
/// 
/// 注意：在多线程同步和互斥问题：
///     1）由于处于游戏表现，子线程刷新时无需考虑游戏帧同步问题
///     2）同样，对于一个视野对象的所有数据，也不需要保证读写时数据的一致性
///     3）共享数据只用简单值类型（很重要：不能在子线程去取游戏逻辑体上的数据，如Charactor）
/// 遵循以上原则可忽略所有由于考虑同步互斥问题进行的加锁操作
/// 
/// @by wsh 2017-05-20
/// </summary>

public interface IFOWRevealer
{
    // 给FOWSystem使用的接口
    bool IsValid();
    Vector3 GetPosition();
    float GetRadius();

    // 给FOWLogic使用的接口，维护数据以及其有效性
    void Update(int deltaMS);
    void Release();
}
