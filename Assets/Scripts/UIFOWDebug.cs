using UnityEngine;

/// <summary>
/// 说明：FOWSystem效果调试和性能测试
/// 注意：测试需要，不走游戏逻辑，扩展时不应该使用FOWLogic类
/// 
/// @by wsh 2017-05-20
/// </summary>

public class UIFOWDebug : MonoBehaviour
{
    public UILabel FOWSystemLbl;
    public UILabel FOWRenderLbl;
    public UILabel FOWFogLbl;
    public UILabel FOWBlendFactorLbl;
    public UILabel FOWElapsedLbl;
    public UIInput UpdatFreqInput;
    public UIInput BlendTimeInput;
    public UIInput BlurTimeInput;
    public UIInput RadiusOffsetInput;
    public UISlider FOWBlendFactorSlider;

    protected void OnEnable()
    {
        UpdateUI();
    }
    
    protected void UpdateUI()
    {
        FOWSystemLbl.text = FOWSystem.instance.enableSystem ?
            "关闭 FOW 系统" : "开启 FOW 系统";
        FOWRenderLbl.text = FOWSystem.instance.enableRender ?
            "关闭 FOW 渲染" : "开启 FOW 渲染";
        FOWFogLbl.text = FOWSystem.instance.enableFog ?
            "关闭 FOW 迷雾" : "开启 FOW 迷雾";
        UpdatFreqInput.value = FOWSystem.instance.updateFrequency.ToString();
        BlendTimeInput.value = FOWSystem.instance.textureBlendTime.ToString();
        BlurTimeInput.value = FOWSystem.instance.blurIterations.ToString();
        RadiusOffsetInput.value = FOWSystem.instance.radiusOffset.ToString();
    }

    protected void Update()
    {
        FOWBlendFactorLbl.text = FOWSystem.instance.blendFactor.ToString();
        FOWBlendFactorSlider.value = FOWSystem.instance.blendFactor;
        FOWElapsedLbl.text = FOWSystem.instance.elapsed.ToString();
    }

    public void NguiOnClick(GameObject go)
    {
        switch (go.name)
        {
            case "closeBtn":
                {
                    gameObject.SetActive(false);
                    break;
                }
            case "FOWSystemBtn":
                {
                    FOWSystem.instance.enableSystem = !FOWSystem.instance.enableSystem;
                    break;
                }
            case "FOWRenderBtn":
                {
                    FOWSystem.instance.enableRender = !FOWSystem.instance.enableRender;
                    break;
                }
            case "FOWFogBtn":
                {
                    FOWSystem.instance.enableFog = !FOWSystem.instance.enableFog;
                    break;
                }
            case "ConformBtn":
                {
                    FOWSystem.instance.updateFrequency = float.Parse(UpdatFreqInput.value);
                    FOWSystem.instance.textureBlendTime = float.Parse(BlendTimeInput.value);
                    FOWSystem.instance.blurIterations = int.Parse(BlurTimeInput.value);
                    FOWSystem.instance.radiusOffset = float.Parse(RadiusOffsetInput.value);
                    break;
                }
            default:
                break;
        }

        UpdateUI();
    }
}
