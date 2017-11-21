using UnityEngine;
using Battle;

/// <summary>
/// 说明：场景战争迷雾
/// 
/// @by wsh 2017-05-20
/// </summary>

public class MapFOWRender
{
    public MapFOWRender(Transform mapParent)
    {
        FOWRender render = FOWLogic.instance.CreateRender(mapParent);
        if (render != null)
        {
            // TODO：实际项目中，自己根据场景地图设置中心点位置
            // 这里为了简单，直接居中
            float fCenterX = 0f;
            float fCenterZ = 0f;
            float scale = FOWSystem.instance.worldSize / 128f * 2.56f;

            render.transform.position = new Vector3(fCenterX, 0f, -fCenterZ);
            render.transform.eulerAngles = new Vector3(-90f, 180f, 0f);
            render.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
